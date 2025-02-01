// -------------------------------------------------------------------------
// File: ChunkData.cs
// Local: Scripts/Core/ChunkData.cs
//
// Armazena as informa��es cruas do chunk, incluindo IDs de blocos em um array
// tridimensional, al�m de m�todos de acesso/manipula��o.
// -------------------------------------------------------------------------
public class ChunkData
{
    private int[,,] blocks;
    private int chunkSize;
    private int chunkHeight;

    public int ChunkSize => chunkSize;
    public int ChunkHeight => chunkHeight;

    public ChunkData(int size, int height)
    {
        chunkSize = size;
        chunkHeight = height;
        blocks = new int[size, height, size];
    }

    public void SetBlock(int x, int y, int z, int blockID)
    {
        blocks[x, y, z] = blockID;
    }

    public int GetBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
}
