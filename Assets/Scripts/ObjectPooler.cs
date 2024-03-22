using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        [Tooltip("The name by which we will pull out the object")]
        public string tag;
        [Tooltip("Object prefab")]
        public GameObject prefab;
        [Tooltip("Size of the pool")]
        public int poolSize;
    }

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<Pool> pools;

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //initialize pools and create them as child objects
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            GameObject poolRoot = new GameObject();
            poolRoot.name = pool.tag;
            poolRoot.transform.SetParent(transform);
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolRoot.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        //display a warning if we specified the wrong tag
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag" + tag + " doesnt exist");
            return null;
        }

        //set object's position and release it from the pool
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(false);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        //putting the object at the end of the queue
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}
