using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class JitterWander: SteeringBehaviour
    {
        [Range(0.0f, 100.0f)]
        public float radius = 10.0f;

        [Range(0.0f, 1000.0f)]
        public float jitter = 5.0f;

        [Range(0.0f, 100.0f)]
        public float distance = 15.0f;

        private Vector3 target;

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.blue;
                Vector3 wanderCircleCenter = Utilities.TransformPointNoScale(Vector3.forward*distance, transform);
                Gizmos.DrawWireSphere(wanderCircleCenter, radius);
                Gizmos.color = Color.gray;
                Vector3 worldTarget = Utilities.TransformPointNoScale(target + Vector3.forward*distance, transform);
                Gizmos.DrawLine(transform.position, worldTarget);
            }
        }

        public void Start()
        {
            target = Utilities.RandomInsideUnitSphere() * radius;
        }

        public override Vector3 Calculate()
        {
            float jitterTimeSlice = jitter * boid.TimeDelta;

            Vector3 toAdd = Utilities.RandomInsideUnitSphere() * jitterTimeSlice;
            target += toAdd;
            target.Normalize();
            target *= radius;

            Vector3 localTarget = target + Vector3.forward * distance;
            Vector3 worldTarget = boid.TransformPoint(localTarget);
            return (worldTarget - boid.position);
        }
    }
}