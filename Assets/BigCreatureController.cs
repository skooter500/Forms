﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

class IdleState : State
{
    public override void Enter()
    {
        owner.ChangeStateDelayed(new MoveCloseToPlayer()
            , Random.Range(30, 60)
            ); 
    }

    public override void Exit()
    {
        owner.CancelDelayedStateChange();
    }

    public override void Think()
    {
        
    }
}

class BackFlip : State
{
    Seek seek;
    NoiseWander nw;
    Constrain constrain;
    Boid boid;
    public override void Enter()
    {
        boid = Utilities.FindBoidInHierarchy(owner.gameObject);
        seek = boid.GetComponent<Seek>();
        nw = boid.GetComponent<NoiseWander>();
        constrain = boid.GetComponent<Constrain>();

        // Set the constrain target to be behind the boid
        // Project the forward vector onto the XZ plane
        Vector3 backwards = boid.transform.forward;
        backwards.y = 0;
        backwards = -backwards;
        float dist = 100;
        constrain.centre = boid.transform.position + (backwards * dist);
        constrain.radius = dist;

        Utilities.SetActive(seek, false);
        Utilities.SetActive(nw, false);
        Utilities.SetActive(constrain, true);
        owner.ChangeStateDelayed(new IdleState(), 3);
    }

    public override void Exit()
    {
        Utilities.SetActive(constrain, false);
    }
}

class CrossPlayer : State
{
    float close = 500;
    Seek seek;
    NoiseWander nw;
    Boid boid;

    public override void Enter()
    {
        Camera player = Camera.main;
        // Reflect the point in the players forward vector
        Vector3 offset = owner.transform.position - player.transform.position;
        Vector3 reflectedOffset = - Vector3.Reflect(offset, player.transform.forward);
        Vector3 pos = player.transform.position + reflectedOffset;

        pos.y = WorldGenerator.Instance.SamplePos(pos.x, pos.z) + Random.Range(owner.GetComponent<BigCreatureController>().minHeight, owner.GetComponent<BigCreatureController>().maxHeight);
        
        boid = Utilities.FindBoidInHierarchy(owner.gameObject);
        seek = boid.GetComponent<Seek>();
        seek.SetActive(true);
        nw = boid.GetComponent<NoiseWander>();
        if (nw != null)
        {
            nw.SetActive(false);
        }
        boid.GetComponent<Constrain>().SetActive(false);
        boid.GetComponent<Seek>().target = pos;
    }

    public override void Exit()
    {
        if (nw != null)
        {
            nw.SetActive(true);
        }
        boid.GetComponent<Constrain>().SetActive(true);
        boid.GetComponent<Constrain>().centre = boid.position;
        seek.SetActive(false);
    }

    public override void Think()
    {
        if (Vector3.Distance(seek.target, boid.position) < 1000)
        {
            if (owner.GetComponent<BigCreatureController>().canIdle)
            {
                owner.ChangeState(new IdleState());
            }
            else
            {
                owner.ChangeState(new MoveCloseToPlayer());
            }
        }
    }

}


class MoveCloseToPlayer : State
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
        pos.y = wg.SamplePos(pos.x, pos.z) + Random.Range(owner.GetComponent<BigCreatureController>().minHeight, owner.GetComponent<BigCreatureController>().maxHeight);
        boid = Utilities.FindBoidInHierarchy(owner.gameObject);
        seek = boid.GetComponent<Seek>();
        seek.SetActive(true);
        
        boid.GetComponent<Seek>().target = pos;
        nw = boid.GetComponent<NoiseWander>();
        if (nw != null)
        {
            nw.SetActive(false);
        }
        boid.GetComponent<Constrain>().SetActive(false);
        boid.GetComponent<Seek>().target = pos;
    }

    public override void Exit()
    {
        if (nw != null)
        {
            nw.SetActive(true);
        }
        boid.GetComponent<Constrain>().SetActive(true);
        boid.GetComponent<Constrain>().centre = boid.position;
        seek.SetActive(false);
    }

    public override void Think()
    {
        if (Vector3.Distance(seek.target, boid.position) < 1000)
        {
            owner.ChangeState(new CrossPlayer());
            //owner.ChangeState(new BackFlip());
        }
    }
}

public class BigCreatureController : MonoBehaviour {

    public bool canIdle = true;

    public float minHeight = 500;
    public float maxHeight = 2000;

    public void Restart()
    {
        GetComponent<StateMachine>().ChangeState(new MoveCloseToPlayer());
    }

    // Use this for initialization
    void Start () {
        Restart();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
