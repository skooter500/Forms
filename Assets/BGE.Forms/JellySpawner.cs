using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class JellySpawner : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxJellys = 20;

        public float playerRadius = 1000;

        public static List<GameObject> alive = new List<GameObject>();
        public static List<GameObject> dead = new List<GameObject>();

        public GameObject jellyPrefab;

        public LayerMask environmentLM;

        System.Collections.IEnumerator SpawnJellys()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = GameObject.FindObjectOfType<WorldGenerator>();
            while (true)
            {
                // Remove too far jellys
                for (int i = alive.Count -1; i >= 0; i --)
                {
                    GameObject jelly = alive[i];
                    Transform cam = Camera.main.transform;
                    Boid boid = Utilities.FindBoidInHierarchy(jelly);
                    // If the jelly is behind the player, divide the distance
                    float deadDistance = (Vector3.Dot(boid.position - cam.position, cam.forward) > 0)
                        ? playerRadius
                        : playerRadius / 4;
                    if (Vector3.Distance(boid.position, Camera.main.transform.position) > deadDistance)
                    {
                        dead.Add(jelly);
                        alive.Remove(jelly);
                    }
                }

                if (alive.Count < maxJellys)
                {
                    // Find a spawn point
                    // Calculate the position
                    bool found = false;
                    int count = 0;
                    Vector3 newPos = Vector3.zero;
                    while (!found)
                    {
                        Vector2 r = Random.insideUnitCircle;
                        newPos = Camera.main.transform.position
                            + new Vector3
                            (r.x * playerRadius
                            , 0
                            , Mathf.Abs(r.y) * playerRadius);
                        newPos.y = wg.SamplePos(newPos.x, newPos.z) + Random.Range(10, 50);
                        found = true;
                        /*
                        float dist = Vector3.Distance(Camera.main.transform.position, newPos);
                        RaycastHit rch;
                        bool hit = Physics.Raycast(Camera.main.transform.position
                            , newPos - Camera.main.transform.position
                            , out rch
                            , dist
                            , environmentLM
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
                        */
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
                            newJelly.SetActive(true);
							newJelly.transform.parent = this.transform.parent;
                        }
                        newJelly.transform.position = newPos;
                        Utilities.FindBoidInHierarchy(newJelly).desiredPosition = newPos;
                        alive.Add(newJelly);
                    }
                    else
                    {
                        //Debug.Log("Couldnt find a place to spawn the jelly");
                    }
                }
                yield return new WaitForSeconds(delay);
            }            
        }

        // Use this for initialization
        void OnEnable()
        {
            StartCoroutine(SpawnJellys());
        }

        void Awake()
        {
            alive.Clear();
            dead.Clear();
        }

        // Update is called once per frame
        void Update()
        {
            CreatureManager.Log("Num jellys: " + alive.Count);
        }
    }
}