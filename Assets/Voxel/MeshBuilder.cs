// -------------------------------------------------------------------------
// File: MeshBuilder.cs
// Local: Scripts/Core/MeshBuilder.cs
//
// Responsável pela criação efetiva da malha (mesh) de um chunk, analisando
// cada voxel e gerando faces apenas onde necessário.
// -------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder
{
    /// <summary>
    /// Gera a malha de um chunk, verificando as faces expostas de cada voxel.
    /// </summary>
    public static Mesh GenerateMesh(ChunkData chunkData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int size = chunkData.ChunkSize;
        int height = chunkData.ChunkHeight;

        // Percorre cada voxel
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int blockID = chunkData.GetBlock(x, y, z);
                    if (blockID == 0)
                    {
                        continue; // Ar, não gera faces
                    }

                    // Checa faces vizinhas (x-1, x+1, y-1, y+1, z-1, z+1)
                    // Gerar face se for ar ou fora dos limites do chunk.

                    // Face -X
                    if (IsBlockTransparent(chunkData, x - 1, y, z))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x, y, z),
                            new Vector3(x, y + 1, z),
                            new Vector3(x, y + 1, z + 1),
                            new Vector3(x, y, z + 1)
                        );
                    }

                    // Face +X
                    if (IsBlockTransparent(chunkData, x + 1, y, z))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x + 1, y, z + 1),
                            new Vector3(x + 1, y + 1, z + 1),
                            new Vector3(x + 1, y + 1, z),
                            new Vector3(x + 1, y, z)
                        );
                    }

                    // Face -Y
                    if (IsBlockTransparent(chunkData, x, y - 1, z))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x, y, z + 1),
                            new Vector3(x, y, z),
                            new Vector3(x + 1, y, z),
                            new Vector3(x + 1, y, z + 1)
                        );
                    }

                    // Face +Y
                    if (IsBlockTransparent(chunkData, x, y + 1, z))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x, y + 1, z),
                            new Vector3(x, y + 1, z + 1),
                            new Vector3(x + 1, y + 1, z + 1),
                            new Vector3(x + 1, y + 1, z)
                        );
                    }

                    // Face -Z
                    if (IsBlockTransparent(chunkData, x, y, z - 1))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x + 1, y, z),
                            new Vector3(x + 1, y + 1, z),
                            new Vector3(x, y + 1, z),
                            new Vector3(x, y, z)
                        );
                    }

                    // Face +Z
                    if (IsBlockTransparent(chunkData, x, y, z + 1))
                    {
                        BuildFace(
                            vertices, triangles, uvs,
                            new Vector3(x, y, z + 1),
                            new Vector3(x, y + 1, z + 1),
                            new Vector3(x + 1, y + 1, z + 1),
                            new Vector3(x + 1, y, z + 1)
                        );
                    }
                }
            }
        }

        // Cria a malha final
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Permite mais de 65k vértices
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Verifica se a posição dada é "transparente" (fora dos limites ou bloco == 0).
    /// </summary>
    private static bool IsBlockTransparent(ChunkData chunkData, int x, int y, int z)
    {
        if (x < 0 || x >= chunkData.ChunkSize ||
            y < 0 || y >= chunkData.ChunkHeight ||
            z < 0 || z >= chunkData.ChunkSize)
        {
            // Fora dos limites, considere transparente
            return true;
        }

        return (chunkData.GetBlock(x, y, z) == 0);
    }

    /// <summary>
    /// Constrói uma face quad a partir de 4 vértices e adiciona ao buffer.
    /// </summary>
    private static void BuildFace(
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector2> uvs,
        Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int index = vertices.Count;

        // Adiciona 4 vértices
        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        // Adiciona 2 triângulos (0,1,2) e (0,2,3) no quad
        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        // UVs básicos (poderia mapear com atlas de texturas)
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
    }
}

