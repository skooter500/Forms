using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class OculusController : MonoBehaviour {
    public Transform leftHand;
    public Transform rightHand;

    GameObject leftEngine;
    GameObject rightEngine;

    private JetFire leftJet;
    private JetFire rightJet;
    private Rigidbody rb;

    public GameObject head;
    public float maxSpeed = 250.0f;
    public float power = 1000.0f;

    public bool haptics = false;

    public static OculusController Instance;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        leftEngine = leftHand.GetChild(0).gameObject;
        leftJet = leftEngine.GetComponentInChildren<JetFire>();

        rightEngine = rightHand.GetChild(0).gameObject;
        rightJet = rightEngine.GetComponentInChildren<JetFire>();

        rb = GetComponent<Rigidbody>();
	}

    public Boid boid;

    public bool GetGrip()
    {
        if (isActiveAndEnabled)
        {
            return (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.5f || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.5f);
        }
        else
        {
            return false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.H))
        {
            haptics = !haptics;
        }
        CreatureManager.Log("Haptics: " + haptics);


        if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch))
        {
            //Vector3 pos = transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
            //Quaternion q = transform.rotation * OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            //leftEngine.transform.SetPositionAndRotation(pos, q);
            leftEngine.SetActive(true);
            float leftTrig =  OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            if (leftTrig > 0.02f)
            {
                if (boid == null)
                {
                    rb.AddForceAtPosition(leftHand.forward * power * leftTrig, leftEngine.transform.position);
                    leftJet.fire = leftTrig;
                }
                else
                {
                    boid.speed = boid.maxSpeed * leftTrig;
                    HarmonicController hc = boid.GetComponent<HarmonicController>();
                    if (hc != null)
                    {
                        boid.GetComponent<Harmonic>().speed = boid.GetComponent<HarmonicController>().initialSpeed * leftTrig;
                    }
                }
            }
            else
            {
                
                leftJet.fire = 0;
                
            }
        }
        else
        {
            leftEngine.SetActive(false);
        }
        if (OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch))
        {
            //Vector3 pos = transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
            //Quaternion q = transform.rotation * OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            //rightEngine.transform.SetPositionAndRotation(pos, q);
            rightEngine.SetActive(true);

            float rightTrig = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            if (rightTrig > 0.02f)
            {
                if (boid == null)
                {
                    rb.AddForceAtPosition(rightHand.forward * power * rightTrig, rightEngine.transform.position);
                    rightJet.fire = rightTrig;
                }
                else
                {
                    boid.speed = boid.maxSpeed * rightTrig;
                    HarmonicController hc = boid.GetComponent<HarmonicController>();
                    if (hc != null)
                    {
                        boid.GetComponent<Harmonic>().speed = boid.GetComponent<HarmonicController>().initialSpeed * rightTrig;
                    }
                }
            }
            else
            {
                rightJet.fire = 0;
            }
        }
        else
        {
            rightEngine.SetActive(false);
        }       
    }
}
