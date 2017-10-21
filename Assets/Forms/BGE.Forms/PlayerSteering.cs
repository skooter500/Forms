using UnityEngine;
using System.Collections;
using System;
using BGE.Forms;

public class PlayerSteering : SteeringBehaviour
{
    public float power = 500.0f;

    public float upForce;
    public float rightForce;

    private ViveController viveController;
    bool viveControllers = false;
    private Vector3 viveForce;

    private Quaternion average;

    private bool pickedUp = false;

    public enum ControlType { Ride, Tenticle };

    public ControlType controlType = ControlType.Ride;

    Harmonic harmonic;

    [HideInInspector]
    public float maxSpeed = 0;

    public void Start()
    {
        viveController = FindObjectOfType<ViveController>();
        harmonic = GetComponent<Harmonic>();
        viveControllers = UnityEngine.XR.XRDevice.isPresent;
        maxSpeed = boid.maxSpeed;
    }

    

    public override void Update()
    {
        base.Update();
        upForce = -Input.GetAxis("Vertical");
        rightForce = Input.GetAxis("Horizontal") * 0.3f;
        // Control the boid
        if (viveController != null)
        {
            if (viveController.leftTrackedObject != null && viveController.rightTrackedObject != null)
            {
                average = Quaternion.Slerp(viveController.leftTrackedObject.transform.rotation
                    , viveController.rightTrackedObject.transform.rotation, 0.5f);

                if (controlType == ControlType.Tenticle)
                {
                    //float theta = Vector3.SignedAngle(Vector3.right, average * Vector3.right, Vector3.forward);
                    //harmonic.theta = theta;
                }
            }
            
        }
        float hSpeed = Utilities.Map(Input.GetAxis("LeftTrigger") + Input.GetAxis("RightTrigger"), 0, 1, 0.1f, 1);
        harmonic.theta += hSpeed * Time.deltaTime;
        boid.maxSpeed = maxSpeed * hSpeed;
        //Debug.Log("Cont: " + contWalk);
    }

    public override Vector3 Calculate()
    {
        if (controlType == ControlType.Ride)
        {
            force = (boid.right * rightForce * power)
                + (boid.up * upForce * power);
            if (viveControllers)
            {
                force += average * Vector3.forward * power;
            }
        }
        else
        {
            force = Vector3.zero;
        }
        return force;
    }
}
