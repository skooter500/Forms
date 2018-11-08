using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {

    public float angle = 20.0f;
    public float frequency = 1.0f;
    public float colorSpeed = 1f;

    private Vector3 axis;

    private float t = 0;
    private Renderer[] rs;

	// Use this for initialization
	void Start () {
        axis = Random.insideUnitSphere;
        axis.y = 0;
        axis.Normalize();
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            colorSpeed = -colorSpeed;
        }
        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            frequency = -frequency;
        }
        if (angle > 0)
        {
            SwayManager.Instance.Add(this.transform, axis);
        }
    }

    System.Collections.IEnumerator ColorChange()
    {
        //yield return new WaitForSeconds(5);
        rs = GetComponentsInChildren<Renderer>();
        float off = 0;

        while (true)
        {
            foreach (Renderer r in rs)
            {
                r.material.SetFloat("_Offset", off * 150);
            }
            off += colorSpeed * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ColorChange());
    }

    // Update is called once per frame
    void Update () {

        /*
        
        if (angle != 0)
        {
            //axis = Quaternion.Euler(0, t * 0.02f, 0) * axis;        
            transform.localRotation = Quaternion.AngleAxis(
                //BGE.Forms.Utilities.Map(Mathf.PerlinNoise(t, 0), 0, 1, -angle, angle)
                BGE.Forms.Utilities.Map(Mathf.Sin(t), -1, 1, -angle, angle)
                , axis
                );
        }
        t += frequency * Time.deltaTime * Mathf.PI * 2.0f;        

    */
	}
}
