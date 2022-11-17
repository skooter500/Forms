
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;


namespace BGE.Forms
{
    public class Mother : MonoBehaviour
    {
        public int spawnRate = 5;
        public int maxcreatures = 1;

        public float playerRadius = 1000;

        public List<SpawnParameters> alive = new List<SpawnParameters>();
        public List<SpawnParameters> dead = new List<SpawnParameters>();
        public GameObject[] prefabs;

        public LayerMask environmentLM;

        public static Mother Instance;

        School school;

        public bool spawnInFront = false;

        public float fov = 20;

        public GameObject player;

        Vector3 FindPlace(SpawnParameters sp)
        {           
            if (sp == null)
            {
                Debug.Log("Creature : " + sp.gameObject + " doesnt have spawn parameters!!!");
                return Vector3.zero;
            }

            float start = Mathf.Min(sp.start, sp.end);

            //Vector3 r = Vector3.forward; // Random.insideUnitSphere;
            //r.z = spawnInFront ? Mathf.Abs(r.z) : r.z;
            //r.y = 0;
            Vector3 r = Vector3.forward;
            r *= Random.Range(start, sp.end);
            r += (r.normalized * start);
            //r = Quaternion.AngleAxis(Random.Range(-fov, fov), Vector3.up) * r;

            Vector3 newPos = player.transform.TransformPoint(r);
            float sampleY = WorldGenerator.Instance.SamplePos(newPos.x, newPos.z);
            float worldMax = WorldGenerator.Instance.surfaceHeight - sp.minDistanceFromSurface;
            float minHeight = sampleY + sp.minHeight;                
            float maxHeight = Mathf.Min(sampleY + sp.maxHeight, worldMax);
            newPos.y = Mathf.Min(Random.Range(minHeight, maxHeight), worldMax);
            return newPos;
        }
      
        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {           
            StartCoroutine(Spawn());
        }

        private void Teleport(SpawnParameters creature, Vector3 newPos)
        {
            Debug.Log("Teleporting: " + creature);
            Vector3 boidPos = creature.boid.position;
            if (creature.GetComponent<SchoolGenerator>() == null && creature.GetComponent<SandWorm>() == null)
            {

                Vector3 trans = newPos - boidPos;
                creature.transform.position += trans;
                Boid boid = creature.boid;
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


        System.Collections.IEnumerator Spawn()
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject prefab = prefabs[i];
                Vector3 pos = FindPlace(prefab.GetComponent<SpawnParameters>());
                SpawnParameters creature = GameObject.Instantiate(prefabs[i], pos, Quaternion.identity).GetComponent<SpawnParameters>();
                if (creature.boid == null)
                {
                    
                    creature.boid = creature.GetComponentInChildren<Boid>();
                    Debug.Log("Assigning boid to: " + creature.boid + " for creature: " + creature);
                }
                if (i >= maxcreatures)
                {
                    creature.gameObject.SetActive(true);
                    //yield return null;
                    creature.gameObject.SetActive(false);
                    dead.Add(creature);
                }
                else
                {
                    creature.gameObject.SetActive(true);
                    alive.Add(creature);
                }
                creature.gameObject.transform.parent = this.transform;
            }
            while (true)
            {
                //Remove too far
                for (int i = alive.Count - 1; i >= 0; i--)
                {
                    SpawnParameters sp = alive[i];
                    float f = Vector3.Distance(sp.boid.position, player.transform.position);
                    if (f > 15000)
                    {
                        Debug.Log("Removing: " + sp);
                        sp.gameObject.SetActive(false);
                        dead.Add(sp);
                        alive.RemoveAt(i);
                    }
                }
                // Create as needed
                if (alive.Count < maxcreatures)
                {
                    SpawnParameters creature = dead[0];
                    Vector3 pos = FindPlace(creature);
                    Teleport(creature, pos);
                    creature.gameObject.SetActive(true);
                    dead.RemoveAt(0);
                    alive.Add(creature);
                }
                yield return null;
            }
        }

            // Update is called once per frame
            void Update()
        {
            CreatureManager.Log("Alive species: " + alive.Count);
            CreatureManager.Log("Suspended species: " + dead.Count);
        }
    }
}