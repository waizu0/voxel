// -------------------------------------------------------------------------
// File: ChunkPool.cs
// Local: Scripts/Managers/ChunkPool.cs
//
// Implementa pooling de objetos de chunk para reduzir custo de criação/
// destruição, mantendo um estoque de GameObjects prontos para uso.
// -------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    [Header("Configurações de Pool")]
    public GameObject chunkPrefab;
    public int initialPoolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject chunkObj = CreateNewChunkObject();
            pool.Enqueue(chunkObj);
        }
    }

    /// <summary>
    /// Retorna um chunk do pool, ou cria um novo se estiver vazio.
    /// </summary>
    public GameObject GetChunkObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return CreateNewChunkObject();
        }
    }

    /// <summary>
    /// Devolve o chunk ao pool, desativando-o.
    /// </summary>
    public void ReleaseChunkObject(GameObject chunkObj)
    {
        chunkObj.SetActive(false);
        pool.Enqueue(chunkObj);
    }

    /// <summary>
    /// Cria um novo objeto de chunk a partir do prefab.
    /// </summary>
    private GameObject CreateNewChunkObject()
    {
        GameObject obj = Instantiate(chunkPrefab);
        obj.SetActive(false);
        return obj;
    }
}
