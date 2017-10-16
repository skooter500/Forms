using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

class IdleState : State
{
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Think()
    {
        
    }
}


class FindTarget : State
{
    float close = 500;
    Seek seek;
    NoiseWander nw;
    Boid boid;
    public override void Enter()
    {
        Vector3 pos = Camera.main.transform.position + (Random.insideUnitSphere * 5000);
        WorldGenerator wg = GameObject.FindObjectOfType<WorldGenerator>();
        //SpawnParameters sp = owner.GetComponent<SpawnParameters>();
        pos.y = wg.SamplePos(pos.x, pos.z) + Random.Range(800, 3000);
        boid = Utilities.FindBoidInHierarchy(owner.gameObject);
        seek = boid.GetComponent<Seek>();
        seek.Activate(true);
        nw = boid.GetComponent<NoiseWander>();
        if (nw != null)
        {
            nw.Activate(false);
        }
        boid.GetComponent<Constrain>().Activate(false);
        boid.GetComponent<Seek>().target = pos;
    }

    public override void Exit()
    {
        if (nw != null)
        {
            nw.Activate(true);
        }
        boid.GetComponent<Constrain>().Activate(true);
        boid.GetComponent<Constrain>().centre = boid.position;
        seek.Activate(false);
    }

    public override void Think()
    {
        if (Vector3.Distance(seek.target, boid.position) < 1000)
        {
            owner.ChangeState(new IdleState());
        }
    }
}

public class BigCreatureController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<StateMachine>().ChangeState(new FindTarget());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
