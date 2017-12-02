using UnityEngine;
using System.Collections;
using BGE.Forms;

public class DetatchFromBoid : MonoBehaviour {

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        CreatureManager.Log("" + Input.GetKeyDown(KeyCode.JoystickButton0));
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)
            || ViveController.Instance.GetGrip())
            
        {
            Boid boid = Utilities.FindBoidInHierarchy(this.gameObject);
            
            if (boid != null)
            {
                GetComponent<ForceController>().moveEnabled = true;
                GetComponent<ForceController>().joyYControllsPitch = false;
                boid.GetComponent<Harmonic>().Activate(true);
                boid.GetComponent<Harmonic>().auto = true;
                if (boid.GetComponent<NoiseWander>() != null)
                {
                    boid.GetComponent<NoiseWander>().Activate(true);
                }

                if (boid.GetComponent<JitterWander>() != null)
                {
                    boid.GetComponent<JitterWander>().Activate(true);
                }
                boid.GetComponent<PlayerSteering>().Activate(false);
                boid.maxSpeed = boid.GetComponent<PlayerSteering>().maxSpeed;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<ForceController>().enabled = true;

                FindObjectOfType<ViveController>().boid = null;

                if (boid.GetComponent<Seek>() != null)
                {
                    boid.GetComponent<Seek>().Activate(true);
                }

                Quaternion desired = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                this.transform.parent = null;
                GetComponent<ForceController>().desiredRotation = desired;
                VaryTenticles vt = boid.transform.parent.GetComponent<VaryTenticles>();
                if (vt != null)
                {
                    vt.Vary();
                }

            }
            RotateMe[] r = FindObjectsOfType<RotateMe>();
            foreach (RotateMe rm in r)
            {
                rm.speed = 0.1f;
            }
        }
	}
}
