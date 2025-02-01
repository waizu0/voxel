// -------------------------------------------------------------------------
// File: BiomeManager.cs
// Local: Scripts/Biomes/BiomeManager.cs
//
// Cont�m uma lista de BiomeDefinition e um m�todo que seleciona
// aleatoriamente um dos biomas com base na raridade.
// -------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Biomes/BiomeManager")]
public class BiomeManager : ScriptableObject
{
    [Header("Lista de Biomas")]
    public List<BiomeDefinition> biomes = new List<BiomeDefinition>();

    /// <summary>
    /// Retorna um bioma aleat�rio baseado na soma de raridades.
    /// </summary>
    public BiomeDefinition PickBiome()
    {
        if (biomes == null || biomes.Count == 0)
        {
            Debug.LogWarning("Nenhum bioma definido no BiomeManager.");
            return null;
        }

        // Soma todas as raridades
        float totalRarity = 0f;
        foreach (var biome in biomes)
        {
            if (biome != null)
            {
                totalRarity += biome.rarity;
            }
        }

        // Escolha aleat�ria baseada na soma
        float randomValue = Random.value * totalRarity;
        float cumulative = 0f;

        foreach (var biome in biomes)
        {
            if (biome == null) continue;

            cumulative += biome.rarity;
            if (randomValue <= cumulative)
            {
                return biome;
            }
        }

        // Se por algum motivo n�o cair em nenhum (improv�vel), retorne o primeiro
        return biomes[0];
    }
}
