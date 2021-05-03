using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            GameObject parent = new GameObject(pool.tag + "s");

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = parent.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool AddBackToPool)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist!");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
            pooledObject.OnObjectSpawn();

        if(AddBackToPool)
            poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void ReturnObjectToPool(string tag, GameObject objToRemove)
    {
        poolDictionary[tag].Enqueue(objToRemove);
        objToRemove.SetActive(false);
    }
}
