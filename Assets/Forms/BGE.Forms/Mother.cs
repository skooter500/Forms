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
                    float dist = Vector3.Distance(creature.transform.position, Camera.main.transform.position);
                    //Debug.Log(i + "\t" + dist);
                    if (dist > playerRadius)
					{
                        GameObject.Destroy(creature);
                        // dead.Add(creature);
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
						/*if (dead.Count > 0)
						{
							newcreature = dead[dead.Count - 1];
							dead.Remove(newcreature);
							newcreature.transform.GetChild(0).localPosition = Vector3.zero;
						}
						else
                        */
						{
							//Debug.Log("Creating a new creature: " + alive.Count + 1);
							newcreature = GameObject.Instantiate<GameObject>(prefabs[nextCreature], newPos, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));  
							newcreature.transform.parent = this.transform.parent;
						}
						//Utilities.FindBoidInHierarchy(newcreature).desiredPosition = newPos;
						alive.Add(newcreature);
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

		// Use this for initialization
		void Start()
		{
			StartCoroutine(Spawn());
		}

		// Update is called once per frame
		void Update()
		{
			CreatureManager.Log("Num creatures: " + alive.Count);
		}
	}
}