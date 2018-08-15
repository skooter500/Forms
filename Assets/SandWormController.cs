using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormController : CreatureController
{
    public override void Restart()
    {
        GetComponent<SandWorm>().moving = false;
        GetComponent<SandWorm>().Restart();
        Invoke("StartMoving", 10);
    }

    public void StartMoving()
    {
        GetComponent<SandWorm>().moving = true;
    }

    // Use this for initialization
    void Start()
    {
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
