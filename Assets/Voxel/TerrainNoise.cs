// -------------------------------------------------------------------------
// File: TerrainNoise.cs
// Local: Scripts/Noise/TerrainNoise.cs
//
// Responsável por fornecer funções de ruído para geração de terreno.
// Agora incorpora o fator de amplitude do bioma selecionado.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Noise/TerrainNoise")]
public class TerrainNoise : ScriptableObject
{
    public int seed = 0; // Seed editável no Inspector

    /// <summary>
    /// Retorna a "altura" para uma dada posição (x, z), usando combinações
    /// de ruído e agora o fator de amplitude do bioma.
    /// </summary>
    /// <param name="x">Coordenada x no mundo.</param>
    /// <param name="z">Coordenada z no mundo.</param>
    /// <param name="settings">Parâmetros de ruído globais.</param>
    /// <param name="biome">Bioma que fornece a amplitude de elevação.</param>
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
