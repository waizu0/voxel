// -------------------------------------------------------------------------
// File: WorldManager.cs
// Local: Scripts/Managers/WorldManager.cs
//
// Responsável por controlar a geração, carregamento e descarregamento de chunks
// ao redor do jogador, comunicando-se com o sistema de LOD e demais módulos.
// -------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Referências")]
    public SettingsVO settings;             // Referência às configurações globais (ScriptableObject ou similar)
    public Transform player;                // Transform do jogador
    public ChunkPool chunkPool;             // Referência ao pool de chunks
    public TerrainNoise terrainNoise;       // Referência à classe de ruído para geração

    // Armazena os chunks ativos usando um Dictionary para acesso rápido via coordenada (x,z).
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

    // Intervalo de atualização para carregar/descarregar chunks, evitando checar todo frame.
    [Header("Frequência de Atualização")]
    public float updateInterval = 0.5f;
    private float timer;

    private void Start()
    {
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
    /// Identifica os chunks que precisam estar ativos em torno do jogador e
    /// chama o sistema de criação/remoção de chunks.
    /// </summary>
    private void UpdateChunksAroundPlayer()
    {
        if (!player || !settings) return;

        // Coordenadas do chunk em que o jogador está.
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);

        // Lista de todos os chunks que deverão estar ativos.
        List<Vector2Int> neededCoords = new List<Vector2Int>();

        // Determina o raio de chunks em cada direção.
        int loadRange = settings.viewDistanceInChunks;

        for (int x = -loadRange; x <= loadRange; x++)
        {
            for (int z = -loadRange; z <= loadRange; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                neededCoords.Add(coord);
            }
        }

        // Ativa ou cria os chunks necessários.
        foreach (Vector2Int coord in neededCoords)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                CreateChunk(coord);
            }
        }

        // Desativa ou libera chunks que não são mais necessários.
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
    /// Instancia ou obtém do pool um novo chunk, configura-o e armazena no dicionário.
    /// </summary>
    private void CreateChunk(Vector2Int coord)
    {
        // Obtém um GameObject de chunk do pool.
        GameObject chunkObj = chunkPool.GetChunkObject();
        chunkObj.name = $"Chunk_{coord.x}_{coord.y}";
        chunkObj.transform.SetParent(this.transform);

        Chunk chunkComponent = chunkObj.GetComponent<Chunk>();
        chunkComponent.InitializeChunk(coord, settings, terrainNoise);
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
    /// Converte a posição do mundo em coordenadas de chunk (x,z).
    /// </summary>
    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int chunkSize = settings.chunkSize;
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        return new Vector2Int(x, z);
    }
}

