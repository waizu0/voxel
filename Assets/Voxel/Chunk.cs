// -------------------------------------------------------------------------
// File: Chunk.cs
// Local: Scripts/Core/Chunk.cs
//
// Respons�vel por encapsular as informa��es de um chunk, integrando
// gera��o de dados (ChunkData), constru��o de mesh e LOD, etc.
// -------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private Vector2Int chunkCoord;   // Coordenada do chunk no mundo
    private ChunkData chunkData;     // Dados voxel do chunk
    private MeshFilter meshFilter;   // Refer�ncia para exibir a malha gerada
    private MeshCollider meshCollider; // Caso deseje colis�o

    // [BIOMA] Adicionamos esta refer�ncia para guardar o bioma do chunk, se desejado
    private BiomeDefinition currentBiome;

    /// <summary>
    /// Inicializa o Chunk com as informa��es necess�rias para gerar seus dados.
    /// Agora aceita tamb�m o BiomeDefinition para definir a biome do chunk.
    /// </summary>
    /// <param name="coord">Coordenada do chunk no grid do mundo.</param>
    /// <param name="settings">Configura��es globais do voxel.</param>
    /// <param name="terrainNoise">ScriptableObject que gera ru�do.</param>
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

        // Se quiser colis�o, pode atribuir a mesma mesh ou gerar outra
        if (meshCollider) meshCollider.sharedMesh = mesh;

        transform.position = new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize);
    }
}
