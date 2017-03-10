using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BGE.Forms
{
    public class Boid : MonoBehaviour
    {
        // Need these because we might be running on a thread and can't touch the transform
        [Header("Transform")]
        public Vector3 position = Vector3.zero;
        public Vector3 forward = Vector3.forward;
        public Vector3 up = Vector3.up;
        public Vector3 right = Vector3.right;
        public Quaternion rotation = Quaternion.identity;

        private Vector3 tempUp;

        // Variables required to implement the boid
        [Header("Boid Attributes")]
        public float mass = 1.0f;
        public float maxSpeed = 20.0f;
        public float maxForce = 10.0f;
        public float weight = 1.0f;
        [Range(0.0f, 10.0f)]
        public float timeMultiplier = 1.0f;
        [Range(0.0f, 1.0f)]
        public float damping = 0.01f;
        public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
        public CalculationMethods calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;
        public float radius = 5.0f;
        public float maxTurnDegrees = 180.0f;
        public bool applyBanking = true;
        public bool keepUpright = false;
        public float straighteningTendancy = 0.2f;
        public float rollingTendancy = 0.05f;
        public bool integrateForces = true;
        public float preferredTimeDelta = 0.0f;

        public bool tagNeighbours = false;
        public float tagNeighboursDither = 0.5f;

        public float bank;

        [HideInInspector]
        public List<Boid> tagged = new List<Boid>();

        //[HideInInspector]
        public volatile School school;

        public bool enforceNonPenetrationConstraint;

        public Vector3 force = Vector3.zero;

        [HideInInspector]
        public Vector3 velocity = Vector3.zero;

        [HideInInspector]
        public Vector3 acceleration;

        [Header("Gravity")]
        public bool applyGravity = false;
        public Vector3 gravity = new Vector3(0, -9, 0);

        [HideInInspector]
        public bool multiThreaded = false;

        [HideInInspector]
        public SteeringBehaviour[] behaviours;

        public string testField;

        public float TimeDelta
        {
            get
            {
                float flockMultiplier = (school == null) ? 1 : school.timeMultiplier;
                float timeDelta = multiThreaded ? CreatureManager.threadTimeDelta : Time.deltaTime;
                return timeDelta * flockMultiplier * timeMultiplier;
            }
        }
        void Start()
        {
            desiredPosition = transform.position;
            timeAcc = preferredTimeDelta;
            UpdateLocalFromTransform();

            behaviours = GetComponents<SteeringBehaviour>();

            //if (transform.parent.gameObject.GetComponent<School>() != null)
            //{
            //    school = transform.parent.gameObject.GetComponent<School>();
            //}

        }

        #region Integration


        public void UpdateLocalFromTransform()
        {
            position = transform.position;
            up = transform.up;
            right = transform.right;
            forward = transform.forward;
            rotation = transform.rotation;
        }

        public Vector3 TransformDirection(Vector3 direction)
        {
            return rotation * direction;
        }

        public Vector3 TransformPoint(Vector3 localPoint)
        {
            Vector3 p = rotation * localPoint;
            p += position;
            return p;
        }

        float timeAcc = 0;

        [HideInInspector]
        public Vector3 desiredPosition = Vector3.zero;

        [HideInInspector]
        public float gravityAcceleration = 0;

        void FixedUpdate()
        {
            //return;
            float smoothRate;

            if (!multiThreaded)
            {
                UpdateLocalFromTransform();
                force = CalculateForce();
            }

            timeAcc += Time.deltaTime;

            if (timeAcc > preferredTimeDelta)
            {
                float timeAccMult = timeAcc * timeMultiplier;
                if (school != null)
                {
                    timeAccMult *= school.timeMultiplier;
                }
                Vector3 newAcceleration = force / mass;
                if (timeAcc > 0.0f)
                {
                    smoothRate = Utilities.Clip(9.0f * timeAccMult, 0.15f, 0.4f) / 2.0f;
                    Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
                }

                if (applyGravity)
                {
                    velocity += gravity * timeAccMult;
                }

                velocity += acceleration * timeAccMult;

                if (integrateForces)
                {
                    desiredPosition = desiredPosition + (velocity * timeAccMult);
                }

                // the length of this global-upward-pointing vector controls the vehicle's
                // tendency to right itself as it is rolled over from turning acceleration
                Vector3 globalUp = new Vector3(0, straighteningTendancy, 0);
                // acceleration points toward the center of local path curvature, the
                // length determines how much the vehicle will roll while turning
                Vector3 accelUp = acceleration * rollingTendancy;
                accelUp.y = 0; // Cancel out the up down
                // combined banking, sum of UP due to turning and global UP
                Vector3 bankUp = accelUp + globalUp;
                // blend bankUp into vehicle's UP basis vector
                smoothRate = timeAccMult;// * 3.0f;
                Vector3 tempUp = transform.up;
                Utilities.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);

                float speed = velocity.magnitude;
                if (speed > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }
                Utilities.checkNaN(velocity);

                if (speed > 0.01f)
                {
                    transform.forward = Vector3.RotateTowards(transform.forward, velocity, Mathf.Deg2Rad * maxTurnDegrees * Time.deltaTime, float.MaxValue);
                    if (keepUpright)
                    {
                        Vector3 uprightForward = transform.forward;
                        uprightForward.y = 0;
                        transform.forward = uprightForward;
                    }
                }
                

                if (applyBanking) 
                {
                    Quaternion q = Quaternion.LookRotation(transform.forward, tempUp);
                    transform.rotation = q;
                }
                velocity *= (1.0f - (damping * timeAccMult));
                timeAcc = 0.0f;


                UpdateLocalFromTransform();
            }

            if (preferredTimeDelta != 0.0f && integrateForces)
            {
                float timeDelta = Time.deltaTime * timeMultiplier;
                timeDelta *= (school == null) ? 1 : school.timeMultiplier;
                float dist = Vector3.Distance(transform.position, desiredPosition);
                float distThisFrame = dist * (timeDelta / preferredTimeDelta);
                transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 50 * Time.deltaTime);
            }
            else
            {
                if (integrateForces) transform.position = desiredPosition;
            }
        }

        private bool AccumulateForce(ref Vector3 runningTotal, ref Vector3 clampedForce, Vector3 force)
        {
            float soFar = runningTotal.magnitude;

            float remaining = maxForce - soFar;
            if (remaining <= 0)
            {
                return false;
            }

            float toAdd = force.magnitude;


            if (toAdd < remaining)
            {
                clampedForce = force;
            }
            else
            {
                clampedForce = Vector3.Normalize(force) * remaining;

            }
            runningTotal += clampedForce;
            return true;
        }

        public Vector3 CalculateForce()
        {
            Vector3 totalForce = Vector3.zero;

            if (tagNeighbours && school != null)
            {
                int taggedCount = TagNeighboursSimple(school.neighbourDistance);
            }

            foreach (SteeringBehaviour behaviour in behaviours)
            {
                if (behaviour.active)
                {
                    Vector3 force = behaviour.Calculate() * behaviour.weight;
                    force *= weight;
                    bool full = AccumulateForce(ref totalForce, ref behaviour.force, force);
                    behaviour.forceMagnitude = behaviour.force.magnitude / maxForce;
                    if (!full)
                    {
                        break;
                    }
                }
            }


            // Calculate how much banking there is so that the fins can animate 
            Vector3 projectRight = right;
            projectRight.y = 0;
            projectRight.Normalize();
            bank = Vector3.Angle(right, projectRight);
            bank = (right.y > 0) ? bank : -bank;
            return totalForce;
        }
        #endregion

        // Shared behaviours
        public Vector3 SeekForce(Vector3 targetPos)
        {
            Vector3 desiredVelocity;

            desiredVelocity = targetPos - position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            return (desiredVelocity - velocity);
        }

        public Vector3 FleeForce(Vector3 target)
        {
            Vector3 desiredVelocity;
            desiredVelocity = position - target;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            Utilities.checkNaN(desiredVelocity);
            return (desiredVelocity - velocity);
        }

        public Vector3 ArriveForce(Vector3 target, float slowingDistance = 15.0f, float deceleration = 1.0f)
        {
            Vector3 toTarget = target - position;

            float distance = toTarget.magnitude;
            float ramped = maxSpeed * (distance / (slowingDistance * deceleration));

            float clamped = Math.Min(ramped, maxSpeed);
            Vector3 desired = clamped * (toTarget / distance);
            Utilities.checkNaN(desired);

            return desired - velocity;
        }

        private int TagNeighboursSimple(float inRange)
        {
            float dice = Utilities.RandomRange(0.0f, 1.0f);
            if (dice < tagNeighboursDither)
            {
                tagged.Clear();

                float inRangeSq = inRange * inRange;
                foreach (Boid boid in school.boids)
                {
                    if (boid != this)
                    {
                        if ((position - boid.position).sqrMagnitude < inRangeSq)
                        {
                            tagged.Add(boid);
                        }
                    }
                }
            }
            return tagged.Count;
        }
    }
}