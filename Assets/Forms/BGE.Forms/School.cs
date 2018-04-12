
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class School: MonoBehaviour
    {
        public float centerOfMassUpdatePerSecond = 1.0f;

        [HideInInspector]
        public Vector3 centerOfMass = Vector3.zero;

        public float neighbourDistance;

        public float radius = 100;

        //[HideInInspector]
        public volatile List<Boid> boids = new List<Boid>();

        [Range(0, 2)]
        public float timeMultiplier = 1.0f;
 
        [Header("Debug")]
        public bool drawGizmos;

        public int targetCreatureCount = 100;

        [Range(0, 1)]
        public float preferredTimeDelta = 0;

        public virtual void Teleport(Vector3 newHome, Vector3 trans, Boid calculationBoid)
        {
            SchoolGenerator sg = GetComponent<SchoolGenerator>();
            foreach (Boid b in boids)
            {                
                if (this is SchoolGenerator)
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    Vector3 pos = newHome + unit * UnityEngine.Random.Range(0, radius * sg.spread);
                    WorldGenerator wg = WorldGenerator.Instance;
                    if (wg != null)
                    {
                        float groundHeight = wg.SamplePos(pos.x, pos.z);
                        if (pos.y < groundHeight)
                        {
                            pos.y = groundHeight + UnityEngine.Random.Range(10, radius * sg.spread);
                        }
                    }
                    b.position = pos;
                    b.desiredPosition = pos;
                    if (b.GetComponent<Constrain>() != null)
                    {
                        b.GetComponent<Constrain>().centre = pos;
                    }
                }
                else
                {
                    if (b != calculationBoid)
                    {
                        b.position += trans;
                        b.desiredPosition += trans;
                        if (b.GetComponent<Constrain>() != null)
                        {
                            b.GetComponent<Constrain>().centre += trans;
                        }
                    }
                }
                b.suspended = false;
                if (b.GetComponent<TrailRenderer>() != null)
                {
                    b.GetComponent<TrailRenderer>().Clear();
                }
            }
        }

        System.Collections.IEnumerator UpdateCenterOfMass()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 0.5f));
            while (true)
            {
                if (centerOfMassUpdatePerSecond == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(1.0f / centerOfMassUpdatePerSecond);
                }
                if (boids.Count == 0)
                {
                    continue;
                }
                Vector3 average = Vector3.zero;
                foreach (Boid boid in boids)
                {
                    average += boid.position;
                }
                average /= boids.Count;
                centerOfMass = average;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
		
        }

        void Start()
        {
            StartCoroutine(UpdateCenterOfMass());
        }
    }
}