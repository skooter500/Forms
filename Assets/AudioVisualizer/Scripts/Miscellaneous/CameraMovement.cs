using UnityEngine;
using System.Collections;



namespace AudioVisualizer
{
    /// <summary>
    /// Move the camera right.
    /// </summary>
	public class CameraMovement : MonoBehaviour {

		public float speed; // movement speed
		public float lerpSpeed; // lerp between current position and next position.

		
		// Update is called once per frame
		void Update ()
		{
			Vector3 step = this.transform.right * speed;
			this.transform.position = Vector3.Lerp(this.transform.position,this.transform.position + step,lerpSpeed*Time.smoothDeltaTime);
		}
	}
}
