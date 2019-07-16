﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParameters : MonoBehaviour {
    public float start = 3000;
    public float end = 5000;
    public float minHeight = 500;
    public float maxHeight = 2000;
    public float minDistanceFromSurface = 1000;

    public float viewingDistance = 500;
    public float followCameraSpeed = 80;
    public float followCameraHalfFOV = 30;

    public float radiusRequired = 0;

    [HideInInspector]
    public GameObject Species;

    public bool singleton = false;

    BGE.Forms.Boid masterBoid;

    public GameObject[] effects;
   
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
