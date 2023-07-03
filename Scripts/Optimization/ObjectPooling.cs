using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Optimization
{
    public class ObjectPooling : MonoBehaviour
    {
        [System.Serializable]
        public struct Pool
        {
            public string tag;
            public int objectsQuantity;
            public GameObject objectPrefab;
            public Pool(string tag, GameObject objectPrefab, int objectsQuantity)
            {
                this.tag = tag;
                this.objectPrefab = objectPrefab;
                this.objectsQuantity = objectsQuantity;
            }
        }
        public List<Pool> pools;
        public static ObjectPooling Instance;
        private void Awake()
        {
            Instance = this;
        }
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (Pool pool in pools)
            {
                CreateQueue(pool);
            }
        }
        public void SetActiveAllPoolObjects(string tag, bool condition)
        {
            if (CheckTag(tag))
            {
                for (int i = 0; i < poolDictionary[tag].Count; i++)
                {
                    GameObject spawnedObject = poolDictionary[tag].Dequeue();
                    if (spawnedObject.activeSelf != condition)
                    {
                        spawnedObject.SetActive(condition);
                    }
                    poolDictionary[tag].Enqueue(spawnedObject);
                }
            }
        }
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Count != 0)
            {
                GameObject spawnedObject = poolDictionary[tag].Dequeue();
                spawnedObject.SetActive(true);
                spawnedObject.transform.position = position;
                spawnedObject.transform.rotation = rotation;
                IPooledObject pooledObject = spawnedObject.GetComponent<IPooledObject>();
                if (pooledObject != null)
                {
                    pooledObject.OnSpawnedFromPool();
                }
                poolDictionary[tag].Enqueue(spawnedObject);
                return spawnedObject;
            }
            else
            {
                Debug.LogWarning("Pool With This Tag :" + tag + " Can Not Be Found Or Queue Is Empty");
                return null;
            }
        }
        public void AddNewPool(string tag, GameObject objectPrefab, int objectsQuantity)
        {
            Pool newPool = new Pool(tag, objectPrefab, objectsQuantity);
            CreateQueue(newPool);
        }
        public void FillPoolWithNewObjects(string tag, int additionalObjectsQuantity)
        {
            GameObject objectPrefab = poolDictionary[tag].Dequeue();
            Queue<GameObject> currentQueue = poolDictionary[tag];
            for (int i = 0; i < additionalObjectsQuantity; i++)
            {
                GameObject instantiatedObject = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
                currentQueue.Enqueue(instantiatedObject);
            }

        }
        public void ReplaceObjectsOfPool(string tag, GameObject newPrefab)
        {
            Queue<GameObject> queueToReplace = poolDictionary[tag];
            Pool pool = new Pool(tag, newPrefab, queueToReplace.Count);
            poolDictionary.Remove(tag);
            foreach (GameObject gameObjectToDestroy in queueToReplace)
            {
                Destroy(gameObjectToDestroy);
            }
            Queue<GameObject> newQueue = CreateQueue(pool);
        }
        private bool CheckTag(string tag)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.Log("Pool With Such Tag Does Not Exist. Tag = " + tag);
                return false;
            }
            return true;
        }
        private Queue<GameObject> CreateQueue(Pool pool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.objectsQuantity; i++)
            {
                GameObject instantiatedPrefab = Instantiate(pool.objectPrefab, Vector3.zero, Quaternion.identity);
                instantiatedPrefab.name = instantiatedPrefab.name + i;
                instantiatedPrefab.SetActive(false);
                objectPool.Enqueue(instantiatedPrefab);
            }
            poolDictionary.Add(pool.tag, objectPool);
            return objectPool;
        }
    }
}