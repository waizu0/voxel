// -------------------------------------------------------------------------
// File: ChunkGenerator.cs
// Local: Scripts/Core/ChunkGenerator.cs
//
// Responsável por criar os dados voxel de cada chunk e também construir
// a malha (mesh) a partir desses dados.
// -------------------------------------------------------------------------
using UnityEngine;

public static class ChunkGenerator
{
    /// <summary>
    /// Gera os dados de voxels (ChunkData) para a coordenada informada.
    /// </summary>
    public static ChunkData GenerateChunkData(Vector2Int coord, SettingsVO settings, TerrainNoise terrainNoise)
    {
        ChunkData chunkData = new ChunkData(settings.chunkSize, settings.chunkHeight);

        int worldXStart = coord.x * settings.chunkSize;
        int worldZStart = coord.y * settings.chunkSize;

        for (int x = 0; x < settings.chunkSize; x++)
        {
            for (int z = 0; z < settings.chunkSize; z++)
            {
                // Calcula a altura via ruído.
                float height = terrainNoise.GetHeight(worldXStart + x, worldZStart + z, settings);

                // Limita ao intervalo [0, chunkHeight)
                int maxHeight = Mathf.Clamp(Mathf.RoundToInt(height), 0, settings.chunkHeight - 1);

                for (int y = 0; y < settings.chunkHeight; y++)
                {
                    if (y <= maxHeight)
                    {
                        // Define blocos de "terra" (p. ex. ID = 1).
                        chunkData.SetBlock(x, y, z, 1);
                    }
                    else
                    {
                        // Define 0 como ar.
                        chunkData.SetBlock(x, y, z, 0);
                    }
                }
            }
        }

        return chunkData;
    }

    /// <summary>
    /// Constrói uma malha (Mesh) a partir dos dados voxel do chunk.
    /// </summary>
    public static Mesh BuildChunkMesh(ChunkData chunkData)
    {
        return MeshBuilder.GenerateMesh(chunkData);
    }
}
