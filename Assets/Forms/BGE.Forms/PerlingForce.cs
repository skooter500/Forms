using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BGE.Forms
{
	public class PerlingForce: SteeringBehaviour
	{
		[Range(0.0f, 100.0f)]
		public float radius = 10.0f;

		[Range(0.0f, 1000.0f)]
		public float jitter = 5.0f;

		[Range(0.0f, 100.0f)]
		public float distance = 15.0f;

		public bool enable_perling_force = true;

		[Range(0.0f,50.0f)]
		public float random_y_range = 5.0f;

		private Vector3 target;
		public float random_y;

		public void OnDrawGizmos()
		{
			if (isActiveAndEnabled && enable_perling_force)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine (boid.position, boid.TransformPoint (target));
			}
		}

		public void Start()
		{
			if (isActiveAndEnabled && enable_perling_force) 
			{
				target = new Vector3(0,0,0);
				random_y_range = random_y_range * Mathf.Deg2Rad;
			}
		}

		public override Vector3 Calculate()
		{
			if (enable_perling_force) {

				float perling_number = Mathf.PerlinNoise (boid.position.x, boid.position.z);
				perling_number = Mathf.Deg2Rad * perling_number * 180.0f;
				float jitterTimeSlice = jitter * boid.TimeDelta;
				//project on circle 


				float circle_x = Mathf.Sin(perling_number) * Mathf.Cos(random_y);
				float circle_z = Mathf.Cos (perling_number) * Mathf.Sin(random_y);
				float circle_y = Mathf.Cos (random_y);

				Vector3 toAdd = new Vector3(circle_x,circle_y,circle_z) * jitterTimeSlice;
				target = boid.position + toAdd;
				target.Normalize ();
				target *= radius;
				Vector3 localTarget = target + Vector3.forward * distance;
				Vector3 worldTarget = boid.TransformPoint (localTarget);
				return (worldTarget - boid.position);
			}
			else return new Vector3 (0,0,0);
		}

		public void Update()
		{
			random_y =  Random.Range(-random_y_range, random_y_range);
		}
			
	}
}