using UnityEngine;
using System.Collections;


namespace AudioVisualizer
{
    /*
 * Rotate around a target
 * */
	public class CameraCircle : MonoBehaviour {

		public Transform target;
		public float rotationSpeed;
		public Vector3 rotationAxis;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () 
		{

			//move around
			this.transform.RotateAround(target.transform.position,rotationAxis,rotationSpeed*Time.smoothDeltaTime);
			this.transform.LookAt (target);
		}
	}
}
