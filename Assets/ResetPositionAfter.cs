using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class ResetPositionAfter : MonoBehaviour {

    Vector3 orig;
    public float timeToWaitStart = 3.0f;
    public float timeToWaitEnd = 6.0f;
    Boid boid;
    TrailRenderer tr;
	// Use this for initialization
	void Start () {
        orig = transform.position;
    }

    public void OnEnable()
    {
        boid = GetComponent<Boid>();
        tr = GetComponent<TrailRenderer>();
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        while (true)
        {
            boid.suspended = false;
            tr.enabled = true;
            tr.Clear();
            yield return new WaitForSeconds(Random.Range(timeToWaitStart, timeToWaitEnd));
            boid.suspended = true;
            tr.Clear();
            tr.enabled = false;
            boid.desiredPosition = orig;
            transform.position = orig;
            boid.velocity = boid.force = boid.acceleration = Vector3.zero;
            boid.position = orig;            
            //tr.time = 0;
            yield return null;                        
        }        
    }

}
