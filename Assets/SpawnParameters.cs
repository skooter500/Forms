using System.Collections;
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

    public bool underneath = true;

    [HideInInspector]
    public GameObject Species;

    public bool singleton = false;

    BGE.Forms.Boid masterBoid;

    public GameObject[] effects;
    internal bool isSuspending = false;

    // Use this for initialization
    void Start () {
        isSuspending = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
