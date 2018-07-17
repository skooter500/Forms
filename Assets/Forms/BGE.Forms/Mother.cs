using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class Mother : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxcreatures = 20;

        public float playerRadius = 1000;

        public List<GameObject> alive = new List<GameObject>();
        private Dictionary<GameObject, GameObject> aliveMap = new Dictionary<GameObject, GameObject>();
        public MultiDictionary<GameObject, GameObject> suspended = new MultiDictionary<GameObject, GameObject>();
        public GameObject[] prefabs;

        public LayerMask environmentLM;

        public static Mother Instance;

        public float genesisSpawnDistance = 100;

        School school;

        public bool spawnInFront = true;

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
              
                Vector3 r = Random.insideUnitSphere;
				r.z = spawnInFront ? Mathf.Abs(r.z) : r.z;
                r.y = 0;
                r *= sp.end - start;
                r += (r.normalized * start);

                newPos = Camera.main.transform.TransformPoint(r);
                float sampleY = WorldGenerator.Instance.SamplePos(newPos.x, newPos.z);
                float worldMax = WorldGenerator.Instance.surfaceHeight - sp.minDistanceFromSurface;
                float minHeight  =  sampleY + sp.minHeight;                
                if (minHeight > worldMax)
                {                    
                    count++;
                    if (count == 10)
                    {
                        found = false;
                        break;
                    }
                    continue;
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
            float dist = Vector3.Distance(Camera.main.transform.position, newPos);
            RaycastHit rch;
            bool hit = Physics.Raycast(Camera.main.transform.position
                , newPos - Camera.main.transform.position
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

        private void Suspend(GameObject creature)
        {
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
        }

        System.Collections.IEnumerator Spawn()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();

            int nextCreature = 0;

            while (true)
            {
                
                // Remove too far creatures
                for (int i = alive.Count - 1; i >= 0; i--)
                {
                    GameObject creature = alive[i];
                    SpawnParameters sp = creature.GetComponent<SpawnParameters>();
                    GameObject species = sp.Species;
                    Vector3 boidPos = GetCreaturePosition(creature);
                    

                    float dist = Vector3.Distance(boidPos, Camera.main.transform.position);
                    //Debug.Log(i + "\t" + dist);
                    if (dist > sp.end)
                    {
                        Debug.Log("Suspending a creature: " + creature);
                        Suspend(creature);
                        suspended.Add(species, creature);
                        alive.Remove(creature);
                        aliveMap.Remove(species);
                    }
                }


                if (alive.Count < maxcreatures)
                {
                    // Find a spawn point
                    // Calculate the position
                    Vector3 newPos = Vector3.zero;
                    GameObject newcreature = null;

                    SpawnParameters sp = prefabs[nextCreature].GetComponent<SpawnParameters>();
                    if (sp.singleton && aliveMap.ContainsKey(prefabs[nextCreature]))
                    {
                        nextCreature = (nextCreature + 1) % prefabs.Length;
                        continue;
                    }
                    if (suspended.ContainsKey(prefabs[nextCreature]))
                    {
                        newcreature = suspended.Get(prefabs[nextCreature]);
                        if (FindPlace(newcreature, out newPos))
                        {
                            suspended.Remove(prefabs[nextCreature], newcreature);                            
                            Teleport(newcreature, newPos);
                            newcreature.SetActive(true);
                            if (newcreature.GetComponent<LifeColours>())
                            {
                                newcreature.GetComponent<LifeColours>().FadeIn();
                            }
                            // Change the school size every time we teleport a school
                            SchoolGenerator sg = newcreature.GetComponentInChildren<SchoolGenerator>();
                            if (sg != null)
                            {
                                sg.transform.position = newPos;
                                sg.targetCreatureCount = Random.Range(sg.minBoidCount, sg.maxBoidCount);
                            }
                            alive.Add(newcreature);
                            aliveMap[prefabs[nextCreature]] = newcreature;
                            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = newPos;
                            cube.transform.localScale = Vector3.one * 5;
                            */
                        }
                    }
                    else
                    {
                        Debug.Log("Instiantiating a new: " + prefabs[nextCreature]);
                        if (FindPlace(prefabs[nextCreature], out newPos))
                        {
                            newcreature = GameObject.Instantiate<GameObject>(prefabs[nextCreature], newPos
                                , prefabs[nextCreature].transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                            );
                            
                            newcreature.GetComponent<SpawnParameters>().Species = prefabs[nextCreature];
                            if (school != null)
                            {
                                Boid b = newcreature.GetComponent<Boid>();
                                b.school = school;
                                school.boids.Add(b);
                            }
                            newcreature.transform.parent = this.transform;
                            newcreature.transform.position = newPos;
                            newcreature.SetActive(true);
                            alive.Add(newcreature);
                        }
                        else
                        {
                            Debug.Log("Couldnt find a place for the new creature");
                        }                        
                    }
                    nextCreature = (nextCreature + 1) % prefabs.Length;
                }
                yield return new WaitForSeconds(delay);
            }
        }

        public GameObject GetCreature(GameObject species)
        {
            SchoolGenerator sg = species.GetComponent<SchoolGenerator>();
            if (species.GetComponent<TenticleCreatureGenerator>() != null)
            {
                return species.GetComponent<TenticleCreatureGenerator>().head.gameObject;
            }
            else if (sg != null)
            {
                if (sg.alive.Count == 0)
                {
                    Debug.Log("school 0");
                }
                return sg.alive[Random.Range(0, sg.alive.Count - 1)].GetComponentInChildren<Boid>().gameObject;
            }
            else if (species.GetComponent<FormationGenerator>() != null)
            {
                return species.GetComponent<FormationGenerator>().leader.GetComponentInChildren<Boid>().gameObject;
            }
            else
            {
                return Utilities.FindBoidInHierarchy(species).gameObject;
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
            else
            {
                return creature.GetComponentInChildren<Boid>().position;

            }
        }

        private void Teleport(GameObject creature, Vector3 newPos)
        {
            Vector3 boidPos = GetCreaturePosition(creature);
            if (creature.GetComponent<SchoolGenerator>() == null)
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

                if (creature.GetComponentInChildren<CreatureController>())
                {
                    creature.GetComponentInChildren<CreatureController>().Restart();
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