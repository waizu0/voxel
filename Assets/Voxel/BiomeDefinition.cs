// -------------------------------------------------------------------------
// File: BiomeDefinition.cs
// ScriptableObject that defines the parameters of an individual biome,
// including rarity and generation properties (currently amplitude).
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Biomes/BiomeDefinition")]
public class BiomeDefinition : ScriptableObject
{
    [Header("Biome Identification")]
    public string biomeName = "New Biome";

    [Header("Rarity")]
    [Tooltip("Defines the relative frequency of the biome's appearance.")]
    public float rarity = 1f;

    [Header("Height Properties")]
    [Tooltip("Noise amplitude for the biome. Mountainous biomes tend to have higher values.")]
    public float amplitude = 20f;
}