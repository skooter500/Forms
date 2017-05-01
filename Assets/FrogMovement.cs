﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{
	public class FrogMovement: SteeringBehaviour
	{
		[Range(0,15)]
		public float drop_speed = 10.0f;

		[Range(0,60)]
		public float body_roate_range = 30.0f;

		[Range(1,10)]
		public float jump_height = 1.0f;

		[Range(1,10)]
		public float jump_length = 1.0f;

		public AnimationCurve curve;

		private float time_delta = 0.001f;
		private GameObject body;
		private float roate_degree;
		private Vector3 target;
		private WorldGenerator wg;
		private bool on_ground = false;
		public Vector3 pos_jump_start;
		public Vector3 pos_jump_in_process;
		private float drop_height;
		private bool in_loop_calculation_needed = false;

		[Range(0,2)]
		public int stage;
		private float current_stage_time = 0.0f;

		public float[] stage_time;

		public void Start()
		{
			wg = FindObjectOfType<WorldGenerator>();
			Vector3 init_position = transform.position;
			init_position.y = wg.SamplePos(init_position.x, init_position.z);
			transform.position = init_position;
			on_ground = true;
			if (stage_time.Length != 3) {
				stage_time = new float[3];
				stage_time [0] = 1.0f;
				stage_time [1] = 1.0f;
				stage_time [2] = 1.0f;
			}
			body = transform.FindChild ("FrogBody").gameObject;
		}
		public override Vector3 Calculate()
		{
			/*float ground_y = wg.SamplePos(boid.position.x, boid.position.z);
			if (boid.position.y > ground_y)
				return new Vector3 (0, -drop_speed, 0);
			else {
				if (!on_ground) {
					on_ground = true;
					boid.velocity = new Vector3 (0, 0, 0);
					boid.speed = 0.0f;
				}

				return new Vector3 (0, 0, 0);
			}*/
			return new Vector3 (0, 0, 0);
		}
		public void Update()
		{
			time_delta = Time.deltaTime;
			if (in_loop_calculation_needed) {
				float height_difference = wg.SamplePos (transform.TransformPoint (new Vector3 (jump_length, 0, 0)).x, transform.TransformPoint (new Vector3 (jump_length, 0, 0)).z) - pos_jump_start.y ;
				float adjust_end_pivot = height_difference / jump_height;
				curve.MoveKey(curve.keys.Length - 1,new Keyframe(1.0f,adjust_end_pivot));
				in_loop_calculation_needed = false;
				if (body != null) {
					body.transform.rotation =  Quaternion.AngleAxis (body_roate_range, new Vector3 (0, 0, 1));
				}
			}

			if (on_ground) {
				switch (stage) {
				case 0:
					//Rotate body part
					current_stage_time += time_delta;
					if (current_stage_time <= stage_time [0]) {
						roate_degree = time_delta * body_roate_range / stage_time [0];
						if (body != null) {
							body.transform.rotation *= Quaternion.AngleAxis (roate_degree, new Vector3 (0, 0, 1));
						}
					} else {
						stage += 1;
						current_stage_time = 0.0f;
						pos_jump_start =  boid.position;
						boid.velocity = new Vector3 (0, 0, 0);
						boid.force = new Vector3 (0, 0, 0);
						boid.speed = 0.0f;
						in_loop_calculation_needed = true;
					}
					break;

				case 1:
					
					current_stage_time += time_delta;
					if (current_stage_time <= stage_time [1]) {
						pos_jump_in_process = new Vector3 ((current_stage_time + time_delta) * jump_length / stage_time [1], jump_height * curve.Evaluate ((current_stage_time + time_delta) / stage_time [1]), 0);
						roate_degree = -time_delta * body_roate_range / stage_time [1];
						if (body != null) {
							body.transform.rotation *= Quaternion.AngleAxis (roate_degree, new Vector3 (0, 0, 1));
						}
						transform.position = pos_jump_start + pos_jump_in_process;
						boid.UpdateLocalFromTransform ();
					} else {
						Vector3 init_position = boid.position;
						drop_height = init_position.y - wg.SamplePos(init_position.x, init_position.z);
						//transform.position = init_position;
						pos_jump_start = transform.position;
						stage += 1;
						current_stage_time = 0;
						boid.velocity = new Vector3 (0, 0, 0);
						boid.force = new Vector3 (0, 0, 0);
						boid.speed = 0.0f;
						if (body != null) {
							body.transform.rotation = new Quaternion();
						}
					}
					break;

				case 2:
					current_stage_time += time_delta;
					if (current_stage_time <= stage_time [2]) {
						boid.UpdateLocalFromTransform ();
					} else {
						//Vector3 init_position = boid.position;
						//init_position.y = wg.SamplePos(init_position.x, init_position.z);
						//transform.position = init_position;
						stage = 0;
						current_stage_time = 0;
						boid.velocity = new Vector3 (0, 0, 0);
						boid.force = new Vector3 (0, 0, 0);
						boid.speed = 0.0f;
					}
					break;
				}
			}
		}
	}
}