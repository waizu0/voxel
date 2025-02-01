// -------------------------------------------------------------------------
// File: ChunkGenerator.cs
// Local: Scripts/Core/ChunkGenerator.cs
//
// Respons�vel por criar os dados voxel de cada chunk e tamb�m construir
// a malha (mesh) a partir desses dados.
//
// Agora, em vez de receber um BiomeDefinition fixo, vamos receber
// o WorldManager (que gerencia regi�es e faz interpola��o suave).
// -------------------------------------------------------------------------
using UnityEngine;

public static class ChunkGenerator
{
    /// <summary>
    /// Gera o ChunkData para uma dada coordenada de chunk,
    /// consultando o WorldManager para obter a amplitude
    /// do bioma (com transi��o suave).
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
                // Coordenadas no mundo
                float worldX = worldXStart + x;
                float worldZ = worldZStart + z;

                // 1) Obt�m a amplitude "blend" do bioma (entre 0 e, por ex., 50 se for montanhoso)
                float biomeAmplitude = worldManager.GetBiomeAmplitudeAt(worldX, worldZ);

                // 2) Calcula ru�do base (de 0 a 1, no Perlin) e multiplica pela amplitude final
                //    (Tamb�m multiplicamos por 'settings.noiseAmplitude', se quiser)
                float noiseValue = Mathf.PerlinNoise(
                    (worldX * settings.noiseFrequency + terrainNoise.seed),
                    (worldZ * settings.noiseFrequency + terrainNoise.seed)
                );

                float finalHeight = noiseValue * biomeAmplitude;
                // Se quiser combinar com "settings.noiseAmplitude" tamb�m, fa�a:
                // float finalHeight = noiseValue * biomeAmplitude * settings.noiseAmplitude;

                // 3) Converte para inteiro e limita
                int maxY = Mathf.RoundToInt(finalHeight);
                maxY = Mathf.Clamp(maxY, 0, settings.chunkHeight - 1);

                // 4) Preenche voxel (ex.: tudo at� maxY = terra, acima = ar)
                for (int y = 0; y < settings.chunkHeight; y++)
                {
                    if (y <= maxY)
                        chunkData.SetBlock(x, y, z, 1); // bloco
                    else
                        chunkData.SetBlock(x, y, z, 0); // ar
                }
            }
        }

        return chunkData;
    }

    /// <summary>
    /// Constr�i a malha do chunk (igual a antes).
    /// </summary>
    public static Mesh BuildChunkMesh(ChunkData chunkData)
    {
        return MeshBuilder.GenerateMesh(chunkData);
    }
}
