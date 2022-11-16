
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

        public Dictionary<SpawnParameters, SpawnParameters> alive = new Dictionary<SpawnParameters, SpawnParameters>();
        public Dictionary<SpawnParameters, SpawnParameters> suspended = new Dictionary<SpawnParameters, SpawnParameters>();
        public SpawnParameters[] prefabs;

        public LayerMask environmentLM;

        public static Mother Instance;

        public float genesisSpawnDistance = 100;

        School school;

        public bool spawnInFront = false;

        public float fov = 20;

        public GameObject player;

        Vector3 FindPlace(SpawnParameters sp)
        {
            bool found = false;
            int count = 0;
            Vector3 newPos = Vector3.zero;
            if (sp == null)
            {
                Debug.Log("Creature : " + sp.gameObject + " doesnt have spawn parameters!!!");
                found = false;
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

            newPos = player.transform.TransformPoint(r);
            float sampleY = WorldGenerator.Instance.SamplePos(newPos.x, newPos.z);
            float worldMax = WorldGenerator.Instance.surfaceHeight - sp.minDistanceFromSurface;
            float minHeight = sampleY + sp.minHeight;                
            float maxHeight = Mathf.Min(sampleY + sp.maxHeight, worldMax);
            newPos.y = Mathf.Min(Random.Range(minHeight, maxHeight), worldMax);
            return newPos;
        }
      
        public SpawnParameters GetCreature(SpawnParameters prefab)
        {
            if (alive.ContainsKey(prefab))
            {
                return alive[prefab];
            }
            else
            {
                if (suspended.ContainsKey(prefab))
                {
                    SpawnParameters sp = suspended[prefab];
                    Vector3 pos = FindPlace(prefab);
                    sp.Teleport(pos);
                    suspended.Remove(prefab);
                    alive[prefab] = sp;
                    sp.gameObject.SetActive(true);
                }
                else
                {
                    Vector3 pos = FindPlace(prefab);
                    SpawnParameters sp = GameObject.Instantiate(prefab, pos, Quaternion.identity) as SpawnParameters;
                    alive[prefab] = sp;
                }
            }

        }


        System.Collections.IEnumerator Spawn()
        {
            float delay = 1.0f / (float)spawnRate;
            WorldGenerator wg = FindObjectOfType<WorldGenerator>();

            int nextSpecies = 0;

            while (true)
            {
                if (alive.Count <= maxcreatures)
                {
                    GetCreature(nextSpecies);
                    yield return null;
                }
                
                
                foreach(SpawnParameters sp in alive)
                {
                    if (sp.boid. )
                    sp.gameObject.SetActive(false);
                    yield return null;
                }
            }



        }


        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {           
            for(int i = 0; i < prefabs.Length; i ++)
            {
                // Find a spawn point
                // Calculate the position
                //Debug.Log("Making a: " + prefabs[i]);
                GetCreature(i);
                if (i > maxcreatures)
                {
                    Debug.Log("Suspending a: " + prefabs[i]);
                    Suspend(i);
                }
            }

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