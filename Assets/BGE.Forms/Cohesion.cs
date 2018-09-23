using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Cohesion : SteeringBehaviour
    {

        public void Start()
        {
            boid.tagNeighbours = true;
        }

        public override Vector3 Calculate()
        {
            Vector3 steeringForce = Vector3.zero;
            Vector3 centreOfMass = Vector3.zero;
            int taggedCount = 0;
            foreach (Boid other in boid.tagged)
            {
                if (other != this)
                {
                    centreOfMass += other.position;
                    taggedCount++;
                }
            }
            if (taggedCount > 0)
            {
                centreOfMass /= (float)taggedCount;

                if (centreOfMass.sqrMagnitude == 0)
                {
                    steeringForce = Vector3.zero;
                }
                else
                {
                    steeringForce = Vector3.Normalize(boid.SeekForce(centreOfMass));
                }
            }
            Utilities.checkNaN(steeringForce);
            return steeringForce;
        }
    }
}