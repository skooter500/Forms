using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;

public class TardigradeSchoolController : MonoBehaviour {

    Boid[] boids;

    public class IdleState : State
    {
        public override void Enter() { }
        public override void Exit() { }

    }

    // Use this for initialization
    void Start () {
        boids = GetComponentsInChildren<Boid>();
        GetComponent<StateMachine>().ChangeState(new IdleState());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
