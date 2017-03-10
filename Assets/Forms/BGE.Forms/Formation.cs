using UnityEngine;
using System.Collections;
using System;

namespace BGE.Forms
{
    public class Formation : SteeringBehaviour
    {
        public Boid leaderBoid;
        public GameObject leader;
        public Vector3 offset;
        private Vector3 targetPos;
        public bool useDeadReconing = false;

        public float reformationDistance = 10.0f;


        public void Start()
        {
            if (leader  != null)
            {
                leaderBoid = leader.GetComponentInChildren<Boid>();
                offset = transform.position - leader.transform.position;
                offset = Quaternion.Inverse(leader.transform.rotation) * offset;
                targetPos = leaderBoid.TransformPoint(offset);
            }
        }

        private void RecalculateOffest()
        {
            offset = boid.position - leaderBoid.position;
            offset = Quaternion.Inverse(leaderBoid.rotation) * offset;
            targetPos = leaderBoid.TransformPoint(offset);
        }

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled && leaderBoid != null)
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
            if (leaderBoid != null)
            {
                Vector3 newTarget = leaderBoid.TransformPoint(offset);
                //newTarget.y = leaderBoid.position.y + offset.y;

                if (useDeadReconing)
                {
                    float dist = Vector3.Distance(boid.position, leaderBoid.position);
                    float lookAhead = (dist / boid.maxSpeed);
                    newTarget = newTarget + (lookAhead * leaderBoid.velocity);
                }
                targetPos = Vector3.Lerp(targetPos, newTarget, boid.TimeDelta * 0.2f);

                if (Vector3.Distance(targetPos, boid.position) > reformationDistance)
                {
                    RecalculateOffest();                                        
                }

                return boid.ArriveForce(targetPos, boid.maxSpeed / 2, 15f);
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}