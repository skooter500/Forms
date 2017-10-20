using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public StateMachine owner;
    public virtual void Enter() {}
    public virtual void Exit() { }
    public virtual void Think() { }
}

public class StateMachine : MonoBehaviour {

    State currentState;
    
    public int updatesPerSecond;
	// Use this for initialization
	void Start () {
        StartCoroutine(Think());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeState(State newState)
    {
        Debug.Log("Changing state to: " + newState.GetType().Name);
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.owner = this;
        currentState.Enter();
    }

    System.Collections.IEnumerator Think()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        while (true)
        {
            currentState.Think();
            yield return new WaitForSeconds(1.0f / (float)updatesPerSecond);
        }
    }    
}
