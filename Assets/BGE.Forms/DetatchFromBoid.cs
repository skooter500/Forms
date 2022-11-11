using UnityEngine;
using System.Collections;
using BGE.Forms;
using UnityEngine.InputSystem;

public class DetatchFromBoid : MonoBehaviour {
    public Boid boid;

    public bool escaping = false;

    // Use this for initialization
    void Start () {
	
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

        transform.rotation = Quaternion.Euler(0, cv.y, 0);
    }

    public void Detatch(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }
        //

        
        if (boid != null)
        {
            Debug.Log("Detatch!!!");
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

            StartCoroutine(StraightenUp());

            RotateMe[] r = FindObjectsOfType<RotateMe>();
            foreach (RotateMe rm in r)
            {
                rm.speed = 0.1f;
            }
            transform.parent = null;

            boid = null;

            foreach(Thruster t in GameObject.FindObjectsOfType<Thruster>())
            {
                t.readInput = true;
            }
            escaping = true;
            Invoke("StopEscaping", 5);
        }
        
    }

    void StopEscaping()
    {
        escaping = false;
    }
	
}
