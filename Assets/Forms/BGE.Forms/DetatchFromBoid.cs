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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Boid boid = Utilities.FindBoidInHierarchy(this.gameObject);
            
            if (boid != null)
            {
                GetComponent<ForceController>().moveEnabled = true;
                boid.GetComponent<Harmonic>().Activate(true);
                boid.GetComponent<NoiseWander>().Activate(true);
                boid.GetComponent<PlayerSteering>().Activate(false);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<ForceController>().enabled = true;

                Quaternion desired = transform.rotation;
                Vector3 cv = desired.eulerAngles;
                desired = Quaternion.Euler(desired.x, desired.y, 0);

                GetComponent<ForceController>().desiredRotation = desired;
                this.transform.parent = null;
            }
            RotateMe[] r = FindObjectsOfType<RotateMe>();
            foreach (RotateMe rm in r)
            {
                rm.speed = 0.1f;
            }
        }
	}
}
