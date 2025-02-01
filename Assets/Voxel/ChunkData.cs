// -------------------------------------------------------------------------
// File: ChunkData.cs
// Local: Scripts/Core/ChunkData.cs
//
// Armazena as informações cruas do chunk, incluindo IDs de blocos em um array
// tridimensional, além de métodos de acesso/manipulação.
// -------------------------------------------------------------------------
public class ChunkData
{
    private int[,,] blocks;
    private int chunkSize;
    private int chunkHeight;

    public int ChunkSize => chunkSize;
    public int ChunkHeight => chunkHeight;

    // [BIOMA] Nova propriedade para armazenar o bioma deste chunk
    public BiomeDefinition chosenBiome { get; private set; }

    /// <summary>
    /// Construtor do ChunkData. Configura o tamanho e altura do chunk,
    /// além de criar a matriz de blocos.
    /// </summary>
    public ChunkData(int size, int height)
    {
        chunkSize = size;
        chunkHeight = height;
        blocks = new int[size, height, size];
    }

    /// <summary>
    /// Define o bioma escolhido para este ChunkData.
    /// </summary>
    public void SetBiome(BiomeDefinition biome)
    {
        chosenBiome = biome;
    }

    /// <summary>
    /// Define o ID de bloco em (x,y,z).
    /// </summary>
    public void SetBlock(int x, int y, int z, int blockID)
    {
        blocks[x, y, z] = blockID;
    }

    /// <summary>
    /// Retorna o ID de bloco em (x,y,z).
    /// </summary>
    public int GetBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
}
