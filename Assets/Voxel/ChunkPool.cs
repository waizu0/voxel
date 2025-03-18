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
    public void ReleaseChunkObject(GameObject chunkObj)
    {
        chunkObj.SetActive(false);
        pool.Enqueue(chunkObj);
    }

    private GameObject CreateNewChunkObject()
    {
        GameObject obj = Instantiate(chunkPrefab);
        obj.SetActive(false);
        return obj;
    }
}
