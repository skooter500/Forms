using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour {
    public GameObject treePrefab;
	// Use this for initialization
	void Start () {
        for (float z = -6000; z < 6000; z += 6000)
        {
            for (float x = -6000; x < 6000; x += 6000)
            {
                GameObject tree = GameObject.Instantiate<GameObject>(treePrefab);
                tree.transform.position = transform.TransformPoint(new Vector3(x, 0, z));
                tree.SetActive(true);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
