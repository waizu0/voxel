// -------------------------------------------------------------------------
// File: TerrainNoise.cs
//
// Responsible for providing noise functions for terrain generation.
// Now incorporates the amplitude factor of the selected biome.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/TerrainNoise")]
public class TerrainNoise : ScriptableObject
{
    public int seed = 0; // Editable seed in the Inspector

    /// <summary>
    /// Returns the "height" for a given position (x, z), using noise combinations
    /// and now the amplitude factor of the biome.
    /// </summary>
    /// <param name="x">X coordinate in the world.</param>
    /// <param name="z">Z coordinate in the world.</param>
    /// <param name="settings">Global noise parameters.</param>
    /// <param name="biome">Biome that provides the elevation amplitude.</param>
    public float GetHeight(float x, float z, SettingsVO settings, BiomeDefinition biome)
    {
        float freq = settings.noiseFrequency;
        float baseAmp = settings.noiseAmplitude;

        // Applies the seed by shifting the coordinates
        float noiseX = x * freq + seed;
        float noiseZ = z * freq + seed;

        float noiseValue = Mathf.PerlinNoise(noiseX, noiseZ);

        // [BIOME] Multiply by the amplitude defined in the biome
        float height = noiseValue * baseAmp * (biome != null ? biome.amplitude : 1f);

        return height;
    }
}
