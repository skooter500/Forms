using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParameters : MonoBehaviour {
    public float start = 3000;
    public float end = 5000;
    public float minHeight = 500;
    public float maxHeight = 2000;
    public float minDistanceFromSurface = 1000;

    public string Species;

    [HideInInspector]
    public GameObject prefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
