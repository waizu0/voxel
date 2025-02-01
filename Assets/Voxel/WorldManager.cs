// -------------------------------------------------------------------------
// File: WorldManager.cs
// Local: Scripts/Managers/WorldManager.cs
//
// Gerencia a gera��o de chunks, mas agora tamb�m controla regi�es de bioma
// de tamanho "regionSize x regionSize" (em chunks), permitindo transi��o suave.
// -------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("Refer�ncias Principais")]
    public SettingsVO settings;      // Configura��es (chunkSize, chunkHeight, etc.)
    public Transform player;         // Jogador, para saber que chunks carregar
    public ChunkPool chunkPool;      // Pool de chunks
    public TerrainNoise terrainNoise;// Ru�do base (Perlin, etc.)
    public BiomeManager biomeManager;// Gerencia a lista de biomas e raridades

    [Header("Seed")]
    public int worldSeed = 0;
    private int currentSeed;

    [Header("Carregamento de Chunks")]
    public float updateInterval = 0.5f;
    private float timer;
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

    // ---------------------------------------------------
    // REGI�ES DE BIOMA
    // ---------------------------------------------------
    [Header("Tamanho da Regi�o (em Chunks)")]
    [Tooltip("Por exemplo, 12 significa que cada regi�o cobre 12x12 chunks.")]
    public int regionSize = 12;

    // Guarda o bioma principal para cada regi�o (regionCoord)
    private Dictionary<Vector2Int, BiomeDefinition> regionBiomeCache = new Dictionary<Vector2Int, BiomeDefinition>();

    // ---------------------------------------------------
    // M�TODOS UNITY
    // ---------------------------------------------------
    private void Start()
    {
        SetSeed(worldSeed);
        InitializeWorld();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateChunksAroundPlayer();
        }
    }

    // ---------------------------------------------------
    // INICIALIZA / SEED
    // ---------------------------------------------------
    private void InitializeWorld()
    {
        if (!player) return;
        UpdateChunksAroundPlayer();
    }

    public void SetSeed(int newSeed)
    {
        currentSeed = newSeed;
        if (terrainNoise != null)
        {
            terrainNoise.seed = newSeed;
        }

        RegenerateWorld();
    }

    private void RegenerateWorld()
    {
        // Desativa todos os chunks existentes
        List<Vector2Int> coordsParaLiberar = new List<Vector2Int>(activeChunks.Keys);
        foreach (var coord in coordsParaLiberar)
        {
            ReleaseChunk(coord);
        }

        // Limpa o cache de biomas por regi�o
        regionBiomeCache.Clear();

        InitializeWorld();
    }

    // ---------------------------------------------------
    // ATUALIZAR CHUNKS EM TORNO DO JOGADOR
    // ---------------------------------------------------
    private void UpdateChunksAroundPlayer()
    {
        if (!player || !settings) return;

        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);
        List<Vector2Int> neededCoords = new List<Vector2Int>();

        int loadRange = settings.viewDistanceInChunks;
        for (int x = -loadRange; x <= loadRange; x++)
        {
            for (int z = -loadRange; z <= loadRange; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                neededCoords.Add(coord);
            }
        }

        foreach (Vector2Int coord in neededCoords)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                CreateChunk(coord);
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!neededCoords.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var c in toRemove)
        {
            ReleaseChunk(c);
        }
    }

    private void CreateChunk(Vector2Int coord)
    {
        GameObject chunkObj = chunkPool.GetChunkObject();
        chunkObj.name = $"Chunk_{coord.x}_{coord.y}";
        chunkObj.transform.SetParent(this.transform);

        // Cria o chunk
        Chunk chunkComponent = chunkObj.GetComponent<Chunk>();

        // Gera dados do chunk usando a nova fun��o "GenerateChunkData" que
        // chamar� de volta este WorldManager para saber do bioma (smooth).
        chunkComponent.InitializeChunk(coord, settings, terrainNoise, this);

        activeChunks.Add(coord, chunkComponent);
    }

    private void ReleaseChunk(Vector2Int coord)
    {
        if (activeChunks.ContainsKey(coord))
        {
            Chunk c = activeChunks[coord];
            chunkPool.ReleaseChunkObject(c.gameObject);
            activeChunks.Remove(coord);
        }
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int cs = settings.chunkSize;
        int x = Mathf.FloorToInt(position.x / cs);
        int z = Mathf.FloorToInt(position.z / cs);
        return new Vector2Int(x, z);
    }

    // ---------------------------------------------------
    // C�LCULO DE BIOMA (transi��o suave)
    // ---------------------------------------------------

    /// <summary>
    /// Retorna o "fator de amplitude" do bioma para um ponto (worldX, worldZ),
    /// fazendo interpola��o bilinear entre at� 4 regi�es vizinhas.
    /// </summary>
    public float GetBiomeAmplitudeAt(float worldX, float worldZ)
    {
        // 1) Dimens�o de cada regi�o em coordenadas de mundo
        float regionWorldSize = regionSize * settings.chunkSize;

        // 2) Coordenada fracion�ria dentro da regi�o
        float rx = worldX / regionWorldSize;
        float rz = worldZ / regionWorldSize;

        // 3) Coordenada "inteira" (qual regi�o) e a fra��o local
        int regionX0 = Mathf.FloorToInt(rx);
        int regionZ0 = Mathf.FloorToInt(rz);

        float fracX = rx - regionX0; // valor [0..1) dentro da c�lula de regi�o
        float fracZ = rz - regionZ0;

        // Coordenadas das 4 "c�lulas" (regi�es) envolvidas
        Vector2Int r00 = new Vector2Int(regionX0, regionZ0);
        Vector2Int r10 = new Vector2Int(regionX0 + 1, regionZ0);
        Vector2Int r01 = new Vector2Int(regionX0, regionZ0 + 1);
        Vector2Int r11 = new Vector2Int(regionX0 + 1, regionZ0 + 1);

        // Obt�m amplitude de cada regi�o
        float a00 = GetBiomeAmplitudeForRegion(r00);
        float a10 = GetBiomeAmplitudeForRegion(r10);
        float a01 = GetBiomeAmplitudeForRegion(r01);
        float a11 = GetBiomeAmplitudeForRegion(r11);

        // Interpola��o bilinear
        float a0 = Mathf.Lerp(a00, a10, fracX); // interpola��o horizontal (entre r00 e r10)
        float a1 = Mathf.Lerp(a01, a11, fracX); // interpola��o horizontal (entre r01 e r11)
        float finalAmplitude = Mathf.Lerp(a0, a1, fracZ); // interpola��o vertical

        return finalAmplitude;
    }

    /// <summary>
    /// Retorna a amplitude definida pelo bioma principal de uma dada regi�o (regionCoord).
    /// Se a regi�o ainda n�o tiver bioma no cache, escolhe agora e armazena.
    /// </summary>
    private float GetBiomeAmplitudeForRegion(Vector2Int regionCoord)
    {
        // Tenta pegar do cache
        if (!regionBiomeCache.ContainsKey(regionCoord))
        {
            // Escolhe um bioma
            BiomeDefinition newBiome = null;
            if (biomeManager != null)
                newBiome = biomeManager.PickBiome();

            regionBiomeCache[regionCoord] = newBiome;
        }

        BiomeDefinition chosenBiome = regionBiomeCache[regionCoord];
        if (chosenBiome == null) return 1f;

        // "amplitude" no pr�prio ScriptableObject do bioma
        return chosenBiome.amplitude;
    }
}
