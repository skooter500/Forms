
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace BGE.Forms
{
    public class SchoolGenerator : School
    {
        public int minBoidCount = 50;
        public int maxBoidCount = 100;
        public GameObject prefab;

        public bool spawnInTopHemisphere;

        [Range(0, 1)]
        public float spread;

        [Range(0, 1)]
        public float speedVariation = 0.1f;

        public List<GameObject> alive = new List<GameObject>();
        public List<GameObject> suspended = new List<GameObject>();

        public bool spawmInWorld = true;

        System.Collections.IEnumerator ManageSchool()
        {
            int maxAudioBoids = 5;
            int audioBoids = 0;

            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            LifeColours lc = GetComponent<LifeColours>();
            while (true)
            {
                yield return null;
                while (alive.Count < targetCreatureCount)
                {                    
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    Vector3 pos = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);

                    GameObject fish = null;
                    TrailRenderer tr;
                    if (suspended.Count > 0)
                    {
                        fish = suspended[suspended.Count - 1];
                        suspended.RemoveAt(suspended.Count - 1);
                        fish.SetActive(true);
                        fish.GetComponentInChildren<Boid>().suspended = false;                                                
                    }
                    else
                    {
                        fish = GameObject.Instantiate<GameObject>(prefab, pos, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));                       
                    }

                    alive.Add(fish);

                    if (wg != null && spawmInWorld)
                    {
                        float groundHeight = wg.SamplePos(pos.x, pos.z);
                        if (pos.y < groundHeight)
                        {
                            pos.y = groundHeight + UnityEngine.Random.Range(10, radius * spread);
                        }
                    }

                    fish.SetActive(true);

                    tr = fish.GetComponentInChildren<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.enabled = false;
                        tr.Clear();
                    }
                    fish.transform.position = pos;
                    fish.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
                    if (tr != null)
                    {
                        tr.enabled = true;
                        tr.Clear();
                    }
                    fish.transform.parent = transform;
                    Boid boid = fish.GetComponentInChildren<Boid>();
                    if (boid != null)
                    {
                        boid.school = this;
                        Constrain c = boid.GetComponent<Constrain>();
                        if (c != null)
                        {
                            c.radius = radius;
                            c.centre = pos;
                        }
                        //boid.transform.position = pos;
                        boid.position = pos;
                        boid.desiredPosition = pos;
                        boid.maxSpeed += boid.maxSpeed * UnityEngine.Random.Range(-speedVariation, speedVariation);

                        boids.Add(boid);
                    }

                    AudioSource audioSource = fish.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        if (audioBoids < maxAudioBoids)
                        {
                            audioSource.enabled = true;
                            audioSource.loop = true;
                            audioSource.Play();
                            audioBoids++;
                        }
                        else
                        {
                            audioSource.enabled = false;
                        }
                    }

                    if (lc != null)
                    {
                        lc.FadeIn();
                    }
                    // Wait for a frame
                    yield return null;
                    // Suspend any that we dont need                    
                }
                while (alive.Count > targetCreatureCount)
                {
                    // Suspend the creature
                    GameObject creature = alive[alive.Count - 1];
                    creature.SetActive(false);
                    Boid b = creature.GetComponentInChildren<Boid>();
                    if (b != null)
                    {
                        b.suspended = true;
                    }
                    suspended.Add(creature);
                    alive.RemoveAt(alive.Count - 1);
                    boids.Remove(b);
                    yield return null;
                }
            }
        }

        public void Suspend()
        {
            for(int i = alive.Count - 1; i >= 0; i --)
            {
                GameObject fish = alive[i];
                fish.SetActive(false);
                Boid b = fish.GetComponentInChildren<Boid>();
                if (b != null)
                {
                    b.suspended = true;
                }
                suspended.Add(fish);
                alive.RemoveAt(alive.Count - 1);
                boids.Remove(b);
            }
        }
    
        public void Update()
        {
            CreatureManager.Log("T: " + targetCreatureCount + " A: " + alive.Count + " S: " + suspended.Count);

            if (Input.GetKeyDown(KeyCode.P))
            {
                targetCreatureCount += 5;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (targetCreatureCount > 5)
                {
                    targetCreatureCount -= 5;
                }
            }

        }

        void Start()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            targetCreatureCount = Random.Range(minBoidCount, maxBoidCount);
        }

        Coroutine cr = null;

        void OnEnable()
        {
            cr = StartCoroutine(ManageSchool());
        }
    }
}