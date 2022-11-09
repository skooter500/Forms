using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class BoidRider : MonoBehaviour
    {        
        bool vrMode = false;

        [HideInInspector]
        PlayerSteering ps;

        // Use this for initialization
        void Start()
        {
            vrMode = true; //  UnityEngine.XR.XRDevice.isPresent;
        }


        Quaternion targetQuaternion = Quaternion.identity;

        public void OnTriggerEnter(Collider c)
        {
            GameObject other = c.gameObject;
            if (other.tag == "Player" && PlayerController.Instance.controlType == PlayerController.ControlType.Player)
            {                
                
                Boid boid = Utilities.FindBoidInHierarchy(this.gameObject);
                other.transform.parent = this.transform.parent;
                other.GetComponent<ForceController>().moveEnabled = false;
                other.GetComponent<ForceController>().attachedToCreature = true;

                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<Rigidbody>().isKinematic = true;
                FindObjectOfType<ViveController>().boid = boid;
                FindObjectOfType<OculusController>().boid = boid;
                ps = boid.GetComponent<PlayerSteering>();
                ps.SetActive(true);
                ps.hSpeed = 1.0f;
                boid.GetComponent<Harmonic>().SetActive(true);
                boid.GetComponent<Harmonic>().auto = false;

                if (boid.GetComponent<Seek>() != null)
                {
                    boid.GetComponent<Seek>().SetActive(false);
                }
                HarmonicController hc = boid.GetComponent<HarmonicController>();
                if (boid.GetComponent<HarmonicController>() != null)
                {
                    hc.enabled = false;
                    boid.GetComponent<Harmonic>().amplitude = hc.initialAmplitude;
                    boid.GetComponent<Harmonic>().speed = hc.initialSpeed;
                }

                VaryTenticles vt = boid.transform.parent.GetComponent<VaryTenticles>();
                if (vt != null)
                {
                    vt.UnVary();
                }
                
                Constrain con = boid.GetComponent<Constrain>();
                if (con != null)
                {
                    con.SetActive(false);
                }

                if (boid.GetComponent<NoiseWander>() != null)
                {
                    boid.GetComponent<NoiseWander>().SetActive(false);
                }

                if (boid.GetComponent<JitterWander>() != null)
                {
                    boid.GetComponent<JitterWander>().SetActive(false);
                }
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
            // iF its a player and still attached
            if (other.tag == "Player" && other.transform.parent == this.transform.parent && PlayerController.Instance.controlType == PlayerController.ControlType.Player)
            {
                other.transform.position = Vector3.Lerp(other.transform.position, this.transform.position, Time.deltaTime);                
                // Dont do this in VR
                if (!vrMode)
                {
                    ForceController fc = other.GetComponent<ForceController>();
                    if (fc.cameraType == ForceController.CameraType.forward)
                    {
                        Transform parent = transform.parent;                         
                        if (!fc.rotating)
                        {
                            fc.desiredRotation = Quaternion.Slerp(fc.desiredRotation, parent.rotation, Time.deltaTime * 1.5f);
                        }
                    }
                }                
            }
        }
    }
}
