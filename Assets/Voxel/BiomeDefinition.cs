// -------------------------------------------------------------------------
// File: BiomeDefinition.cs
// Local: Scripts/Biomes/BiomeDefinition.cs
//
// ScriptableObject que define os par�metros de um bioma individual,
// incluindo raridade e propriedades de gera��o (por enquanto amplitude).
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Biomes/BiomeDefinition")]
public class BiomeDefinition : ScriptableObject
{
    [Header("Identifica��o do Bioma")]
    public string biomeName = "Novo Bioma";

    [Header("Raridade")]
    [Tooltip("Define a frequ�ncia relativa de aparecimento do bioma.")]
    public float rarity = 1f;

    [Header("Propriedades de Altura")]
    [Tooltip("Amplitude do ru�do para o bioma. Biomas montanhosos tendem a ter valores maiores.")]
    public float amplitude = 20f;
}
