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
		public List<GameObject> dead = new List<GameObject>();

		public GameObject[] prefabs;

		public LayerMask environmentLM;

        public static Mother Instance;

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

		System.Collections.IEnumerator Spawn()
		{
			float delay = 1.0f / (float)spawnRate;
			WorldGenerator wg = FindObjectOfType<WorldGenerator>();

            int nextCreature = 0;

			while (true)
			{
				// Remove too far creatures
				for (int i = alive.Count -1; i >= 0; i --)
				{
					GameObject creature = alive[i];
                    Boid boid = Utilities.FindBoidInHierarchy(creature);
                    float dist = Vector3.Distance(boid.transform.position, Camera.main.transform.position);
                    //Debug.Log(i + "\t" + dist);
                    if (dist > playerRadius)
					{
                        if (creature.GetComponent<School>() != null)
                        {
                            foreach (Boid b in creature.GetComponent<School>().boids)
                            {
                                b.suspended = true;
                                //CreatureManager.Instance.boids.Remove(b);
                            }
                        }
                        else
                        {
                            boid.suspended = true;
                            //CreatureManager.Instance.boids.Remove(boid);
                        }

                        //GameObject.Destroy(creature);
                        creature.SetActive(false);
                        dead.Add(creature);
                        Debug.Log("Deleting a creature");
                        alive.Remove(creature);

                    }
                }
                
				if (alive.Count < maxcreatures)
				{
					// Find a spawn point
					// Calculate the position
					bool found = false;
					int count = 0;
					Vector3 newPos = Vector3.zero;
					while (!found)
					{
                        SpawnParameters sp = prefabs[nextCreature].GetComponent<SpawnParameters>();
                        Vector3 r = Random.insideUnitSphere;
                        r.z = Mathf.Abs(r.z);
                        r.y = 0;
                        r *= sp.end - sp.start;
                        r += (r.normalized * sp.start);
                       
                        newPos = Camera.main.transform.TransformPoint(r);
						newPos.y = wg.SamplePos(newPos.x, newPos.z) + Random.Range(sp.minHeight, sp.maxHeight);
                        if (newPos.y > WorldGenerator.Instance.surfaceHeight)
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
                        /*
                        bool clear = TestPos(newPos, spaceRequired[nextCreature]);
						if (clear)
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
						GameObject newcreature = null;
                        Boid boid;
						if (dead.Count > 0)
						{
                            Debug.Log("Teleporting an old creature");
							newcreature = dead[dead.Count - 1];
							dead.Remove(newcreature);
                            newcreature.SetActive(true);
                            
                            boid = Utilities.FindBoidInHierarchy(newcreature);
                            // Restore the creature to its original state
                            Vector3 trans = newPos - boid.transform.position;
                            newcreature.transform.position += trans;
                            // Translate it to the new position                            
                            boid.suspended = false;
                            boid.transform.position = newPos;
                            boid.position = newPos; // The boid
                            boid.desiredPosition = newPos;

                            if (newcreature.GetComponent<BigCreatureController>())
                            {
                                newcreature.GetComponent<BigCreatureController>().Restart();
                            }
                        }
                        else                        
						{
							Debug.Log("Creating a new creature: " + alive.Count + 1);
							newcreature = GameObject.Instantiate<GameObject>(prefabs[nextCreature], newPos
                                , prefabs[nextCreature].transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                                );  
							newcreature.transform.parent = this.transform;
                            newcreature.transform.position = newPos;
                        }
                         // The generator
                        alive.Add(newcreature);
                        //newcreature.GetComponent<CreatureGenerator>().CreateCreature();
					}
					else
					{
						//Debug.Log("Couldnt find a place to spawn the creature");
					}
					nextCreature = (nextCreature + 1) % prefabs.Length;
				}
				yield return new WaitForSeconds(delay);
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
            CreatureManager.Log("Suspended creatures: " + dead.Count);
        }
    }
}