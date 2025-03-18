// -------------------------------------------------------------------------
// File: ChunkData.cs
//
// Stores the raw information of the chunk, including block IDs in a
// three-dimensional array, as well as access/manipulation methods.
// -------------------------------------------------------------------------
public class ChunkData
{
    private int[,,] blocks;
    private int chunkSize;
    private int chunkHeight;

    public int ChunkSize => chunkSize;
    public int ChunkHeight => chunkHeight;

    // [BIOME] New property to store the biome of this chunk
    public BiomeDefinition chosenBiome { get; private set; }

    /// <summary>
    /// ChunkData constructor. Sets the size and height of the chunk,
    /// and creates the block matrix.
    /// </summary>
    public ChunkData(int size, int height)
    {
        chunkSize = size;
        chunkHeight = height;
        blocks = new int[size, height, size];
    }

    /// <summary>
    /// Sets the chosen biome for this ChunkData.
    /// </summary>
    public void SetBiome(BiomeDefinition biome)
    {
        chosenBiome = biome;
    }

    /// <summary>
    /// Sets the block ID at (x,y,z).
    /// </summary>
    public void SetBlock(int x, int y, int z, int blockID)
    {
        blocks[x, y, z] = blockID;
    }

    /// <summary>
    /// Returns the block ID at (x,y,z).
    /// </summary>
    public int GetBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
}