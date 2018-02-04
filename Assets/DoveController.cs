using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class DoveController : CreatureController {

    public class Travel : State
    {
        Boid boid;
        float close = 500;
        Seek seek;

        public override void Enter()
        {
            Vector3 pos = Camera.main.transform.position + (Random.insideUnitSphere * 5000);
            WorldGenerator wg = GameObject.FindObjectOfType<WorldGenerator>();
            //SpawnParameters sp = owner.GetComponent<SpawnParameters>();
            pos.y = wg.SamplePos(pos.x, pos.z) + Random.Range(owner.GetComponent<CreatureController>().minHeight, owner.GetComponent<CreatureController>().maxHeight);
            boid = Utilities.FindBoidInHierarchy(owner.gameObject);
            seek = boid.GetComponent<Seek>();
            seek.SetActive(true);
            boid.GetComponent<Seek>().target = pos;
        }

        public override void Think()
        {
            // Give a new target in the same direction!
            if (Vector3.Distance(seek.target, boid.position) < 1000)
            {
                Vector3 newtarget = seek.target
                    + ((seek.target - boid.position).normalized * 100000);
                seek.target = newtarget;
            }
        }
    }

    public override void Restart()
    {
        GetComponent<StateMachine>().ChangeState(new Travel());
    }

    // Use this for initialization
    void Start () {
        Restart();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
