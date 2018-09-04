using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class OnlyMoveTowardsCamera : MonoBehaviour {
    BGE.Forms.Boid boid;
	// Use this for initialization
	void Start () {
        boid = GetComponent<Boid>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 f = boid.velocity;
        f.y = - Mathf.Abs(f.y);
        boid.velocity = f;
	}
}
