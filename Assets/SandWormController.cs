using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormController : CreatureController
{
    /*
    System.Collections.IEnumerator Control()
    {
        GetComponent<SandWorm>().moving = false;
        yield return new WaitForSeconds(10);
        while (true)
        {
            GetComponent<SandWorm>().moving = true;
            yield return new WaitForSeconds(Random.Range(10, 20));
            GetComponent<SandWorm>().moving = false;
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }
    */
    public override void Restart()
    {
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
