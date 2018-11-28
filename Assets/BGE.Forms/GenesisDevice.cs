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
        
        public float radius = 5; // width of the square around the player
        public float gap = 200; // gap between nodes

        public Dictionary<string, GameObject> alive = new Dictionary<string, GameObject>();
        public List<GameObject> dead = new List<GameObject>();

        public GameObject[] prefabs;
        int nextPlant = 0;
        public float threshold = 0.6f;
        public GameObject plantPrefab;

        public LayerMask environmentLM;

        public GameObject player;

        public enum Positioning { Ground, Air }

        public Positioning positioning = Positioning.Ground;

        public void OnDrawGizmos()
        {
            Vector3 center = player.transform.position / gap;
            Sampler s = GetComponent<Sampler>();
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            center.x = (Mathf.Round(center.x) * gap);
            center.z = (Mathf.Round(center.z) * gap);
            Vector3 bottomLeft = center - new Vector3(gap, 0, gap) * radius;
            for (int row = 0; row <= radius * 2; row++)
            {
                for (int col = 0; col <= radius * 2; col++)
                {
                    Vector3 pos = bottomLeft + (new Vector3(col, 0, row) * gap);
                    float sample = s.Sample(pos.x, pos.z);
                    if (sample >= threshold)
                    {
                        float height = wg.SamplePos(pos.x, pos.z);
                        pos.y = height;
                        Gizmos.color = Color.green;
                        //Gizmos.DrawLine(pos, pos + Vector3.up * 200 + (Vector3.right * 30));
                        Gizmos.DrawSphere(pos, 100);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        //Gizmos.DrawLine(pos, pos + Vector3.up * 200 + (Vector3.right * 30));
                        Gizmos.DrawSphere(pos, 100);
                    }
                }
            }
        }

        System.Collections.IEnumerator SpawnPlantsNoise()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            float maxDist = (radius + 1) * gap;
            yield return new WaitForSeconds(2.0f);
            while (true)
            {
                Vector3 center = player.transform.position / gap;
                Sampler s = GetComponent<Sampler>();
                center.x = (Mathf.Round(center.x) * gap);
                center.y = (Mathf.Round(center.y) * gap);
                center.z = (Mathf.Round(center.z) * gap);
                Vector3 bottomLeft = center - new Vector3(gap, 0, gap) * radius;
                Vector3 topRight = center + new Vector3(gap, 0, gap) * radius;

                List<string> keys = new List<string>();
                foreach (string key in alive.Keys)
                {
                    keys.Add(key);
                }
                foreach(string key in keys)
                {
                    GameObject tree = alive[key];
                    Vector3 tp = tree.transform.position;
                    if (tp.x < bottomLeft.x || tp.x > topRight.x || tp.z < bottomLeft.z || tp.z > topRight.z)
                    {
                        dead.Add(tree);
                        tree.SetActive(false);
                        alive.Remove(key);
                    }                    
                }
                
                for (int row = 0; row <= radius * 2; row++)
                {
                    for (int col = 0; col <= radius * 2; col++)
                    {
                        Vector3 pos = bottomLeft + (new Vector3(col, 0, row) * gap);
                        pos.y = 0;
                        float sample = s.Sample(pos.x, pos.z);
                        if (sample > threshold)
                        {
                            RaycastHit rch;
                            float height = wg.SamplePos(pos.x, pos.z);
                            if (Physics.Raycast(new Vector3(pos.x, height + 1000, pos.z), Vector3.down, out rch, 10000, environmentLM))
                            {
                                height = rch.point.y;
                            }
                            if (positioning == Positioning.Ground)
                            {
                                pos.y = height;
                            }
                            else
                            {
                                //float air = ((wg.surfaceHeight + wg.transform.position.y) - height);
                                pos.y =  height + 500;
                            }                            

                            if (!alive.ContainsKey("" + pos))
                            {
                                GameObject newPlant = null;

                                if (dead.Count > 0)
                                {
                                    newPlant = dead[0];
                                    dead.RemoveAt(0);
                                }
                                else
                                {
                                    newPlant = GameObject.Instantiate<GameObject>(prefabs[nextPlant]);
                                    newPlant.transform.parent = this.transform;
                                    nextPlant = (nextPlant + 1) % prefabs.Length;
                                }
                                newPlant.SetActive(true);
                                newPlant.transform.parent = this.transform;
                                float r = 20;
                                float angle = Vector3.Angle(Vector3.up, rch.normal);
                                Vector3 axis = Vector3.Cross(Vector3.up, rch.normal);
                                Quaternion q = Quaternion.AngleAxis(angle, axis);

                                newPlant.transform.rotation = q;
                                /*Quaternion.Euler(
                                    Random.Range(-r, r)
                                    , Random.Range(0, 360)
                                    , Random.Range(-r, r)
                                    );
                                    */
                                float size = Random.Range(0.7f, 1.0f);
                                newPlant.transform.localScale = new Vector3(size, size, size);
                                //newPos.y += size * 200;
                                newPlant.transform.position = pos;
                                alive["" + pos] = newPlant;
                                if (newPlant.GetComponent<LifeColours>())
                                {
                                    newPlant.GetComponent<LifeColours>().FadeIn();
                                }
                                yield return new WaitForSeconds(delay);
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        /*
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
        */

        // Use this for initialization
        void Start()
        {
            StartCoroutine(SpawnPlantsNoise());
        }

        // Update is called once per frame
        void Update()
        {
            CreatureManager.Log("Num plants: " + alive.Count);
        }
    }
}