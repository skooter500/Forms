using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSpawner : MonoBehaviour
{
    public int size = 10;
    public GameObject prefab;

    public float gap = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        int half = size / 2;
        for (int x = -half ; x < half ; x ++)
        {
            for (int y = -half ; y < half ; y ++)
            {
                for (int z = -half ; z < half ; z ++)
                {
                    GameObject cube = GameObject.Instantiate<GameObject>(prefab);
                    Vector3 pos = new Vector3(x * (1 + gap), y * (1 + gap), z * (1 + gap));
                    pos = transform.TransformPoint(pos);
                    cube.transform.position = pos;
                    cube.transform.rotation = transform.rotation;

                    cube.transform.parent = this.transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
