﻿using UnityEngine;
using System.Collections;
using BGE.Forms;
using UnityEngine.InputSystem;

public class DetatchFromBoid : MonoBehaviour {
    public Boid boid;



    // Use this for initialization
    void Start () {
	
	}

    public void Detatch(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }
        //Debug.Log("Detatch!!!");

        
        if (boid != null)
        {
            GetComponent<ForceController>().moveEnabled = true;
            GetComponent<ForceController>().attachedToCreature = false;
            boid.GetComponent<Harmonic>().SetActive(true);
            boid.GetComponent<Harmonic>().auto = true;
            if (boid.GetComponent<NoiseWander>() != null)
            {
                boid.GetComponent<NoiseWander>().SetActive(true);
            }

            if (boid.GetComponent<JitterWander>() != null)
            {
                boid.GetComponent<JitterWander>().SetActive(true);
            }
            boid.GetComponent<PlayerSteering>().SetActive(false);
            boid.maxSpeed = boid.GetComponent<PlayerSteering>().maxSpeed;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            GetComponent<ForceController>().enabled = true;

            ViveController.Instance.boid = null;
            OculusController.Instance.boid = null;

            if (boid.GetComponent<Seek>() != null)
            {
                boid.GetComponent<Seek>().SetActive(true);
            }

            Quaternion desired = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            this.transform.parent = null;
            GetComponent<ForceController>().desiredRotation = desired;
            VaryTenticles vt = boid.transform.parent.GetComponent<VaryTenticles>();
            if (vt != null)
            {
                vt.Vary();
            }

            RotateMe[] r = FindObjectsOfType<RotateMe>();
            foreach (RotateMe rm in r)
            {
                rm.speed = 0.1f;
            }
            transform.parent = null;

            boid = null;

        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        CreatureManager.Log("" + Input.GetKeyDown(KeyCode.JoystickButton0));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)
            || ViveController.Instance.GetGrip() || OculusController.Instance.GetGrip())
            
        {
            //Detatch();
        }
	}
}
