
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

        System.Collections.IEnumerator ManageSchool()
        {
            int maxAudioBoids = 5;
            int audioBoids = 0;

            WorldGenerator wg = FindObjectOfType<WorldGenerator>();
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                while (alive.Count < targetCreatureCount)
                {                    
                Vector3 unit = UnityEngine.Random.insideUnitSphere;
                Vector3 pos = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);

                    GameObject fish = null;
                    if (suspended.Count > 0)
                    {
                        fish = suspended[suspended.Count - 1];
                        suspended.RemoveAt(suspended.Count - 1);
                        fish.SetActive(true);
                        fish.GetComponentInChildren<Boid>().suspended = false;
                        TrailRenderer tr = fish.GetComponentInChildren<TrailRenderer>();
                        if (tr != null)
                        {
                            tr.Clear();
                        }
                    }
                    else
                    {
                        fish = GameObject.Instantiate<GameObject>(prefab, unit, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                    }

                    alive.Add(fish);

                    if (wg != null)
                    {
                        float groundHeight = wg.SamplePos(pos.x, pos.z);
                        if (pos.y < groundHeight)
                        {
                            pos.y = groundHeight + UnityEngine.Random.Range(10, radius * spread);
                        }
                    }

                    fish.transform.position = pos;
                    fish.transform.parent = transform;
                    Boid boid = fish.GetComponentInChildren<Boid>();
                    if (boid != null)
                    {
                        boid.school = this;
                        boid.GetComponent<Constrain>().radius = radius;
                        boid.GetComponent<Constrain>().centre = pos;
                        boid.position = pos;
                        boid.desiredPosition = pos;
                        //boid.maxSpeed += boid.maxSpeed * UnityEngine.Random.Range(-speedVariation, speedVariation);

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

        void OnEnable()
        {
            StartCoroutine(ManageSchool());
        }
    }
}