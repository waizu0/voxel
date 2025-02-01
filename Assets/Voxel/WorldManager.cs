// -------------------------------------------------------------------------
// File: WorldManager.cs
// Local: Scripts/Managers/WorldManager.cs
//
// Gerencia a geração de chunks, mas agora também controla regiões de bioma
// de tamanho "regionSize x regionSize" (em chunks), permitindo transição suave.
// -------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("Referências Principais")]
    public SettingsVO settings;      // Configurações (chunkSize, chunkHeight, etc.)
    public Transform player;         // Jogador, para saber que chunks carregar
    public ChunkPool chunkPool;      // Pool de chunks
    public TerrainNoise terrainNoise;// Ruído base (Perlin, etc.)
    public BiomeManager biomeManager;// Gerencia a lista de biomas e raridades

    [Header("Seed")]
    public int worldSeed = 0;
    private int currentSeed;

    [Header("Carregamento de Chunks")]
    public float updateInterval = 0.5f;
    private float timer;
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

    // ---------------------------------------------------
    // REGIÕES DE BIOMA
    // ---------------------------------------------------
    [Header("Tamanho da Região (em Chunks)")]
    [Tooltip("Por exemplo, 12 significa que cada região cobre 12x12 chunks.")]
    public int regionSize = 12;

    // Guarda o bioma principal para cada região (regionCoord)
    private Dictionary<Vector2Int, BiomeDefinition> regionBiomeCache = new Dictionary<Vector2Int, BiomeDefinition>();

    // ---------------------------------------------------
    // MÉTODOS UNITY
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

        // Limpa o cache de biomas por região
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

        // Gera dados do chunk usando a nova função "GenerateChunkData" que
        // chamará de volta este WorldManager para saber do bioma (smooth).
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
    // CÁLCULO DE BIOMA (transição suave)
    // ---------------------------------------------------

    /// <summary>
    /// Retorna o "fator de amplitude" do bioma para um ponto (worldX, worldZ),
    /// fazendo interpolação bilinear entre até 4 regiões vizinhas.
    /// </summary>
    public float GetBiomeAmplitudeAt(float worldX, float worldZ)
    {
        // 1) Dimensão de cada região em coordenadas de mundo
        float regionWorldSize = regionSize * settings.chunkSize;

        // 2) Coordenada fracionária dentro da região
        float rx = worldX / regionWorldSize;
        float rz = worldZ / regionWorldSize;

        // 3) Coordenada "inteira" (qual região) e a fração local
        int regionX0 = Mathf.FloorToInt(rx);
        int regionZ0 = Mathf.FloorToInt(rz);

        float fracX = rx - regionX0; // valor [0..1) dentro da célula de região
        float fracZ = rz - regionZ0;

        // Coordenadas das 4 "células" (regiões) envolvidas
        Vector2Int r00 = new Vector2Int(regionX0, regionZ0);
        Vector2Int r10 = new Vector2Int(regionX0 + 1, regionZ0);
        Vector2Int r01 = new Vector2Int(regionX0, regionZ0 + 1);
        Vector2Int r11 = new Vector2Int(regionX0 + 1, regionZ0 + 1);

        // Obtém amplitude de cada região
        float a00 = GetBiomeAmplitudeForRegion(r00);
        float a10 = GetBiomeAmplitudeForRegion(r10);
        float a01 = GetBiomeAmplitudeForRegion(r01);
        float a11 = GetBiomeAmplitudeForRegion(r11);

        // Interpolação bilinear
        float a0 = Mathf.Lerp(a00, a10, fracX); // interpolação horizontal (entre r00 e r10)
        float a1 = Mathf.Lerp(a01, a11, fracX); // interpolação horizontal (entre r01 e r11)
        float finalAmplitude = Mathf.Lerp(a0, a1, fracZ); // interpolação vertical

        return finalAmplitude;
    }

    /// <summary>
    /// Retorna a amplitude definida pelo bioma principal de uma dada região (regionCoord).
    /// Se a região ainda não tiver bioma no cache, escolhe agora e armazena.
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

        // "amplitude" no próprio ScriptableObject do bioma
        return chosenBiome.amplitude;
    }
}
