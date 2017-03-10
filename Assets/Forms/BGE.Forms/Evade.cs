using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Evade: SteeringBehaviour
    {
        public Boid enemy = null;    

        public override Vector3 Calculate()
        {
            float dist = (enemy.position - boid.position).magnitude;
            float lookAhead = boid.maxSpeed;

            Vector3 target = enemy.position + (lookAhead * enemy.velocity);
            return boid.FleeForce(target);
        }
    }
}