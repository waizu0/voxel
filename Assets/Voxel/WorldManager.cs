// -------------------------------------------------------------------------
// File: WorldManager.cs
// Manages chunk generation and now also controls biome regions
// of size "regionSize x regionSize" (in chunks), enabling smooth transitions.
// -------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("Main References")]
    public SettingsVO settings;      // Settings (chunkSize, chunkHeight, etc.)
    public Transform player;         // Player, to determine which chunks to load
    public ChunkPool chunkPool;      // Pool of chunks
    public TerrainNoise terrainNoise;// Base noise (Perlin, etc.)
    public BiomeManager biomeManager;// Manages the list of biomes and rarities

    [Header("Seed")]
    public int worldSeed = 0;
    private int currentSeed;

    [Header("Chunk Loading")]
    public float updateInterval = 0.5f;
    private float timer;
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

    // ---------------------------------------------------
    // BIOME REGIONS
    // ---------------------------------------------------
    [Header("Region Size (in Chunks)")]
    [Tooltip("For example, 12 means each region covers 12x12 chunks.")]
    public int regionSize = 12;

    // Stores the primary biome for each region (regionCoord)
    private Dictionary<Vector2Int, BiomeDefinition> regionBiomeCache = new Dictionary<Vector2Int, BiomeDefinition>();

    // ---------------------------------------------------
    // UNITY METHODS
    // ---------------------------------------------------
    private void Start()
    {
        SetSeed(worldSeed);
        InitializeWorld();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateChunksAroundPlayer();
        }
    }

    // ---------------------------------------------------
    // INITIALIZATION / SEED
    // ---------------------------------------------------
    private void InitializeWorld()
    {
        if (!player) return;
        UpdateChunksAroundPlayer();
    }

    public void SetSeed(int newSeed)
    {
        currentSeed = newSeed;
        if (terrainNoise != null)
        {
            terrainNoise.seed = newSeed;
        }

        RegenerateWorld();
    }

    private void RegenerateWorld()
    {
        List<Vector2Int> coordsToRelease = new List<Vector2Int>(activeChunks.Keys);
        foreach (var coord in coordsToRelease)
        {
            ReleaseChunk(coord);
        }

        regionBiomeCache.Clear();
        InitializeWorld();
    }

    // ---------------------------------------------------
    // UPDATE CHUNKS AROUND PLAYER
    // ---------------------------------------------------
    private void UpdateChunksAroundPlayer()
    {
        if (!player || !settings) return;

        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(player.position);
        List<Vector2Int> neededCoords = new List<Vector2Int>();

        int loadRange = settings.viewDistanceInChunks;
        for (int x = -loadRange; x <= loadRange; x++)
        {
            for (int z = -loadRange; z <= loadRange; z++)
            {
                Vector2Int coord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                neededCoords.Add(coord);
            }
        }

        foreach (Vector2Int coord in neededCoords)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                CreateChunk(coord);
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!neededCoords.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var c in toRemove)
        {
            ReleaseChunk(c);
        }
    }

    private void CreateChunk(Vector2Int coord)
    {
        GameObject chunkObj = chunkPool.GetChunkObject();
        chunkObj.name = $"Chunk_{coord.x}_{coord.y}";
        chunkObj.transform.SetParent(this.transform);

        Chunk chunkComponent = chunkObj.GetComponent<Chunk>();
        chunkComponent.InitializeChunk(coord, settings, terrainNoise, this);

        activeChunks.Add(coord, chunkComponent);
    }

    private void ReleaseChunk(Vector2Int coord)
    {
        if (activeChunks.ContainsKey(coord))
        {
            Chunk c = activeChunks[coord];
            chunkPool.ReleaseChunkObject(c.gameObject);
            activeChunks.Remove(coord);
        }
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int cs = settings.chunkSize;
        int x = Mathf.FloorToInt(position.x / cs);
        int z = Mathf.FloorToInt(position.z / cs);
        return new Vector2Int(x, z);
    }

    // ---------------------------------------------------
    // BIOME CALCULATION (smooth transition)
    // ---------------------------------------------------

    public float GetBiomeAmplitudeAt(float worldX, float worldZ)
    {
        float regionWorldSize = regionSize * settings.chunkSize;
        float rx = worldX / regionWorldSize;
        float rz = worldZ / regionWorldSize;
        int regionX0 = Mathf.FloorToInt(rx);
        int regionZ0 = Mathf.FloorToInt(rz);

        float fracX = rx - regionX0;
        float fracZ = rz - regionZ0;

        Vector2Int r00 = new Vector2Int(regionX0, regionZ0);
        Vector2Int r10 = new Vector2Int(regionX0 + 1, regionZ0);
        Vector2Int r01 = new Vector2Int(regionX0, regionZ0 + 1);
        Vector2Int r11 = new Vector2Int(regionX0 + 1, regionZ0 + 1);

        float a00 = GetBiomeAmplitudeForRegion(r00);
        float a10 = GetBiomeAmplitudeForRegion(r10);
        float a01 = GetBiomeAmplitudeForRegion(r01);
        float a11 = GetBiomeAmplitudeForRegion(r11);

        float a0 = Mathf.Lerp(a00, a10, fracX);
        float a1 = Mathf.Lerp(a01, a11, fracX);
        float finalAmplitude = Mathf.Lerp(a0, a1, fracZ);

        return finalAmplitude;
    }

    private float GetBiomeAmplitudeForRegion(Vector2Int regionCoord)
    {
        if (!regionBiomeCache.ContainsKey(regionCoord))
        {
            BiomeDefinition newBiome = null;
            if (biomeManager != null)
                newBiome = biomeManager.PickBiome();

            regionBiomeCache[regionCoord] = newBiome;
        }

        BiomeDefinition chosenBiome = regionBiomeCache[regionCoord];
        if (chosenBiome == null) return 1f;

        return chosenBiome.amplitude;
    }
}
