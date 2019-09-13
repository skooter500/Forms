using UnityEngine;
using System.Collections;
using System;

namespace BGE.Forms
{
    public class OffsetPursue : SteeringBehaviour
    {
        [HideInInspector]
        public Boid leaderBoid;
        public GameObject leader;
        public Vector3 offset = Vector3.zero;
        private Vector3 targetPos;

        public float pitchForceScale = 1;

        public bool autoAssignOffset = true;

        public void Start()
        {
            if (autoAssignOffset && leader  != null)
            {
                leaderBoid = leader.GetComponentInChildren<Boid>();
                offset = transform.position - leader.transform.position;
                offset = Quaternion.Inverse(leader.transform.rotation) * offset;
                targetPos = transform.position;
            }            
        }

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled && boid.drawGizmos && leaderBoid != null )
            {
                Gizmos.color = Color.yellow;
                if (Application.isPlaying)
                {
                    Gizmos.DrawLine(transform.position, targetPos);
                }
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, leaderBoid.transform.position);
            }
        }

        public override Vector3 Calculate()
        {
            Vector3 newTarget = Vector3.zero;

            newTarget = leaderBoid.TransformPoint(offset);

            float dist = (newTarget - boid.position).magnitude;

            float lookAhead = (dist / boid.maxSpeed);

            newTarget = newTarget + (lookAhead * leaderBoid.velocity);

            float pitchForce = newTarget.y - boid.position.y;
            pitchForce *= (1.0f - pitchForceScale);
            newTarget.y -= pitchForce;

            targetPos = newTarget;
            return boid.SeekForce(targetPos);
        }
    }
}