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
    /// <summary>
    /// Retorna a "altura" para uma dada posição (x, z) usando combinações de ruído.
    /// </summary>
    public float GetHeight(float x, float z, SettingsVO settings)
    {
        // Exemplo simples usando Mathf.PerlinNoise
        float freq = settings.noiseFrequency;
        float amp = settings.noiseAmplitude;

        float noiseValue = Mathf.PerlinNoise(x * freq, z * freq);
        float height = noiseValue * amp;

        return height;
    }
}
