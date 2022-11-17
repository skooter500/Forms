
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

       

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
		
        }

    }
}