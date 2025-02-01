// -------------------------------------------------------------------------
// File: Chunk.cs
// Local: Scripts/Core/Chunk.cs
//
// Responsável por encapsular as informações de um chunk, integrando
// geração de dados (ChunkData), construção de mesh e LOD, etc.
// -------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private Vector2Int chunkCoord;   // Coordenada do chunk no mundo
    private ChunkData chunkData;     // Dados voxel do chunk
    private MeshFilter meshFilter;   // Referência para exibir a malha gerada
    private MeshCollider meshCollider; // Caso deseje colisão

    // [BIOMA] Adicionamos esta referência para guardar o bioma do chunk, se desejado
    private BiomeDefinition currentBiome;

    /// <summary>
    /// Inicializa o Chunk com as informações necessárias para gerar seus dados.
    /// Agora aceita também o BiomeDefinition para definir a biome do chunk.
    /// </summary>
    /// <param name="coord">Coordenada do chunk no grid do mundo.</param>
    /// <param name="settings">Configurações globais do voxel.</param>
    /// <param name="terrainNoise">ScriptableObject que gera ruído.</param>
    /// <param name="biome">Bioma escolhido para este chunk.</param>
    public void InitializeChunk(Vector2Int coord, SettingsVO settings, TerrainNoise terrainNoise, BiomeDefinition biome)
    {
        chunkCoord = coord;
        meshFilter = GetComponent<MeshFilter>();
        if (!meshCollider) meshCollider = GetComponent<MeshCollider>();

        // [BIOMA] Armazena o bioma selecionado
        currentBiome = biome;

        // Gera os dados do chunk, agora levando em conta o bioma
        chunkData = ChunkGenerator.GenerateChunkData(coord, settings, terrainNoise, biome);

        // Gera a malha
        Mesh mesh = ChunkGenerator.BuildChunkMesh(chunkData);
        meshFilter.sharedMesh = mesh;

        // Se quiser colisão, pode atribuir a mesma mesh ou gerar outra
        if (meshCollider) meshCollider.sharedMesh = mesh;

        transform.position = new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize);
    }
}
