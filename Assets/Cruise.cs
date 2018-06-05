using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruise : MonoBehaviour
{
    Rigidbody rb;
    public float preferredHeight = 200;
    public float height = 0;

    public float hoverForce = 1000;
    public float forwardForce = 1000;
    public LayerMask environment;
    private CameraTransitionController ctc;

    private GameObject player;

    

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        rb = GetComponent<Rigidbody>();
        Vector3 pos = transform.position;
        //preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
        ctc = FindObjectOfType<CameraTransitionController>();
    }

    public float wait = 30;
    public float forwardDistance = 1000;

    IEnumerator ControlTransitions()
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            //int 
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
        {
            enabled = !enabled;
            Vector3 pos = transform.position;
            preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
        }
        if (!enabled)
        {
            return;
        }
        */
            // Keep height
        RaycastHit rch;
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out rch, 100000, environment);
        
        height = rch.distance;
        if (rch.distance < preferredHeight - 50)
        {
            rb.AddForce(Vector3.up * hoverForce * Time.deltaTime);
        }
        if (rch.distance > preferredHeight + 50)
        {
            rb.AddForce(Vector3.down * hoverForce * Time.deltaTime);
        }

        // Check for obstacles in front
        Vector3 f = transform.forward;
        f.y = 0;
        if (Physics.Raycast(transform.position, f, forwardDistance, environment))
        {
            rb.AddForce(Vector3.up * hoverForce * Time.deltaTime * 3.0f);
        }

        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();
        rb.AddForce(forwardDir * forwardForce * Time.deltaTime);
    }
}
