// -------------------------------------------------------------------------
// File: WorldManager.cs
// Local: Scripts/Managers/WorldManager.cs
//
// Respons�vel por controlar a gera��o, carregamento e descarregamento de chunks
// ao redor do jogador, comunicando-se com o sistema de LOD e demais m�dulos.
// Agora inclui um sistema de seed integrado para permitir a regenera��o do mundo,
// e a escolha de biomas via BiomeManager.
// -------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Refer�ncias")]
    public SettingsVO settings;             // Refer�ncia �s configura��es globais (ScriptableObject ou similar)
    public Transform player;                // Transform do jogador
    public ChunkPool chunkPool;             // Refer�ncia ao pool de chunks
    public TerrainNoise terrainNoise;       // Refer�ncia � classe de ru�do para gera��o

    // [BIOMA] Refer�ncia para escolher biomas
    public BiomeManager biomeManager;

    // Armazena os chunks ativos usando um Dictionary para acesso r�pido via coordenada (x,z).
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

    // Intervalo de atualiza��o para carregar/descarregar chunks, evitando checar todo frame.
    [Header("Frequ�ncia de Atualiza��o")]
    public float updateInterval = 0.5f;
    private float timer;

    [Header("Seed do Mundo")]
    public int worldSeed = 0;              // Seed principal do mundo
    private int currentSeed;               // Rastreia a seed atual em uso

    private void Start()
    {
        // Define o seed inicial a partir da vari�vel `worldSeed`.
        SetSeed(worldSeed);

        // Inicia a gera��o inicial do mundo.
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

    /// <summary>
    /// Configura o mundo inicial e gera os primeiros chunks.
    /// </summary>
    private void InitializeWorld()
    {
        if (!player) return;
        UpdateChunksAroundPlayer();
    }

    /// <summary>
    /// Atualiza e/ou regenera o mundo com base na seed fornecida.
    /// Esse m�todo pode ser chamado a qualquer momento para mudar a seed em runtime.
    /// </summary>
    /// <param name="newSeed">Nova seed para o mundo.</param>
    public void SetSeed(int newSeed)
    {
        // Atualiza a seed do TerrainNoise e o campo de controle local.
        currentSeed = newSeed;
        if (terrainNoise != null)
        {
            terrainNoise.seed = newSeed;
        }

        // Caso voc� queira reinicializar completamente a gera��o (descarregar e recarregar chunks):
        RegenerateWorld();
    }

    /// <summary>
    /// Limpa todos os chunks atuais e for�a a recarga com a nova seed.
    /// </summary>
    private void RegenerateWorld()
    {
        // Remove todos os chunks ativos (devolvendo-os para o pool).
        List<Vector2Int> coordsParaLiberar = new List<Vector2Int>(activeChunks.Keys);
        foreach (var coord in coordsParaLiberar)
        {
            ReleaseChunk(coord);
        }

        // Reinicializa o mundo com a seed j� definida.
        InitializeWorld();
    }

    /// <summary>
    /// Identifica os chunks que precisam estar ativos em torno do jogador e
    /// chama o sistema de cria��o/remo��o de chunks.
    /// </summary>
    private void UpdateChunksAroundPlayer()
    {
        if (!player || !settings) return;

        // Coordenadas do chunk em que o jogador est�.
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);

        // Lista de todos os chunks que dever�o estar ativos.
        List<Vector2Int> neededCoords = new List<Vector2Int>();

        // Determina o raio de chunks em cada dire��o.
        int loadRange = settings.viewDistanceInChunks;

        for (int x = -loadRange; x <= loadRange; x++)
        {
            for (int z = -loadRange; z <= loadRange; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                neededCoords.Add(coord);
            }
        }

        // Ativa ou cria os chunks necess�rios.
        foreach (Vector2Int coord in neededCoords)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                CreateChunk(coord);
            }
        }

        // Desativa ou libera chunks que n�o s�o mais necess�rios.
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!neededCoords.Contains(kvp.Key))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var coord in keysToRemove)
        {
            ReleaseChunk(coord);
        }
    }

    /// <summary>
    /// Instancia ou obt�m do pool um novo chunk, configura-o e armazena no dicion�rio.
    /// Agora tamb�m escolhe um bioma para o chunk de forma aleat�ria.
    /// </summary>
    private void CreateChunk(Vector2Int coord)
    {
        // Obt�m um GameObject de chunk do pool.
        GameObject chunkObj = chunkPool.GetChunkObject();
        chunkObj.name = $"Chunk_{coord.x}_{coord.y}";
        chunkObj.transform.SetParent(this.transform);

        Chunk chunkComponent = chunkObj.GetComponent<Chunk>();

        // [BIOMA] Escolhemos um bioma do BiomeManager
        BiomeDefinition chosenBiome = null;
        if (biomeManager != null)
        {
            chosenBiome = biomeManager.PickBiome();
        }

        // Inicializa o chunk com o bioma escolhido
        chunkComponent.InitializeChunk(coord, settings, terrainNoise, chosenBiome);

        activeChunks.Add(coord, chunkComponent);
    }

    /// <summary>
    /// Devolve o chunk ao pool e remove-o da lista de ativos.
    /// </summary>
    private void ReleaseChunk(Vector2Int coord)
    {
        if (activeChunks.ContainsKey(coord))
        {
            Chunk chunkComponent = activeChunks[coord];
            chunkPool.ReleaseChunkObject(chunkComponent.gameObject);
            activeChunks.Remove(coord);
        }
    }

    /// <summary>
    /// Converte a posi��o do mundo em coordenadas de chunk (x,z).
    /// </summary>
    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int chunkSize = settings.chunkSize;
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        return new Vector2Int(x, z);
    }
}
