using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class ForceController : MonoBehaviour {
        public Camera headCamera;
        public float speed = 10.0f;
        public bool vrMode;

        public bool lookEnabled = true;
        public bool moveEnabled = true;
     
        Rigidbody rigidBody;

        [HideInInspector]
        public bool rotating = false;

        public ForceController()
        {
            vrMode = true;
        }

        // Use this for initialization
        void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.freezeRotation = true;        

            desiredRotation = transform.rotation;
        }

        public Quaternion desiredRotation;

        void Yaw(float angle)
        {
            //rigidBody.AddTorque(Vector3.up * angle * 150);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            desiredRotation = rot * desiredRotation;
            //transform.rotation = rot * transform.rotation;
        }

        void Roll(float angle)
        {
            //Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            //transform.rotation = rot * transform.rotation;

            Quaternion rot = Quaternion.AngleAxis(angle, transform.forward);
            desiredRotation = rot * desiredRotation;

        }

        void Pitch(float angle)
        {
            float invcosTheta1 = Vector3.Dot(transform.forward, Vector3.up);
            float threshold = 0.95f;
            if ((angle > 0 && invcosTheta1 < (-threshold)) || (angle < 0 && invcosTheta1 > (threshold)))
            {
                return;
            }
        
            // A pitch is a rotation around the right vector
        
            Quaternion rot = Quaternion.AngleAxis(angle, transform.right);
            desiredRotation = rot * desiredRotation;

        
            //Quaternion rot = Quaternion.AngleAxis(angle, transform.right);
            //transform.rotation = rot * transform.rotation;
        
    
        }

        void Walk(float units)
        {
            if (headCamera != null)
            {
                rigidBody.AddForce(headCamera.transform.forward* units);
            }
            else
            {
                rigidBody.AddForce(transform.forward* units);
            }
        }

        void Fly(float units)
        {
            rigidBody.AddForce(Vector3.up * units);
        }

        void Strafe(float units)
        {
            if (headCamera != null)
            {
                rigidBody.AddForce(headCamera.transform.right* units);
            }
            else
            {
                rigidBody.AddForce(transform.right * units);
            }
        }

        int test = 0;

        // Update is called once per frame
        void FixedUpdate()
        {

            rotating = false;
            float mouseX, mouseY;
            float contSpeed = this.speed;

            float runAxis = Input.GetAxis("Fire1");
            float angularSpeed = 60.0f;

            if (Input.GetKey(KeyCode.LeftShift) || runAxis != 0)
            {
                contSpeed *= 2f;
            }

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0)
            {
                Yaw(mouseX * Time.deltaTime * angularSpeed);
            }
            else if (mouseY != 0)
            {
                Pitch(-mouseY * Time.deltaTime * angularSpeed);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime);

            //float joyX = Input.GetAxis("Joy X");
            //float joyY = Input.GetAxis("Joy Y");

            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                Fly(10 * contSpeed * Time.deltaTime);
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                Fly(- contSpeed * Time.deltaTime);
            }

            //Yaw(joyX * angularSpeed * Time.deltaTime);
            //Fly(-joyY * contSpeed * Time.deltaTime);

            float contWalk = Input.GetAxis("Vertical");
            float contStrafe = Input.GetAxis("Horizontal");
            if (Mathf.Abs(contWalk) > 0.1f && moveEnabled)
            {
                Walk(contWalk * contSpeed * Time.deltaTime);
            }
            if (Mathf.Abs(contStrafe) > 0.1f && moveEnabled)
            {
                Strafe(contStrafe * contSpeed * Time.deltaTime);
            }      
        }
    }
}