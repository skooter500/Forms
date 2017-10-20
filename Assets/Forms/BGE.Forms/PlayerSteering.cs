using UnityEngine;
using System.Collections;
using System;
using BGE.Forms;

public class PlayerSteering : SteeringBehaviour {
    public float power = 500.0f;

    public float upForce;
    public float rightForce;

    private ViveController viveController;
    bool viveControllers = false;
    private Vector3 viveForce;

    private Quaternion average;

    private bool pickedUp = false;

    public void Start()
    {
        viveController = FindObjectOfType<ViveController>();
        viveControllers = UnityEngine.XR.XRDevice.isPresent;
    }

    public override void Update()
    {
        base.Update();
        upForce = - Input.GetAxis("Vertical");
        rightForce = Input.GetAxis("Horizontal");
        // Control the boid
        if (viveController != null)
        {
            if (viveController.leftTrackedObject != null && viveController.rightTrackedObject != null)
            {
                average = Quaternion.Slerp(viveController.leftTrackedObject.transform.rotation
                    , viveController.rightTrackedObject.transform.rotation, 0.5f);

                //average = viveController.leftTrackedObject.transform.rotation;

                //float viveRightForce = viveController.leftTrackedObject.transform.position.y - viveController.rightTrackedObject.transform.position.y;
                //rightForce += (viveRightForce * 3.0f);

                /*
                Quaternion average = Quaternion.Slerp(viveController.leftTrackedObject.transform.localRotation
                    , viveController.rightTrackedObject.transform.localRotation, 0.5f);
                Vector3 ypr = average.eulerAngles;
                float pitch = ypr.x;
                if (pitch > 180.0f)
                {
                    pitch = pitch - 360.0f;
                }
                pitch = Utilities.Map(pitch, -90, 90, 1, -1);
                CreatureManager.Log("Pitch: " + pitch);
                //upForce += (pitch * 0.5f);
                */

            }
        }

        //Debug.Log("Cont: " + contWalk);
    }

    public override Vector3 Calculate()
    {
        Vector3 force = (boid.right * rightForce * power)
            + (boid.up * upForce * power);
        if (viveControllers)
        {
            force += average * Vector3.forward * power;
        }
        return force;
    }
}
