// -------------------------------------------------------------------------
// File: SettingsVO.cs
// Local: Scripts/Utilities/SettingsVO.cs
//
// Armazena parâmetros globais de configuração do mundo voxel.
// Pode ser um ScriptableObject para fácil edição no Unity.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/SettingsVO")]
public class SettingsVO : ScriptableObject
{
    [Header("Tamanho do Chunk")]
    public int chunkSize = 16;
    public int chunkHeight = 128;

    [Header("Distância de Visão (chunks)")]
    public int viewDistanceInChunks = 8;

    [Header("Parâmetros de Ruído")]
    public float noiseFrequency = 0.01f;
    public float noiseAmplitude = 20f;
}
