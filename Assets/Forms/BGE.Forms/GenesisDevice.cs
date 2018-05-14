using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class GenesisDevice : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxPlants = 20;

        public float playerRadius = 1000;
        public float closeness = 1000;

        public static List<GameObject> alive = new List<GameObject>();
        public static List<GameObject> dead = new List<GameObject>();

        public GameObject[] prefabs;
        int nextPlant = 0;
        public GameObject plantPrefab;

        public LayerMask environmentLM;

        System.Collections.IEnumerator SpawnPlants()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            while (true)
            {
                // Remove too far plants
                for (int i = alive.Count -1; i >= 0; i --)
                {
                    GameObject plant = alive[i];
                    Transform cam = Camera.main.transform;
                    // If the jelly is behind the player, divide the distance
                    float deadDistance = (Vector3.Dot(plant.transform.position - cam.position, cam.forward) > 0)
                        ? playerRadius
                        : playerRadius / 4;
                    if (Vector3.Distance(plant.transform.position, Camera.main.transform.position) > deadDistance)
                    {
                        dead.Add(plant);
                        alive.Remove(plant);
                    }
                }

                if (alive.Count < maxPlants)
                {
                    // Find a spawn point
                    // Calculate the position
                    bool found = false;
                    int count = 0;
                    Vector3 newPos = Vector3.zero;
                    while (!found)
                    {
                        float start = 1500;

                        Vector3 r = Random.insideUnitSphere;
                        //r.z = Mathf.Abs(r.z);
                        r.y = 0;
                        r *= playerRadius - start;
                        r += (r.normalized * start);

                        newPos = Camera.main.transform.TransformPoint(r);
                        newPos.y = wg.SamplePos(newPos.x, newPos.z);
                        bool tooClose = false;
                        foreach (GameObject tree in alive)
                        {
                            if (Vector3.Distance(tree.transform.position, newPos) < closeness)
                            {
                                tooClose = true;
                                count++;
                                if (count == 10)
                                {
                                    found = false;
                                }
                                break;
                            }
                        }
                        if (!tooClose)
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        GameObject newPlant = null;
                        if (dead.Count > 0)
                        {
                            newPlant = dead[dead.Count - 1];
                            dead.Remove(newPlant);
                        }
                        else
                        {
                            newPlant = GameObject.Instantiate<GameObject>(prefabs[nextPlant]);
                            nextPlant = (nextPlant + 1) % prefabs.Length;
                            newPlant.SetActive(true);
							newPlant.transform.parent = this.transform.parent;
                        }
                        float r = 20;
                        newPlant.transform.rotation = Quaternion.Euler(
                            Random.Range(-r, r)
                            , Random.Range(0, 360)
                            , Random.Range(-r, r)
                            );

                        float size = Random.Range(0.7f, 1.0f);
                        newPlant.transform.localScale = new Vector3(size, size, size);
                        //newPos.y += size * 200;
                        newPlant.transform.position = newPos;

                        alive.Add(newPlant);

                        if (newPlant.GetComponent<LifeColours>())
                        {
                            newPlant.GetComponent<LifeColours>().FadeIn();
                        }
                    }
                    else
                    {
                        Debug.Log("Couldnt find a place to spawn the plant");
                    }
                }
                yield return new WaitForSeconds(delay);
            }            
        }

        // Use this for initialization
        void Start()
        {
            StartCoroutine(SpawnPlants());
        }

        // Update is called once per frame
        void Update()
        {
            CreatureManager.Log("Num plants: " + alive.Count);
        }
    }
}