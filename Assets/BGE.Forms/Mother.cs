
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
        public SpawnParameters[] prefabs;

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

        System.Collections.IEnumerator Spawn()
        {
            for(int i = 0; i < prefabs.Length; i++)
            {
                Vector3 pos = FindPlace(prefabs[i]);
                SpawnParameters creature = GameObject.Instantiate(prefabs[i], pos, Quaternion.identity);
                if (i >= maxcreatures)
                {
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
            //while(true)
            //{
            //    // Remove too far
            //    for(int i = alive.Count - 1; i >= 0; i --)
            //    {
            //        SpawnParameters sp = alive[i];
            //        if (Vector3.Distance(sp.transform.position, player.transform.position) > 5000)
            //        {
            //            sp.gameObject.SetActive(false);
            //            dead.Add(sp);
            //            alive.RemoveAt(i);
            //        }
            //    }
            //    // Create as needed
            //    if (alive.Count < maxcreatures)
            //    {
            //        SpawnParameters creature = dead[0];
            //        Vector3 pos = FindPlace(creature);
            //        creature.Teleport(pos);
            //        creature.gameObject.SetActive(true);
            //        dead.RemoveAt(0);
            //        alive.Add(creature);
                 
            //    yield return null;
            //}            
        }

        // Update is called once per frame
        void Update()
        {
            CreatureManager.Log("Alive species: " + alive.Count);
            CreatureManager.Log("Suspended species: " + dead.Count);
        }
    }
}