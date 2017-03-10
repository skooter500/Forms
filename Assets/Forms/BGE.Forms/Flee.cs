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
        public Boid targetBoid;

        public override Vector3 Calculate()
        {
            if (targetBoid != null)
            {
                target = targetBoid.position;
            }    

            if (Vector3.Distance(boid.position, target) < fleeRange)
            {
                return boid.FleeForce(target);
            }
            else
            {
                return Vector3.zero;
            }    
        }
    }
}