using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Seperation : SteeringBehaviour
    {

        public void Start()
        {
            boid.tagNeighbours = true;
        }

        public override Vector3 Calculate()
        {
            Vector3 steeringForce = Vector3.zero;
            foreach (Boid other in boid.tagged)
            {
                if (other != this.boid)
                {
                    Vector3 toEntity = boid.position - other.position;
                    steeringForce += (Vector3.Normalize(toEntity) / toEntity.magnitude);
                }
            }

            return steeringForce;
        }
    }
}