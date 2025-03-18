// -------------------------------------------------------------------------
// File: BiomeManager.cs
//
// Contains a list of BiomeDefinition and a method that randomly selects
// one of the biomes based on rarity.
// -------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Biomes/BiomeManager")]
public class BiomeManager : ScriptableObject
{
    [Header("Biome List")]
    public List<BiomeDefinition> biomes = new List<BiomeDefinition>();

    /// <summary>
    /// Returns a random biome based on the sum of rarities.
    /// </summary>
    public BiomeDefinition PickBiome()
    {
        if (biomes == null || biomes.Count == 0)
        {
            Debug.LogWarning("No biomes defined in BiomeManager.");
            return null;
        }

        // Sum all rarities
        float totalRarity = 0f;
        foreach (var biome in biomes)
        {
            if (biome != null)
            {
                totalRarity += biome.rarity;
            }
        }

        // Random selection based on the total sum
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

        // If for some reason none is selected (unlikely), return the first one
        return biomes[0];
    }
}