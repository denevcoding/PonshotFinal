using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn;

    [SerializeField]
    private Queue<GameObject> pool = new Queue<GameObject>();

    [SerializeField]
    private int poolStartSize = 5;

    // Start is called before the first frame update
    void Start()
    {
        InitPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void InitPool()
    {
        for (int i = 0; i < poolStartSize; i++)
        {
            GameObject prefab = Instantiate(prefabToSpawn);
            pool.Enqueue(prefab);
            prefab.SetActive(false);
        }
    }

    public GameObject GetPrefab()
    {
        if (pool.Count > 0)
        {
            GameObject prefab = pool.Dequeue();
            prefab.SetActive(true);
            return prefab;
        }
        else
        {
            GameObject prefab = Instantiate(prefabToSpawn);
            return prefab;
        }
    }

    public void ReturnPrefab(GameObject prefab)
    {
        pool.Enqueue(prefab);
        prefab.SetActive(false);

    }
}
