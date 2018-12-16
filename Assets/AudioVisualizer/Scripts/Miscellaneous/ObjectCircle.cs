using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// place objects perfectly in a circle.
    /// </summary>
	public class ObjectCircle : MonoBehaviour {

		public List<GameObject> objects;
		public float radius;
		float angle = 0;

		void Awake()
		{
			PositionObjects ();
		}

		//position each object around this gameobject, based on rotation speed.
		void PositionObjects()
		{
			//orient the objects
			foreach(GameObject obj in objects)
			{
				obj.transform.position = this.transform.position;
				obj.transform.rotation = this.transform.rotation;
			}


			//position the object accourding to the average
			angle = 0;
			float angleStep = 360f/objects.Count;
			for(int i = 0; i < objects.Count; i++)
			{
				//angle of this object, = angle + startAngle
				float rad = (angle)*Mathf.Deg2Rad; // radians

				float x = Mathf.Cos(rad)*radius; //update x and y pos
				float y = Mathf.Sin(rad)*radius;
				//position each object around this one.
				Vector3 newPos = this.transform.position + this.transform.right*x + this.transform.up*y; 
				objects[i].transform.position = newPos;
				//orient the object
				objects[i].transform.RotateAround(objects[i].transform.position,this.transform.forward,angle-90);

				angle += angleStep;
			}
		}
	}
}
