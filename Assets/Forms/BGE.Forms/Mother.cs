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
                    Boid boid = GetCalculationBoid(creature);
                    
            
                    float dist = Vector3.Distance(boid.transform.position, Camera.main.transform.position);
                    //Debug.Log(i + "\t" + dist);
                    if (dist > playerRadius)
					{
                        Boid[] boids = creature.GetComponentsInChildren<Boid>();
                        foreach (Boid b in boids)
                        {
                            b.suspended = true;
                        }
                        creature.SetActive(false);
                        suspended.Add(creature);
                        Debug.Log("Deleting a creature");
                        alive.Remove(creature);
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
						suspended.Remove(newcreature);
                        newcreature.SetActive(true);
                        bool found = FindPlace(newcreature, out newPos);

                        Teleport(newcreature, newPos);
                        // Change the school size every time we teleport a school
                        SchoolGenerator sg = newcreature.GetComponentInChildren<SchoolGenerator>();
                        if (sg != null)
                        {
                            sg.targetCreatureCount = Random.Range(sg.minBoidCount, sg.maxBoidCount);
                        }
                    }
                    else                        
					{
						Debug.Log("Creating a new creature: " + alive.Count + 1);
						newcreature = GameObject.Instantiate<GameObject>(prefabs[nextCreature], newPos
                            , prefabs[nextCreature].transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)
                            );  
						newcreature.transform.parent = this.transform;
                        bool found = FindPlace(newcreature, out newPos);
                        newcreature.transform.position = newPos;
                        nextCreature = (nextCreature + 1) % prefabs.Length;
                    }
                    // The generator
                    alive.Add(newcreature);
                    //newcreature.GetComponent<CreatureGenerator>().CreateCreature();
					
				}
				yield return new WaitForSeconds(delay);
			}            
		}

        public Boid GetCalculationBoid(GameObject creature)
        {
            if (creature.GetComponent<TenticleCreatureGenerator>() != null)
            {
                return creature.GetComponent<TenticleCreatureGenerator>().head.GetComponent<Boid>();
            }
            else
            {
                return Utilities.FindBoidInHierarchy(creature);
            }
        }

        private void Teleport(GameObject newcreature, Vector3 newPos)
        {

            // Restore the creature to its original state
            Boid calculationBoid = GetCalculationBoid(newcreature);
            Vector3 trans = newPos - calculationBoid.transform.position;
            newcreature.transform.position += trans;
            // Translate it to the new position                            
            calculationBoid.suspended = false;
            calculationBoid.transform.position = newPos;
            calculationBoid.position = newPos; // The boid
            calculationBoid.desiredPosition = newPos;
            if (calculationBoid.GetComponent<Constrain>() != null)
            {
                calculationBoid.GetComponent<Constrain>().centre += trans;
            }

            if (newcreature.GetComponent<BigCreatureController>())
            {
                newcreature.GetComponent<BigCreatureController>().Restart();
            }

            if (calculationBoid.GetComponent<TrailRenderer>() != null)
            {
                calculationBoid.GetComponent<TrailRenderer>().Clear();
            }

            // Teleport any schools
            School[] schools = newcreature.GetComponentsInChildren<School>();
            foreach(School school in schools)
            {
                school.Teleport(newPos, trans, calculationBoid);
                
            }

            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = newPos;
            cube.transform.localScale = Vector3.one * 50;
            */
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