using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class NoiseEffect : MonoBehaviour
{
    public bool useJobSystem = false;  
    public float speed  = 0.1f;
    public float scale  = 10.0f;

    float theta = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (useJobSystem)
        {
            NoiseJobManager.Instance.Add(this);
        }
    }

    public float t = 0;

    void Update()
    {
        if (! useJobSystem)
        {
            t += speed * Time.deltaTime;
            float nn = Utilities.Map(Perlin.Noise(transform.position.x + t, transform.position.y + t, transform.position.z + t ), -1, 1, 0, scale);
            Vector3 sc = new Vector3(nn, nn, nn);
            transform.localScale = sc;
        }
    }
    
}
