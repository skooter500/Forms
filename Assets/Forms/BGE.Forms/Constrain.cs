using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Constrain: SteeringBehaviour
    {
        public bool centreOnPosition = true;
        public Vector3 centre = Vector3.zero;
        public float radius = 1000.0f;

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(centre, radius);
            }
        }

        public void Start()
        {
            if (centreOnPosition)
            {
                centre = transform.position;
            }
        }
    
        public override Vector3 Calculate()
        {

            Vector3 toTarget = boid.position - centre;
            float sphereRadius = radius;
            Vector3 steeringForce = Vector3.zero;
            if (toTarget.magnitude > sphereRadius)
            {
                steeringForce = Vector3.Normalize(toTarget) * (sphereRadius - toTarget.magnitude);
            }
            return steeringForce;
        }
    }
}