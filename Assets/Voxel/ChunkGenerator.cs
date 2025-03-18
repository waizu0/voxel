// -------------------------------------------------------------------------
// File: ChunkGenerator.cs
//
// Responsible for creating the voxel data for each chunk and also building
// the mesh from that data.
//
// Now, instead of receiving a fixed BiomeDefinition, we receive
// the WorldManager (which manages regions and performs smooth interpolation).
// -------------------------------------------------------------------------
using UnityEngine;

public static class ChunkGenerator
{
    /// <summary>
    /// Generates ChunkData for a given chunk coordinate,
    /// consulting the WorldManager to obtain the biome amplitude
    /// (with smooth transition).
    /// </summary>
    public static ChunkData GenerateChunkData(
        Vector2Int coord,
        SettingsVO settings,
        TerrainNoise terrainNoise,
        WorldManager worldManager)
    {
        ChunkData chunkData = new ChunkData(settings.chunkSize, settings.chunkHeight);

        int worldXStart = coord.x * settings.chunkSize;
        int worldZStart = coord.y * settings.chunkSize;

        for (int x = 0; x < settings.chunkSize; x++)
        {
            for (int z = 0; z < settings.chunkSize; z++)
            {
                // World coordinates
                float worldX = worldXStart + x;
                float worldZ = worldZStart + z;

                // 1) Get the "blended" biome amplitude (e.g., between 0 and 50 for mountainous regions)
                float biomeAmplitude = worldManager.GetBiomeAmplitudeAt(worldX, worldZ);

                // 2) Calculate base noise (from 0 to 1 in Perlin) and multiply by the final amplitude
                //    (Also multiply by 'settings.noiseAmplitude' if needed)
                float noiseValue = Mathf.PerlinNoise(
                    (worldX * settings.noiseFrequency + terrainNoise.seed),
                    (worldZ * settings.noiseFrequency + terrainNoise.seed)
                );

                float finalHeight = noiseValue * biomeAmplitude;
                // If you also want to combine with "settings.noiseAmplitude", do:
                // float finalHeight = noiseValue * biomeAmplitude * settings.noiseAmplitude;

                // 3) Convert to integer and clamp
                int maxY = Mathf.RoundToInt(finalHeight);
                maxY = Mathf.Clamp(maxY, 0, settings.chunkHeight - 1);

                // 4) Fill voxel (e.g., everything up to maxY = solid, above = air)
                for (int y = 0; y < settings.chunkHeight; y++)
                {
                    if (y <= maxY)
                        chunkData.SetBlock(x, y, z, 1); // solid block
                    else
                        chunkData.SetBlock(x, y, z, 0); // air
                }
            }
        }

        return chunkData;
    }

    /// <summary>
    /// Builds the chunk mesh (same as before).
    /// </summary>
    public static Mesh BuildChunkMesh(ChunkData chunkData)
    {
        return MeshBuilder.GenerateMesh(chunkData);
    }
}
