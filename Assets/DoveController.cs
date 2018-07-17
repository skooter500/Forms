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
            Vector3 pos = Camera.main.transform.position + (Random.insideUnitSphere * 500);
            WorldGenerator wg = GameObject.FindObjectOfType<WorldGenerator>();
            //SpawnParameters sp = owner.GetComponent<SpawnParameters>();
            //pos.y = wg.SamplePos(pos.x, pos.z) + Random.Range(owner.GetComponent<CreatureController>().minHeight, owner.GetComponent<CreatureController>().maxHeight);
            pos.y = owner.transform.position.y;
            boid = Utilities.FindBoidInHierarchy(owner.gameObject);

            Vector3 toTarget = pos - boid.position;
            toTarget.y = 0;
            /*
            FormationGenerator fg = owner.transform.parent.GetComponent<FormationGenerator>();
            if (fg != null)
            {
                fg.transform.position = owner.transform.position;
                fg.transform.rotation = Quaternion.LookRotation(toTarget);
                fg.GeneratePositions();
                fg.Teleport();
            }
            else
            {
                
            }
            */
            owner.transform.rotation = Quaternion.LookRotation(toTarget);
            
            seek = boid.GetComponent<Seek>();
            seek.SetActive(true);
            boid.GetComponent<Seek>().target = pos;
        }

        public override void Think()
        {
            // Give a new target in the same direction!
            if (Vector3.Distance(seek.target, boid.position) < 1000)
            {
                Debug.Log("Finding new dove target");
                Vector3 newtarget = boid.position
                    + (boid.forward * 100000);
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
