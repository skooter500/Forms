
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class Mother : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxcreatures = 1;

        public float playerRadius = 1000;

        public List<GameObject> alive = new List<GameObject>();
        public Dictionary<GameObject, GameObject> aliveMap = new Dictionary<GameObject, GameObject>();
        public MultiDictionary<GameObject, GameObject> suspended = new MultiDictionary<GameObject, GameObject>();
        public GameObject[] prefabs;

        public LayerMask environmentLM;

        public static Mother Instance;

        public float genesisSpawnDistance = 100;

        School school;

        public bool spawnInFront = true;

        public float fov = 20;

        public GameObject player;

        bool FindPlace(GameObject creature, out Vector3 newPos)
        {
            bool found = false;
            int count = 0;
            newPos = Vector3.zero;
            while (!found)
            {
                SpawnParameters sp = creature.GetComponent<SpawnParameters>();

                if (sp == null)
                {
                    Debug.Log("Creature : " + creature + " doesnt have spawn parameters!!!");
                    found = false;
                    break;
                }

                float start = Mathf.Min(sp.start, genesisSpawnDistance);

                //Vector3 r = Vector3.forward; // Random.insideUnitSphere;
                //r.z = spawnInFront ? Mathf.Abs(r.z) : r.z;
                //r.y = 0;
                Vector3 r = Vector3.forward;
                r *= Random.Range(start, sp.end);
                //r += (r.normalized * start);
                r = Quaternion.AngleAxis(Random.Range(-fov, fov), Vector3.up) * r;

                newPos = player.transform.TransformPoint(r);
                float sampleY = WorldGenerator.Instance.SamplePos(newPos.x, newPos.z);
                float worldMax = WorldGenerator.Instance.surfaceHeight - sp.minDistanceFromSurface;
                float minHeight = sampleY + sp.minHeight;
                int segments = 3;
                if (sp.radiusRequired != 0)
                {
                    float[] heights = new float[segments + 1];
                    heights[0] = sampleY;
                    float sum = sampleY;
                    float thetaInc = (Mathf.PI * 2.0f) / segments;
                    for (int i = 0; i < segments; i++)
                    {
                        float theta = i * thetaInc;
                        Vector3 p = new Vector3
                            (Mathf.Sin(theta) * sp.radiusRequired
                            , 0
                            , Mathf.Cos(theta) * sp.radiusRequired
                            );

                        // Translate by newPos
                        p += newPos;

                        heights[i + 1] = WorldGenerator.Instance.SamplePos(p.x, p.z);
                    }
                    float stdDev = Utilities.StdDev(heights);
                    if (stdDev > 2)
                    {
                        count++;
                        continue;
                    }
                }
                if (minHeight > worldMax)
                {
                    count++;
                    continue;
                }
                if (count == 10)
                {
                    found = false;
                    break;
                }

                float maxHeight = Mathf.Min(sampleY + sp.maxHeight, worldMax);
                newPos.y = Mathf.Min(Random.Range(minHeight, maxHeight), worldMax);
                found = true;
            }
            return found;
        }

        private bool TestPos(Vector3 newPos, float spaceRequired)
        {
            // Testing visibility to the player
            float dist = Vector3.Distance(player.transform.position, newPos);
            RaycastHit rch;
            bool hit = Physics.Raycast(player.transform.position
                , newPos - player.transform.position
                , out rch
                , dist
                , environmentLM
            );

            if (!hit)
            {
                return false;
            }

            // Testing enough space
            hit = Physics.SphereCast(newPos, spaceRequired, Vector3.up, out rch, spaceRequired, environmentLM);
            if (hit)
            {
                return false;
            }

            return true;
        }

        IEnumerator SuspendCoRoutine(GameObject species, GameObject creature, bool fade)
        {
            if (fade && creature.activeInHierarchy)
            {
                if (creature.GetComponent<LifeColours>())
                {
                    creature.GetComponent<LifeColours>().FadeOut();
                }
                yield return new WaitForSeconds(2);
            }
            Boid[] boids = creature.GetComponentsInChildren<Boid>();
            foreach (Boid b in boids)
            {
                b.suspended = true;
            }
            SchoolGenerator[] sgs = creature.GetComponentsInChildren<SchoolGenerator>();
            foreach (SchoolGenerator sg in sgs)
            {
                sg.Suspend();
            }
            creature.SetActive(false);
            suspended.Add(species, creature);
            alive.Remove(creature);
            aliveMap.Remove(species);
            species.GetComponent<SpawnParameters>().isSuspending = false;
        }

        public void Suspend(GameObject species, GameObject creature, bool fadeOut = false)
        {
            SpawnParameters sp = species.GetComponent<SpawnParameters>();
            if (sp.isSuspending)
            {
                return;
            }
            else
            {
                sp.isSuspending = true;
                StartCoroutine(SuspendCoRoutine(species, creature, fadeOut));
            }
        }



        System.Collections.IEnumerator Spawn()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();

            int nextSpecies = 0;

            while (true)
            {

                // Remove too far creatures
                for (int i = alive.Count - 1; i >= 0; i--)
                {
                    GameObject creature = alive[i];
                    SpawnParameters sp = creature.GetComponent<SpawnParameters>();
                    GameObject species = sp.Species;
                    Vector3 boidPos = GetCreaturePosition(creature);

                    Vector3 camPos = player.transform.position;
                    float dist = Vector3.Distance(boidPos, camPos);
                    bool behind;
                    //bool behind = (Vector3.Dot(boidPos - camPos, player.transform.forward) < 0) && (dist > 500);
                    behind = Vector3.Angle(boidPos - camPos, player.transform.forward) > 80 && (dist > 2000);
                    if (sp.Species == PlayerController.Instance.species)
                    {
                        continue;
                    }

                    //Debug.Log(i + "\t" + dist);
                    if (behind)
                    {
                        Suspend(species, creature);                        
                    }
                }


                if (alive.Count < maxcreatures)
                {
                    // Find a spawn point
                    // Calculate the position
                    GetSpecies(nextSpecies, prefabs[nextSpecies].GetComponent<SpawnParameters>().singleton);
                    nextSpecies = (nextSpecies + 1) % prefabs.Length;
                }
                yield return new WaitForSeconds(delay);
            }
        }

        public GameObject GetSpecies(int speciesIndex, bool useExisting)
        {
            Vector3 newPos = Vector3.zero;
            GameObject newCreature = null;

            SpawnParameters sp = prefabs[speciesIndex].GetComponent<SpawnParameters>();
            
            if (useExisting && aliveMap.ContainsKey(prefabs[speciesIndex]))
            {
                GameObject creature = aliveMap[prefabs[speciesIndex]];
                if (!creature.GetComponent<SpawnParameters>().isSuspending)
                {
                    return prefabs[speciesIndex];
                }
            }
            if (suspended.ContainsKey(prefabs[speciesIndex]))
            {
                newCreature = suspended.Get(prefabs[speciesIndex]);
                if (FindPlace(newCreature, out newPos))
                {
                    suspended.Remove(prefabs[speciesIndex], newCreature);
                    Teleport(newCreature, newPos);
                    newCreature.SetActive(true);
                    if (newCreature.GetComponent<LifeColours>())
                    {
                        newCreature.GetComponent<LifeColours>().FadeIn();
                    }
                    if (newCreature.GetComponentInChildren<CreatureController>())
                    {
                        newCreature.GetComponentInChildren<CreatureController>().Restart();
                    }
                    // Change the school size every time we teleport a school
                    SchoolGenerator sg = newCreature.GetComponentInChildren<SchoolGenerator>();
                    if (sg != null)
                    {
                        sg.transform.position = newPos;
                        sg.targetCreatureCount = Random.Range(sg.minBoidCount, sg.maxBoidCount);
                    }
                    alive.Add(newCreature);
                    aliveMap[prefabs[speciesIndex]] = newCreature;
                    /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = newPos;
                    cube.transform.localScale = Vector3.one * 5;
                    */
                }
            }
            else
            {
                //Debug.Log("Instiantiating a new: " + prefabs[nextCreature]);
                if (FindPlace(prefabs[speciesIndex], out newPos))
                {
                    newCreature = GameObject.Instantiate<GameObject>(prefabs[speciesIndex], newPos
                        , prefabs[speciesIndex].transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                    );

                    newCreature.GetComponent<SpawnParameters>().Species = prefabs[speciesIndex];
                    if (school != null)
                    {
                        Boid b = newCreature.GetComponent<Boid>();
                        b.school = school;
                        school.boids.Add(b);
                    }

                    if (newCreature.GetComponentInChildren<CreatureController>())
                    {

                        newCreature.GetComponentInChildren<CreatureController>().mother = this;
                    }

                    newCreature.transform.parent = this.transform;
                    newCreature.transform.position = newPos;
                    newCreature.SetActive(true);
                    alive.Add(newCreature);
                    aliveMap[prefabs[speciesIndex]] = newCreature;
                }
                else
                {
                    Debug.Log("Couldnt find a place for the new creature");
                }
            }
            return prefabs[speciesIndex];
        }

        public GameObject GetCreature(GameObject species)
        {
            GameObject speciesInstance = aliveMap[species];
            SchoolGenerator sg = speciesInstance.GetComponent<SchoolGenerator>();
            if (speciesInstance.GetComponent<TenticleCreatureGenerator>() != null)
            {
                return speciesInstance.GetComponent<TenticleCreatureGenerator>().head.gameObject;
            }
            else if (sg != null)
            {
                if (sg.alive.Count == 0)
                {
                    Debug.Log("school 0");
                    return null;
                }
                return sg.alive[UnityEngine.Random.Range(0, sg.alive.Count)].GetComponentInChildren<Boid>().gameObject;
            }
            else if (speciesInstance.GetComponent<FormationGenerator>() != null)
            {
                return speciesInstance.GetComponent<FormationGenerator>().leader.GetComponentInChildren<Boid>().gameObject;
            }
            else
            {
                return Utilities.FindBoidInHierarchy(speciesInstance).gameObject;
            }
        }

        public Vector3 GetCreaturePosition(GameObject creature)
        {
            //return creature.transform.position;
            if (creature.GetComponent<TenticleCreatureGenerator>() != null)
            {
                return creature.GetComponent<TenticleCreatureGenerator>().head.GetComponent<Boid>().position;
            }
            else if (creature.GetComponent<SchoolGenerator>() != null)
            {
                return creature.transform.position;
            }
            else if (creature.GetComponent<FormationGenerator>() != null)
            {
                return creature.GetComponent<FormationGenerator>().leader.GetComponentInChildren<Boid>().position;
            }
            else if (creature.GetComponent<SandWorm>() != null)
            {
                return creature.transform.GetChild(0).transform.position;
            }
            else
            {
                return creature.GetComponentInChildren<Boid>().transform.position;
            }
        }

        private void Teleport(GameObject creature, Vector3 newPos)
        {
            Vector3 boidPos = GetCreaturePosition(creature);
            if (creature.GetComponent<SchoolGenerator>() == null && creature.GetComponent<SandWorm>() == null)
            {

                Vector3 trans = newPos - boidPos;
                creature.transform.position += trans;
                Boid boid = Utilities.FindBoidInHierarchy(creature);
                // Translate it to the new position                            
                //boid.transform.position = newPos;
                boid.position = boid.transform.position; // The boid
                boid.desiredPosition = boid.position;
                boid.suspended = false;
                if (boid.GetComponent<Constrain>() != null)
                {
                    boid.GetComponent<Constrain>().centre = newPos;
                }

                if (boid.GetComponent<TrailRenderer>() != null)
                {
                    boid.GetComponent<TrailRenderer>().Clear();
                }
                FormationGenerator fg = creature.GetComponent<FormationGenerator>();
                if (fg != null)
                {
                    fg.GeneratePositions();
                    fg.Teleport();
                }
            }
            else
            {
                creature.transform.position = newPos;
            }
        }


        private void AfterGenesis()
        {
            genesisSpawnDistance = 100000;
        }

        private void Awake()
        {
            Instance = this;
            if (Application.isEditor)
            {
                //maxcreatures = 10;
            }
            Invoke("AfterGenesis", 10);
        }

        // Use this for initialization
        void Start()
        {
            school = GetComponent<School>();

            StartCoroutine(Spawn());
        }

        // Update is called once per frame
        void Update()
        {
            CreatureManager.Log("Alive species: " + alive.Count);
            CreatureManager.Log("Suspended species: " + suspended.Count);
        }
    }
}