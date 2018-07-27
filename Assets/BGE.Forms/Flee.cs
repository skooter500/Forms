using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Flee : SteeringBehaviour
    {
        public float fleeRange = 100.0f;
        public Vector3 target = Vector3.zero;
        public GameObject targetGameObject;

        private float originalSpeed;

        public void Start()
        {
            originalSpeed = boid.maxSpeed;
        }

        public override void Update()
        {
            base.Update();
            if (targetGameObject != null)
            {
                target = targetGameObject.transform.position;
            }
        }

        public override Vector3 Calculate()
        {
            if (Vector3.Distance(boid.position, target) < fleeRange)
            {
                boid.maxSpeed = originalSpeed * 5.0f;
                return boid.FleeForce(target);
            }
            else
            {
                boid.maxSpeed = originalSpeed;
                return Vector3.zero;
            }    
        }
    }
}