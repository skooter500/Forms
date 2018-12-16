using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{

    /// <summary>
    /// this class creates a sphere of objects, all with up vectors at the normal away from the sphere
    /// </summary>
	public class ObjectSphere : MonoBehaviour {

		public List<GameObject> objectsToPlace; // objects to evenly place around a sphere
		public float radius = 3.0f; //radius of hte sphere



		// Use this for initialization
		void Awake () 
		{
			PlaceObjectsAroundSphere(objectsToPlace,this.transform.position,radius);
		}

		public static void PlaceObjectsAroundSphere(List<GameObject> objects, Vector3 origin, float radius)
		{
			Vector3[] myPoints = GetPointsOnSphere(objects.Count); //get a point for each object
			//for each object
			for(int i = 0; i < objects.Count; i++)
			{
				Vector3 point = myPoints[i]; //get the position for this object
				Vector3 pos = origin + point.normalized * radius; //adjust for the radius
				Vector3 toOrigin = pos - origin; // get the vector from the origin, to the point
				
				objects[i].transform.position = pos; // place the object
				objects[i].transform.LookAt(pos + toOrigin); // rotate the object to face outward from the sphere
			}
		}
		
		//get points, evenly spaced around a sphere
		public static Vector3[] GetPointsOnSphere(int numPoints)
		{
			Vector3[] points = new Vector3[numPoints];
			
			float increment = Mathf.PI * (3 - Mathf.Sqrt(5));
			float offset = 2f / numPoints;
			
			for (int i = 0; i < numPoints; i++)
			{
				float y = i * offset - 1 + (offset / 2);
				float r = Mathf.Sqrt(1 - y * y);
				float phi = i * increment;
				
				points[i] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
			}
			
			return points;
		}
	}
}
