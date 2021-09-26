using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private Dictionary<int, Queue<GameObject>> poolDicitionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;

    private void Start()
    {
        // Create object pools on start
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        // Create parentGameObject to parent the child objects to
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if(!poolDicitionary.ContainsKey(poolKey))
        {
            poolDicitionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);

                poolDicitionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if(poolDicitionary.ContainsKey(poolKey))
        {
            // Get object from pool queue
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab);

            return objectToReuse;
        }
        else
        {
            Debug.LogWarning("No object pool for " + prefab);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDicitionary[poolKey].Dequeue();
        poolDicitionary[poolKey].Enqueue(objectToReuse);

        // Log to console if object is currently active
        if(objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}
