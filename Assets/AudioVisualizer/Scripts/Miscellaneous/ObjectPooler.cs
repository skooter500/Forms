using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace AudioVisualizer
{
    /// <summary>
    /// Loads objects into the scene, and makes them available for grabbing using ObjectPooler.isntance.GetObject(string name)
    /// This increases performance by allowing the dev to enable/disable objects, instead of instantiating them.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {

        public static ObjectPooler instance;
        public GameObject LoadingCanvas;
        public Image loadBar;
        public List<ObjectToPool> ObjectsToPool; // here the user can input a lot of different gameobjects, each one will be pooled
        public Dictionary<string, List<GameObject>> objectPools; // key = name of GameObject, Value is a the pool of objects
        public bool hideInHeirarchy;
        public UnityEvent OnLoadComplete;


        void Awake()
        {
            instance = this;
            objectPools = new Dictionary<string, List<GameObject>>();

        }

        // Use this for initialization
        void Start()
        {

            StartCoroutine(LoadObjects());
            //Debug.Log ("Keys" + objectPools.Keys);
        }

        /* Instantiate all the obejcts*/
        IEnumerator LoadObjects()
        {
            //Debug.Log("Loading Objects");

            if (LoadingCanvas != null)
            {
                LoadingCanvas.SetActive(true);
                loadBar.fillAmount = 0.0f;
            }

            //queue up all the objects for creation
            Queue<ObjectToPool> objectQueue = new Queue<ObjectToPool>();
            foreach (ObjectToPool obj in ObjectsToPool)
            {
                if (obj.name == "")
                    continue;
                for (int j = 0; j < obj.poolSize; j++)
                {
                    objectQueue.Enqueue(obj);
                }
            }

            int totalObjects = objectQueue.Count;
            string lastName = ""; // the string of the last unique object we loaded
            GameObject folder = null; //an empty gameObject that acts as a folder
            List<GameObject> objectList = null;
            while (objectQueue.Count > 0)
            {
                float percentDone = (float)(totalObjects - objectQueue.Count) / (totalObjects - 1);
                if (loadBar != null)
                {
                    loadBar.fillAmount = percentDone;
                }

                //Debug.Log((percentDone * 100) + "%");

                ObjectToPool pooledObject = objectQueue.Dequeue();
                if (pooledObject.name != lastName)
                {
                    folder = new GameObject(pooledObject.name);
                    folder.transform.SetParent(this.transform);

                    if (objectList != null)
                    {
                        //store the list of objects in the dictionary
                        objectPools.Add(lastName, objectList);
                    }
                    objectList = new List<GameObject>();
                    lastName = pooledObject.name;
                }

                GameObject obj = Instantiate(pooledObject.prefab, this.transform.position, this.transform.rotation) as GameObject;
                obj.SetActive(false);
                obj.transform.parent = folder.transform;
                //add this new object into the dicitonary list
                objectList.Add(obj);

                yield return new WaitForFixedUpdate();
            }

            //add the final objectList to the dictionary
            objectPools.Add(lastName, objectList);

            if (OnLoadComplete != null)
            {
                OnLoadComplete.Invoke();
            }
            if (LoadingCanvas != null)
            {
                LoadingCanvas.SetActive(false);
            }


        }

        /* grab an object out of the pool, create a new object if neccesary */
        public GameObject GetObject(string name)
        {
            if (!objectPools.ContainsKey(name))
            {
                //Debug.Log("Key: " + name + " doesn't exist in the ObjectPooler dictionary");
                return null;
            }
            List<GameObject> objectPool = objectPools[name]; //get the object pool for this object out of the dictionary
                                                             //return the first inactive object
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (!objectPool[i].activeInHierarchy)
                {
                    return objectPool[i];
                }
            }

            //Debug.Log("Out of pooled objects");

            //if we reach this code, the object pool is empty, or all objects in the pool are active
            //grow if we've said we can grow
            foreach (ObjectToPool obj in ObjectsToPool) // go through our list of objects
            {
                //find the object with the same name
                if (obj.name == name)
                {
                    //see if it can grow
                    if (obj.canGrow)
                    {
                        //Debug.Log("can grow");
                        GameObject newObject = (GameObject)Instantiate(obj.prefab); // instantiate the gameobject
                        if (hideInHeirarchy)
                        {
                            newObject.hideFlags = HideFlags.HideInHierarchy;
                        }
                        objectPool.Add(newObject);//add it to the list
                        objectPools[name] = objectPool;//since the list has changed, store it in the dictionary
                        newObject.SetActive(false);
                        return newObject;
                    }
                    break;
                }
            }

            return null;
        }

    }

    [System.Serializable]
    public class ObjectToPool
    {
        public string name; // not necessary but nice for inspectors
        public GameObject prefab;
        public bool instantiatePool = true;
        public int poolSize;
        public bool canGrow = true;
        //public bool canShrink? shouldn't be necessary


    }
}

