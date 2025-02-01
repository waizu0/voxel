// -------------------------------------------------------------------------
// File: SettingsVO.cs
// Local: Scripts/Utilities/SettingsVO.cs
//
// Armazena par�metros globais de configura��o do mundo voxel.
// Pode ser um ScriptableObject para f�cil edi��o no Unity.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/SettingsVO")]
public class SettingsVO : ScriptableObject
{
    [Header("Tamanho do Chunk")]
    public int chunkSize = 16;
    public int chunkHeight = 128;

    [Header("Dist�ncia de Vis�o (chunks)")]
    public int viewDistanceInChunks = 8;

    [Header("Par�metros de Ru�do")]
    public float noiseFrequency = 0.01f;
    public float noiseAmplitude = 20f;
}
