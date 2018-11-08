using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSpawner : MonoBehaviour {
    public GameObject cubePrefab;
    public GameObject wormPrefab;
    public GameObject player;

    public LayerMask groundLM;
    // Use this for initialization

    void CreateTower(float radius, int height, int segments, Vector3 point)
    {
        float thetaInc = (Mathf.PI * 2.0f) / (float)segments;

        for (int h = 0; h < height; h++)
        {
            for (int i = 0; i < segments; i++)
            {
                float theta = thetaInc * i + (h * thetaInc * 0.5f);
                float x = radius * Mathf.Sin(theta);
                float z = radius * Mathf.Cos(theta) + 20;
                GameObject cube = GameObject.Instantiate<GameObject>(cubePrefab);
                cube.transform.rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, Vector3.up);
                cube.transform.position = point + new Vector3(x, h, z);
                cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1, 0.8f);
            }
        }
    }

    void CreateWorm(Vector3 point, Quaternion q)
    {
        GameObject worm = GameObject.Instantiate<GameObject>(wormPrefab, point, q);

    }

	void Start () {        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.U))
        {
            RaycastHit rch;
            if (Physics.Raycast(player.transform.position, player.transform.forward, out rch, 100, groundLM))
            {
                Vector3 p = rch.point;
                p.y = 0.5f;
                CreateTower(3, 10, 12, p);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            RaycastHit rch;
            if (Physics.Raycast(player.transform.position, player.transform.forward, out rch, 100, groundLM))
            {
                Vector3 p = rch.point;
                p.y = 5;
                Quaternion q = player.transform.rotation;
                Vector3 xyz = q.eulerAngles;
                q = Quaternion.Euler(0, xyz.y + 90, 0);
                CreateWorm(p, q);
            }
        }       
    }
}
