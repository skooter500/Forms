
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

        public virtual void Teleport(Vector3 newHome, Vector3 trans, Boid calculationBoid)
        {
            foreach (Boid b in boids)
            {
                if (b != calculationBoid)
                {
                    b.position += trans;
                    b.desiredPosition += trans;
                    if (b.GetComponent<Constrain>() != null)
                    {
                        b.GetComponent<Constrain>().centre += trans;
                    }
                    b.suspended = false;
                    if (b.GetComponent<TrailRenderer>()!= null)
                    {
                        b.GetComponent<TrailRenderer>().Clear();
                    }
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