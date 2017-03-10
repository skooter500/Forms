using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class RotateMe : MonoBehaviour {
        public float speed = 0.1f;
        Vector3 axis;
        float lerpedSpeed = 0;
        // Use this for initialization
        void Start () {
            axis = Random.insideUnitSphere;
        }   
	
        // Update is called once per frame
        void Update () {
            lerpedSpeed = Mathf.Lerp(lerpedSpeed, speed, Time.deltaTime);
            transform.Rotate(axis, lerpedSpeed * Time.deltaTime * 360);
        }
    }
}