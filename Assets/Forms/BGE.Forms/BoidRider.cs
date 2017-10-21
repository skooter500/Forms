using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class BoidRider : MonoBehaviour
    {
        bool viveControllers = false;
        // Use this for initialization
        void Start()
        {
            viveControllers = UnityEngine.XR.XRDevice.isPresent;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        Quaternion targetQuaternion = Quaternion.identity;

        void OnTriggerEnter(Collider c)
        {
            GameObject other = c.gameObject;
            if (other.tag == "Player")
            {
                other.transform.parent = this.transform.parent;
                other.GetComponent<ForceController>().moveEnabled = false;
                other.GetComponent<ForceController>().joyYControllsPitch = true;

                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<Rigidbody>().isKinematic = true;
                Boid boid = Utilities.FindBoidInHierarchy(this.gameObject);
                FindObjectOfType<ViveController>().boid = boid;
                boid.GetComponent<PlayerSteering>().Activate(true);
                boid.GetComponent<Harmonic>().Activate(true);
                boid.GetComponent<Harmonic>().auto = false;

                HarmonicController hc = boid.GetComponent<HarmonicController>();
                if (boid.GetComponent<HarmonicController>() != null)
                {
                    hc.enabled = false;
                    boid.GetComponent<Harmonic>().amplitude = hc.initialAmplitude;
                    boid.GetComponent<Harmonic>().speed = hc.initialSpeed;
                }
                
                Constrain con = boid.GetComponent<Constrain>();
                if (con != null)
                {
                    con.Activate(false);
                }
                boid.GetComponent<NoiseWander>().Activate(false);
                
                RotateMe r = GetComponent<RotateMe>();
                if (r != null)
                {
                    r.speed = 0;
                }
                boid.damping = 0.01f;
                Debug.Log(boid);
            }
        }

        void OnTriggerStay(Collider c)
        {
            GameObject other = c.gameObject;
            if (other.tag == "Player")
            {
                other.transform.position = Vector3.Lerp(other.transform.position, this.transform.position, Time.deltaTime);

                // Dont do this in VR!
                if (!viveControllers)
                {
                    GameObject parent = this.transform.parent.gameObject;
                    ForceController fc = other.GetComponent<ForceController>();

                    if (!fc.rotating)
                    {
                        fc.desiredRotation = Quaternion.Slerp(fc.desiredRotation, parent.transform.rotation, Time.deltaTime * 1.5f);
                    }
                    //other.transform.rotation = Quaternion.Slerp(other.transform.rotation, parent.transform.rotation, Time.deltaTime);
                    //other.transform.forward = Vector3.forward;
                }
            }
        }
    }
}
