using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Arrive: SteeringBehaviour
    {
        public Vector3 targetPosition = Vector3.zero;
        public float slowingDistance = 15.0f;

        [Range(0.0f, 1.0f)]
        public float deceleration = 0.9f;

        public GameObject targetGameObject = null;
        
        public override Vector3 Calculate()
        {
            return boid.ArriveForce(targetPosition, slowingDistance, deceleration);
        }

        public override void Update()
        {
            if (targetGameObject != null)
            {
                targetPosition = targetGameObject.transform.position;
            }
        }
    }
}