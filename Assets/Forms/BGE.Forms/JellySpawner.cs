using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class JellySpawner : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxJellys = 20;

        public float playerRadius = 2000;

        public static List<GameObject> alive = new List<GameObject>();
        public static List<GameObject> dead = new List<GameObject>();

        public GameObject jellyPrefab;

        System.Collections.IEnumerator SpawnJellys()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = GetComponent<WorldGenerator>();
            while (true)
            {
                if (alive.Count < maxJellys)
                {
                    // Find a spawn point
                    // Calculate the position
                    bool found = false;
                    int count = 0;
                    while (!found)
                    {
                        Vector2 r = Random.insideUnitCircle;
                        Vector3 newPos = Camera.main.transform.position
                            + new Vector3
                            (r.x * playerRadius
                            , 0
                            , r.y * playerRadius);
                        newPos.y = wg.SamplePos(newPos.x, newPos.y) + 20;
                        float dist = Vector3.Distance(Camera.main.transform.position, newPos);
                        RaycastHit rch;
                        bool hit = Physics.Raycast(Camera.main.transform.position
                            , newPos - Camera.main.transform.position
                            , out rch
                            , dist
                            , LayerMask.NameToLayer("Environment")
                            );

                        if (hit)
                        {
                            found = true;
                            break;
                        }
                        count++;
                        if (count == 10)
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        GameObject newJelly = null;
                        if (dead.Count > 0)
                        {
                            newJelly = dead[dead.Count - 1];
                            dead.Remove(newJelly);
                            newJelly.transform.GetChild(0).localPosition = Vector3.zero;
                        }
                        else
                        {
                            newJelly = GameObject.Instantiate<GameObject>(jellyPrefab);
                        }
                        alive.Add(newJelly);
                    }                    
                }
                yield return new WaitForSeconds(delay);
            }            
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}