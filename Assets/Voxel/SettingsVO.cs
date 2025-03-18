// -------------------------------------------------------------------------
// File: SettingsVO.cs
//
// Stores global configuration parameters for the voxel world.
// Can be a ScriptableObject for easy editing in Unity.
// -------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/SettingsVO")]
public class SettingsVO : ScriptableObject
{
    [Header("Chunk Size")]
    public int chunkSize = 16;
    public int chunkHeight = 128;

    [Header("View Distance (chunks)")]
    public int viewDistanceInChunks = 8;

    [Header("Noise Parameters")]
    public float noiseFrequency = 0.01f;
    public float noiseAmplitude = 20f;
}