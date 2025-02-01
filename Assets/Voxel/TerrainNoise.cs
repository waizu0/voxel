// -------------------------------------------------------------------------
// File: TerrainNoise.cs
// Local: Scripts/Noise/TerrainNoise.cs
//
// Respons�vel por fornecer fun��es de ru�do para gera��o de terreno.
// Agora incorpora o fator de amplitude do bioma selecionado.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/TerrainNoise")]
public class TerrainNoise : ScriptableObject
{
    public int seed = 0; // Seed edit�vel no Inspector

    /// <summary>
    /// Retorna a "altura" para uma dada posi��o (x, z), usando combina��es
    /// de ru�do e agora o fator de amplitude do bioma.
    /// </summary>
    /// <param name="x">Coordenada x no mundo.</param>
    /// <param name="z">Coordenada z no mundo.</param>
    /// <param name="settings">Par�metros de ru�do globais.</param>
    /// <param name="biome">Bioma que fornece a amplitude de eleva��o.</param>
    public float GetHeight(float x, float z, SettingsVO settings, BiomeDefinition biome)
    {
        float freq = settings.noiseFrequency;
        float baseAmp = settings.noiseAmplitude;

        // Aplica a seed deslocando as coordenadas
        float noiseX = x * freq + seed;
        float noiseZ = z * freq + seed;

        float noiseValue = Mathf.PerlinNoise(noiseX, noiseZ);

        // [BIOMA] Multiplicamos pela amplitude definida no bioma
        float height = noiseValue * baseAmp * (biome != null ? biome.amplitude : 1f);

        return height;
    }
}
