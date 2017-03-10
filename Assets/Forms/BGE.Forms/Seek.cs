using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE.Forms
{
    public class Seek : SteeringBehaviour
    {
        public GameObject targetGameObject = null;
        public Vector3 target = Vector3.zero;

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.cyan;
                if (targetGameObject != null)
                {
                    target = targetGameObject.transform.position;
                }
                Gizmos.DrawLine(transform.position, target);
            }
        }
    
        public override Vector3 Calculate()
        {
            return boid.SeekForce(target);    
        }

        public override void Update()
        {
            if (targetGameObject != null)
            {
                target = targetGameObject.transform.position;
            }
        }
    }
}