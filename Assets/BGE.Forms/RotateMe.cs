using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class RotateMe : MonoBehaviour {
        public float speed = 0.1f;
        float lerpedSpeed = 0;
        public Vector3 axis1 = Vector3.up;
        // Use this for initialization
        void Start () {
        }   
	
        // Update is called once per frame
        void Update () {
            lerpedSpeed = Mathf.Lerp(lerpedSpeed, speed, Time.deltaTime);
            transform.Rotate(axis1, lerpedSpeed * Time.deltaTime * 360);
        }
    }
}