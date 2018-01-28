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
        public List<GameObject> suspended = new List<GameObject>();

        public GameObject[] prefabs;

        public LayerMask environmentLM;

        public static Mother Instance;

        bool FindPlace(GameObject creature, out Vector3 newPos)
        {
            bool found = false;
            int count = 0;
            newPos = Vector3.zero;
            while (!found)
            {
                SpawnParameters sp = creature.GetComponent<SpawnParameters>();
                Vector3 r = Random.insideUnitSphere;
                r.z = Mathf.Abs(r.z);
                r.y = 0;
                r *= sp.end - sp.start;
                r += (r.normalized * sp.start);

                newPos = Camera.main.transform.TransformPoint(r);
                newPos.y = WorldGenerator.Instance.SamplePos(newPos.x, newPos.z) + Random.Range(sp.minHeight, sp.maxHeight);
                if (newPos.y > WorldGenerator.Instance.surfaceHeight - sp.minDistanceFromSurface)
                {
                    count++;
                    if (count == 10)
                    {
                        found = false;
                        break;
                    }
                    continue;
                }
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
            suspended.Add(creature);
            Debug.Log("Suspending a creature");
            alive.Remove(creature);
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
                    Vector3 boidPos = GetCreaturePosition(creature);


                    float dist = Vector3.Distance(boidPos, Camera.main.transform.position);
                    //Debug.Log(i + "\t" + dist);
                    if (dist > playerRadius)
                    {
                        Suspend(creature);
                    }
                }

                if (alive.Count < maxcreatures)
                {
                    // Find a spawn point
                    // Calculate the position
                    Vector3 newPos = Vector3.zero;

                    GameObject newcreature = null;
                    if (suspended.Count > 0)
                    {
                        Debug.Log("Teleporting an old creature");
                        newcreature = suspended[suspended.Count - 1];
                        if (FindPlace(newcreature, out newPos))
                        {
                            suspended.Remove(newcreature);                            
                            Teleport(newcreature, newPos);
                            newcreature.SetActive(true);
                            // Change the school size every time we teleport a school
                            SchoolGenerator sg = newcreature.GetComponentInChildren<SchoolGenerator>();
                            if (sg != null)
                            {
                                sg.targetCreatureCount = Random.Range(sg.minBoidCount, sg.maxBoidCount);
                            }
                            alive.Add(newcreature);
                            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = newPos;
                            cube.transform.localScale = Vector3.one * 5;
                        }
                    }
                    else
                    {
                        Debug.Log("Creating a new creature: " + alive.Count + 1);
                        newcreature = GameObject.Instantiate<GameObject>(prefabs[nextCreature], newPos
                            , prefabs[nextCreature].transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                            );
                        newcreature.transform.parent = this.transform;
                        if (FindPlace(newcreature, out newPos))
                        {
                            newcreature.transform.position = newPos;
                            alive.Add(newcreature);
                        }
                        else
                        {
                            Debug.Log("Couldnt find a place for it so suspending it");
                            Suspend(newcreature);
                        }
                        nextCreature = (nextCreature + 1) % prefabs.Length;
                    }

                    //newcreature.GetComponent<CreatureGenerator>().CreateCreature();

                }
                yield return new WaitForSeconds(delay);
            }
        }

        public Vector3 GetCreaturePosition(GameObject creature)
        {
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
                return creature.GetComponent<FormationGenerator>().leader.transform.position;
            }
            else
            {
                return Utilities.FindBoidInHierarchy(creature).position;

            }
        }

        private void Teleport(GameObject newcreature, Vector3 newPos)
        {
            Vector3 boidPos = GetCreaturePosition(newcreature);
            if (newcreature.GetComponent<SchoolGenerator>() == null)
            {
                Vector3 trans = newPos - boidPos;
                newcreature.transform.position += trans;
                Boid boid = Utilities.FindBoidInHierarchy(newcreature);
                // Translate it to the new position                            
                boid.suspended = false;
                boid.transform.position = newPos;
                boid.position = newPos; // The boid
                boid.desiredPosition = newPos;
                if (boid.GetComponent<Constrain>() != null)
                {
                    boid.GetComponent<Constrain>().centre += trans;
                }

                if (newcreature.GetComponentInChildren<BigCreatureController>())
                {
                    newcreature.GetComponentInChildren<BigCreatureController>().Restart();
                }

                if (boid.GetComponent<TrailRenderer>() != null)
                {
                    boid.GetComponent<TrailRenderer>().Clear();
                }
                if (newcreature.GetComponent<FormationGenerator>())
                {
                    foreach (GameObject follower in newcreature.GetComponent<FormationGenerator>().followers)
                    {
                        Boid b = Utilities.FindBoidInHierarchy(follower);
                        b.suspended = false;
                        b.position += trans;
                        b.desiredPosition += trans;
                    }
                }

            }
            else
            {
                newcreature.transform.position = newPos;
            }
        }

        private void Awake()
        {
            Instance = this;
            if (Application.isEditor)
            {
                //maxcreatures = 10;
            }
        }

        // Use this for initialization
        void Start()
		{
			StartCoroutine(Spawn());
		}

		// Update is called once per frame
		void Update()
		{
            CreatureManager.Log("Alive creatures: " + alive.Count);
            CreatureManager.Log("Suspended creatures: " + suspended.Count);
        }
    }
}