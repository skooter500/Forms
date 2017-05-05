using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{
    public class scuttleWander : SteeringBehaviour
    {

        [Range(0.0f, 100.0f)]
        public float radius = 10.0f;

        [Range(0.0f, 100.0f)]
        public float distance = 15.0f;

        [Range(0.0f, 100.0f)]
        public float changeDir = 10.0f;

        private float random =0;
        private float dirspeed = 0;
        private Vector3 target;

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.blue;
                Vector3 wanderCircleCenter = Utilities.TransformPointNoScale(Vector3.forward * distance, transform);
                Gizmos.DrawWireSphere(wanderCircleCenter, radius);
                Gizmos.color = Color.gray;
                Vector3 worldTarget = Utilities.TransformPointNoScale(target + Vector3.forward * distance, transform);
                Gizmos.DrawLine(transform.position, worldTarget);
            }
        }

        public void Start()
        {
            target = Utilities.RandomInsideUnitSphere() * radius;
        }

        public override Vector3 Calculate()
        {
            Vector3 toAdd = Utilities.RandomInsideUnitSphere();
            dirspeed = boid.speed;
            random = Random.Range(1.0f, 100.0f);
            if (random < changeDir)
            {
                dirspeed = -dirspeed;
                //Now we go from right to left
                toAdd.z = toAdd.z + dirspeed;
                toAdd.y = toAdd.y - 0.25f;//Diving value
            }
            target += toAdd;
            target.Normalize();
            target *= radius;

            Vector3 localTarget = target + Vector3.forward * distance;
            Vector3 worldTarget = boid.TransformPoint(localTarget);
            return (worldTarget - boid.position);
        }
    }
}