// -------------------------------------------------------------------------
// File: TerrainNoise.cs
// Local: Scripts/Noise/TerrainNoise.cs
//
// Responsável por fornecer funções de ruído para geração de terreno.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/TerrainNoise")]
public class TerrainNoise : ScriptableObject
{
    public int seed = 0; // Seed editável no Inspector

    /// <summary>
    /// Retorna a "altura" para uma dada posição (x, z) usando combinações de ruído.
    /// </summary>
    public float GetHeight(float x, float z, SettingsVO settings)
    {
        float freq = settings.noiseFrequency;
        float amp = settings.noiseAmplitude;

        // Aplica a seed deslocando as coordenadas
        float noiseX = x * freq + seed;
        float noiseZ = z * freq + seed;

        float noiseValue = Mathf.PerlinNoise(noiseX, noiseZ);
        float height = noiseValue * amp;

        return height;
    }
}