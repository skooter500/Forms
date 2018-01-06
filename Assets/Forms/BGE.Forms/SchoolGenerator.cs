﻿
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

        System.Collections.IEnumerator CreateSchool()
        {
            int maxAudioBoids = 5;
            int audioBoids = 0;

            WorldGenerator wg = FindObjectOfType<WorldGenerator>();

            int boidCount = Random.Range(minBoidCount, maxBoidCount);
            while (boids.Count < boidCount)
            {
                Vector3 unit = UnityEngine.Random.insideUnitSphere;
                Vector3 pos = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);
                GameObject fish = GameObject.Instantiate<GameObject>(prefab, unit, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));

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
                    boid.GetComponent<Constrain>().centreOnPosition = true;
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
                // Wait for a frame
                yield return null;
            }
        }

        void Start()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            StartCoroutine(CreateSchool());
        }
    }
}