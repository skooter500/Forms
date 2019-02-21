using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    public Tunnell pipeSystem;
    private float distanceTraveled;
    private float deltaToRotation;
    private float systemRotation;
    private Transform environment, control;
    public float rotationVelocity;
    private float worldRotation, viewRotation;

    public float velocity;

    private Wormhole currentPipe;
    

    private void Start()
    {
        environment = pipeSystem.transform.parent;
        control = transform.GetChild(0);
        //start at the first pipe of the system
        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();
        
    }

    private void Update()
    {
        //distance that it has traveled increases with time to change effects
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        //convert the delta into a rotation, used to update the system's orientation.
        
        systemRotation += delta * deltaToRotation;
        if (systemRotation >= currentPipe.CurveAngle)
        {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
			SetupCurrentPipe();
            systemRotation = delta * deltaToRotation;
        }
        pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
        UpdateViewRotation();
    }


    private void SetupCurrentPipe()
    {
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
        worldRotation += currentPipe.RelativeRotation;
        if (worldRotation < 0f)
        {
            worldRotation += 360f;
        }
        else if (worldRotation >= 360f)
        {
            worldRotation -= 360f;
        }
        environment.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }

    private void UpdateViewRotation()
    {
        viewRotation +=
            rotationVelocity * Time.deltaTime * Input.GetAxis("Horizontal");
        if (viewRotation < 0f)
        {
            viewRotation += 360f;
        }
        else if (viewRotation >= 360f)
        {
            viewRotation -= 360f;
        }
        control.localRotation = Quaternion.Euler(viewRotation, 0f, 0f);
    }
}
