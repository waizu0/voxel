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

    public void InitializeChunk(Vector2Int coord, SettingsVO settings, TerrainNoise terrainNoise)
    {
        chunkCoord = coord;
        meshFilter = GetComponent<MeshFilter>();
        if (!meshCollider) meshCollider = GetComponent<MeshCollider>();

        // Gera os dados do chunk
        chunkData = ChunkGenerator.GenerateChunkData(coord, settings, terrainNoise);

        // Gera a malha
        Mesh mesh = ChunkGenerator.BuildChunkMesh(chunkData);
        meshFilter.sharedMesh = mesh;

        // Se quiser colisão, pode atribuir a mesma mesh ou gerar outra
        if (meshCollider) meshCollider.sharedMesh = mesh;

        transform.position = new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize);
    }
}
