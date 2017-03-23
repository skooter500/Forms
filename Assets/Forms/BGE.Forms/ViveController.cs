using System;
using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class ViveController : MonoBehaviour {

        public SteamVR_TrackedObject leftTrackedObject;
        public SteamVR_TrackedObject rightTrackedObject;
        private Rigidbody rigidBody;

        public GameObject leftEngine;
        public GameObject rightEngine;

        public GameObject head;
        public float maxSpeed = 250.0f;
        public float power = 1000.0f;

        public Boid boid; // Am I controlling a boid?

        private SteamVR_Controller.Device leftController
        {
            get
            {
                return SteamVR_Controller.Input((int)leftTrackedObject.index);
            }
        }

        private SteamVR_Controller.Device rightController
        {
            get
            {
                return SteamVR_Controller.Input((int)rightTrackedObject.index);
            }
        }

        // Use this for initialization
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            desiredYaw = transform.rotation;
        }

        System.Collections.IEnumerator StraightenUp()
        {
            float t = 0;
            Quaternion current = transform.rotation;
            Vector3 cv = current.eulerAngles;
            Quaternion desired = Quaternion.Euler(0, cv.y, 0);
            while (t < 1.0f)
            {
                transform.rotation = Quaternion.Slerp(current, desired, t);
                t += Time.deltaTime * 0.2f;
                yield return null;
            }
        }

        void DetatchFromBoid()
        {
            if (boid != null)
            {
                Constrain con = boid.GetComponent<Constrain>();
                if (con != null)
                {
                    con.Activate(true);
                }

                HarmonicController hc = boid.GetComponent<HarmonicController>();
                if (boid.GetComponent<HarmonicController>() != null)
                {
                    hc.enabled = true;
                }

                boid.GetComponent<Harmonic>().Activate(true);
                boid.GetComponent<PlayerSteering>().Activate(false);
                GetComponent<Rigidbody>().isKinematic = false;
                boid.damping = 0.5f;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.transform.parent = null;
                StartCoroutine("StraightenUp");
                boid = null;
                RotateMe[] r = FindObjectsOfType<RotateMe>();
                foreach (RotateMe rm in r)
                {
                    rm.speed = 0.1f;
                }                
            }
        }


        Quaternion desiredYaw;

        // Update is called once per frame
        void FixedUpdate()
        {
            float leftTrig = 0.0f;
            float rightTrig = 0.0f;

            if (leftTrackedObject != null && leftTrackedObject.isActiveAndEnabled)
            {
                // The trigger button
                leftTrig = leftController.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;

                if (leftTrig > 0.2f)
                {
                    if (boid != null)
                    {
                        rigidBody.AddForceAtPosition(leftTrackedObject.transform.forward * power * leftTrig, leftTrackedObject.transform.position);
                        leftEngine.GetComponent<JetFire>().fire = leftTrig;
                    }
                    else
                    {
                        boid.maxSpeed *= leftTrig;
                        boid.GetComponent<Harmonic>().speed *= leftTrig;
                    }
                }
            }
        
            if (rightTrackedObject != null && rightTrackedObject.isActiveAndEnabled)
            {
                // The trigger button
                rightTrig = rightController.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;

                if (rightTrig > 0.2f)
                {
                    if (boid != null)
                    {
                        rigidBody.AddForceAtPosition(leftTrackedObject.transform.forward * power * leftTrig, leftTrackedObject.transform.position);
                        rightEngine.GetComponent<JetFire>().fire = rightTrig;
                    }
                    else
                    {
                        boid.maxSpeed *= rightTrig;
                        boid.GetComponent<Harmonic>().speed *= rightTrig;
                    }
                }
            }

            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

            float max = 3500;
            /*
            try
            {

                SteamVR_Controller.Device l = SteamVR_Controller.Input(
                    SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
                if (l != null)
                {
                    l.TriggerHapticPulse((ushort) (leftTrig*max));
                }

                SteamVR_Controller.Device r = SteamVR_Controller.Input(
                    SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
                if (r != null)
                {
                    r.TriggerHapticPulse((ushort) (rightTrig*max));
                }
            }
            catch (IndexOutOfRangeException e)
            {
                
            }
            */

            /*
            if (leftTrig > 0.2f && rightTrig > 0.2f)
            {
                rigidBody.AddForce(head.transform.forward * power * 10);
            }
            else if (leftTrig > 0.2f)
            {
                desiredYaw *= Quaternion.AngleAxis(leftTrig * 0.5f, Vector3.up);
            }
            else if (rightTrig > 0.2f)
            {
                desiredYaw *= Quaternion.AngleAxis(rightTrig * 0.5f, -Vector3.up);
            }

            float currentYaw = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredYaw, Time.deltaTime);
            */
        }
    }
}