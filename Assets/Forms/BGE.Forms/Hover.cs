using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Hover:Harmonic
    {
        public bool automatic = true;
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (boid != null)
            {
                Gizmos.DrawLine(boid.transform.position, boid.transform.position + (force));
            }
        }

        float oldTheta = 0.0f;

        public override void Start()
        {
            oldTheta = theta;
            base.Start();
        }

        public float thetaDelta;

        public override Vector3 Calculate()
        {
            Vector3 force = Vector3.zero;
            theta = theta % (Utilities.TWO_PI);
            rampedAmplitude = Mathf.Lerp(rampedAmplitude, amplitude, boid.TimeDelta);

            if (automatic)
            {
                rampedSpeed = Mathf.Lerp(rampedSpeed, speed, boid.TimeDelta);
                theta += boid.TimeDelta * rampedSpeed * Mathf.Deg2Rad;
            }

            thetaDelta = theta - oldTheta;
            if ((theta < Mathf.PI & thetaDelta > 0) || (theta > Mathf.PI && thetaDelta < 0))
            {
                force = boid.forward 
                        * Mathf.Abs(thetaDelta)
                        * rampedAmplitude;                    
            }        
        
            oldTheta = theta;
            return force;
        }
    }
}