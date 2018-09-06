using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class ControllableJellyController : CreatureController
 {

    Boid boid;

    public override void Restart()
    {
        Vector3 newF = Random.insideUnitCircle;
        newF.z = newF.y;
        newF.y = 0;
        boid.transform.forward = newF;
        boid.UpdateLocalFromTransform();
    }

    // Use this for initialization
    void Start()
    {
        boid = Utilities.FindBoidInHierarchy(this.gameObject);
        Restart();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
