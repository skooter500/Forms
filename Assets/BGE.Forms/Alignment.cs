using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Alignment : SteeringBehaviour
    {
        public void Start()
        {
            boid.tagNeighbours = true;
        }
        public override Vector3 Calculate()
        {
            Vector3 steeringForce = Vector3.zero;
            int taggedCount = 0;
            foreach (Boid other in boid.tagged)
            {
                if (other != this)
                {
                    steeringForce += other.forward;
                    taggedCount++;
                }
            }

            if (taggedCount > 0)
            {
                steeringForce /= (float)taggedCount;
                steeringForce -= boid.forward;
            }
            return steeringForce;
        }
    }
}