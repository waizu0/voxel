// -------------------------------------------------------------------------
// File: TerrainNoise.cs
// Local: Scripts/Noise/TerrainNoise.cs
//
// Respons�vel por fornecer fun��es de ru�do para gera��o de terreno.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/TerrainNoise")]
public class TerrainNoise : ScriptableObject
{
    /// <summary>
    /// Retorna a "altura" para uma dada posi��o (x, z) usando combina��es de ru�do.
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
