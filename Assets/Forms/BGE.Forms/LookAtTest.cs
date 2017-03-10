using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class LookAtTest : MonoBehaviour
    {
        Vector3 up = Vector3.up;
        Vector3 look = Vector3.forward;

        // Use this for initialization
        void Start()
        {

        }



        // Update is called once per frame
        void Update()
        {
            Debug.DrawLine(transform.position, transform.position + (up*20), Color.blue);
            Debug.DrawLine(transform.position, transform.position + (look*20), Color.blue);

            float angle = 90.0f;
            if (Input.GetKey(KeyCode.I))
            {
                up = Quaternion.AngleAxis(Time.deltaTime*angle, transform.right)*up;
                look = Quaternion.AngleAxis(Time.deltaTime*angle, transform.right)*look;
            }

            if (Input.GetKey(KeyCode.K))
            {
                up = Quaternion.AngleAxis(Time.deltaTime*angle, -transform.right)*up;
                look = Quaternion.AngleAxis(Time.deltaTime*angle, -transform.right)*look;
            }

            transform.LookAt(transform.position + look, up);
        }
    }
}