using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormController : CreatureController
{
    public float distanceToPlayer = 180.0f;
    Transform sw;

    System.Collections.IEnumerator Cull()
    {
        while (true)
        {
            float sp = sw.GetComponent<Rigidbody>().velocity.magnitude;
            if (sp > 50)
            {
                mother.Suspend(GetComponent<SpawnParameters>().Species, this.gameObject);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    System.Collections.IEnumerator Control()
    {

        GetComponent<SandWorm>().moving = false;
        yield return new WaitForSeconds(10);
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<SandWorm>().moving = true;
        GetComponent<SandWorm>().current = 0;
        while (true)
        {
            if (Vector3.Distance(sw.position, player.position) < distanceToPlayer)
            {
                GetComponent<SandWorm>().moving = false;
            }
            else
            {
                GetComponent<SandWorm>().moving = true;
            }
            
            yield return new WaitForSeconds(2);
            
        }
    }

    public override void Restart()
    {
        GetComponent<SandWorm>().Restart();
        StartCoroutine(Control());
        StartCoroutine(Cull());
        //GetComponent<SandWorm>().moving = false;
        //Invoke("StartMoving", 10);
    }

    public void StartMoving()
    {
        GetComponent<SandWorm>().moving = true;
    }

    // Use this for initialization
    void Start() {
    
        sw = transform.GetChild(transform.childCount - 1);
        Restart();
    }
}
