using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public float radius = 30;
    public int height = 20;
    public int segments = 10;
    public GameObject cubePrefab;

    void CreateTower(float radius, int height, int segments, Vector3 point)
    {
        float thetaInc = (Mathf.PI * 2.0f) / (float)segments;
        float scale = cubePrefab.transform.localScale.y;
        for (int h = 0; h < height; h++)
        {
            for (int i = 0; i < segments; i++)
            {
                float theta = thetaInc * i + (h * thetaInc * 0.5f);
                float x = radius * Mathf.Sin(theta);
                float z = radius * Mathf.Cos(theta);
                GameObject cube = GameObject.Instantiate<GameObject>(cubePrefab);
                cube.transform.rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, Vector3.up);
                cube.transform.position = point + new Vector3(x, scale * 0.5f + (h * scale * 1.2f), z);
                cube.transform.parent = this.transform;
                cube.GetComponent<Rigidbody>().mass = scale * scale * scale;
                cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1, 0.8f);
            }
        }
    }

    // Use this for initialization
    void Awake () {
        CreateTower(radius, height, segments, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
