// -------------------------------------------------------------------------
// File: Chunk.cs
// Local: Scripts/Core/Chunk.cs
//
// Responsável por encapsular as informações de um chunk.
// -------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private Vector2Int chunkCoord;
    private ChunkData chunkData;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void InitializeChunk(
        Vector2Int coord,
        SettingsVO settings,
        TerrainNoise terrainNoise,
        WorldManager worldManager)
    {
        chunkCoord = coord;
        meshFilter = GetComponent<MeshFilter>();
        if (!meshCollider) meshCollider = GetComponent<MeshCollider>();

        // Gera dados usando o ChunkGenerator com "transição suave" via WorldManager
        chunkData = ChunkGenerator.GenerateChunkData(coord, settings, terrainNoise, worldManager);

        // Constrói a mesh
        Mesh mesh = ChunkGenerator.BuildChunkMesh(chunkData);
        meshFilter.sharedMesh = mesh;

        if (meshCollider) meshCollider.sharedMesh = mesh;

        transform.position = new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize);
    }
}
