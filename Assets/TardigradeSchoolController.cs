using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class TardigradeSchoolController : CreatureController {

    
    public class IdleState : State
    {
        public override void Enter()
        {
        }
        public override void Exit() { }
    }

    class BackFlip : State
    {
        public override void Enter()
        {
            owner.GetComponent<TardigradeSchoolController>().StartBackFlipCoRoutine();
            owner.ChangeStateDelayed(new IdleState(), 5);
        }

        public override void Exit()
        {
            foreach (Boid boid in owner.GetComponent<School>().boids)
            {
                NoiseWander nw = boid.GetComponent<NoiseWander>();
                Constrain constrain = boid.GetComponent<Constrain>();
                Utilities.SetActive(nw, true);
                constrain.centre = boid.transform.position;

            }
        }
    }

    public override void Restart()
    {
        GetComponent<StateMachine>().ChangeState(new IdleState());
    }

    // Use this for initialization
    void Start () {
        Restart();
        boids = GetComponent<School>().boids;
	}

    List<Boid> boids;

    System.Collections.IEnumerator BackFlipAll()
    {
        foreach (Boid boid in boids)
        {
            NoiseWander nw = boid.GetComponent<NoiseWander>();
            Constrain constrain = boid.GetComponent<Constrain>();

            // Set the constrain target to be behind the boid
            // Project the forward vector onto the XZ plane
            Vector3 backwards = boid.transform.forward;
            backwards.y = 0;
            backwards = -backwards;
            float dist = 150;
            constrain.centre = boid.transform.position + (backwards * dist);
            constrain.centre.y += 50;
            constrain.radius = dist;
            Utilities.SetActive(nw, false);
            Utilities.SetActive(constrain, true);
            yield return new WaitForSeconds(Random.Range(0.3f, 1.0f));
        }
    }

    void StartBackFlipCoRoutine()
    {
        StartCoroutine(BackFlipAll());
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
                GetComponent<StateMachine>().ChangeState(new BackFlip());
        }
    }
}
