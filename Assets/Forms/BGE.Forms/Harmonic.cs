using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Harmonic : SteeringBehaviour
    {
        [Range(0.0f, 3600.0f)]
        public float speed = 30;
        public float frequency = 1.0f;
        public float amplitude = 50;
        public Axis direction = Axis.Horizontal;
        public enum Axis { Horizontal, Vertical };

        [Range(0.0f, Utilities.TWO_PI)]
        public float theta = 0.0f;

        [HideInInspector]
        protected Vector3 target = Vector3.zero;

        [HideInInspector]
        public float rampedSpeed = 0;
        [HideInInspector]
        public float rampedAmplitude = 0;
        [Range(0.0f, 500.0f)]
        public float radius = 50.0f;

        [Range(-500.0f, 500.0f)]
        public float distance = 5.0f;

        public Vector3 yawRoll = Vector3.zero;

        public Vector3 worldTarget;

        public virtual void Start()
        {
            theta = UnityEngine.Random.Range(0, Mathf.PI);
        }

        public virtual void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.blue;
                Vector3 wanderCircleCenter = Utilities.TransformPointNoScale(Vector3.forward * distance, transform);
                Gizmos.DrawWireSphere(wanderCircleCenter, radius);

                if (worldTarget != Vector3.zero)
                {
                    Gizmos.DrawLine(transform.position, worldTarget);
                }
            }
        }

        public override Vector3 Calculate()
        {
            float n = Mathf.Sin(this.theta);
            rampedAmplitude = Mathf.Lerp(rampedAmplitude, amplitude, boid.TimeDelta);

            float t = Utilities.Map(n, -1.0f, 1.0f, -rampedAmplitude, rampedAmplitude);
            float theta = Utilities.DegreesToRads(t);

            if (direction == Axis.Horizontal)
            {
                target.x = Mathf.Sin(theta);
                target.z = Mathf.Cos(theta);
                target.y = 0;
            }
            else
            {
                target.y = Mathf.Sin(theta);
                target.z = Mathf.Cos(theta);
                target.x = 0;
            }

            target *= radius;
        
            // The forces 
            /*
        Vector3 noPitch = boid.force;
        //if (boid.force.magnitude < 0.01)
        {
            noPitch = boid.forward;
            //noPitch.y = 0;           
        }
        noPitch.Normalize();
        worldTarget = boid.position + (target + (noPitch * distance));
        */

            yawRoll = boid.rotation.eulerAngles;
            yawRoll.x = 0;

            Vector3 localTarget = target + (Vector3.forward * distance);
            //Vector3 worldTarget = boid.TransformPoint(localTarget);

            worldTarget = boid.position + Quaternion.Euler(yawRoll) * localTarget;
            if (this.theta > Utilities.TWO_PI)
            {
                this.theta = Utilities.TWO_PI - this.theta;
            }
        
            rampedSpeed = Mathf.Lerp(rampedSpeed, speed, boid.TimeDelta);
            this.theta += boid.TimeDelta * rampedSpeed * Mathf.Deg2Rad;
            return boid.SeekForce(worldTarget);
        }
    }
}