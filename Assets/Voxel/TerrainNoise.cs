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
    public int seed = 0; // Seed edit�vel no Inspector

    /// <summary>
    /// Retorna a "altura" para uma dada posi��o (x, z) usando combina��es de ru�do.
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