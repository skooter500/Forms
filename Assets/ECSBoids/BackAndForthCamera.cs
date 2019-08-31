using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ew
{
    public class BackAndForthCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        public float speed = 10;
        public float maxDist = 5000;
        public float range = 200;
        public float target;
        public Vector3 targetPos;
        void Start()
        {
            transform.position = new Vector3(0, 0, maxDist);
            targetPos = transform.position;
            transform.rotation = Quaternion.LookRotation(-transform.position);

        }

        public Quaternion targetQuaternion = Quaternion.identity;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                float dist = Random.Range(maxDist - range, maxDist + range);
                targetPos = Random.insideUnitSphere.normalized * dist;
                targetPos.y = 0;
            }
            //Vector3 pos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, targetPos) < 5)
            {
                float dist = Random.Range(maxDist - range, maxDist + range);
                targetPos = Random.insideUnitSphere.normalized * dist;
                targetPos.y *= 0.0f;                

            }
            

            //transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 3.0f, speed);
            transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);
            transform.LookAt(Vector3.zero);
            //transform.position = pos;
            //transform.forward = Vector3.Lerp(transform.forward, -transform.position, Time.deltaTime * 0.2f);

            //transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, Time.deltaTime);
            //transform.forward = Vector3.Lerp(transform.forward, -transform.position, Time.deltaTime);
        }

        Vector3 velocity = Vector3.zero;

    }
}
