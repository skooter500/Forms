using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGE.Forms;
public class SteveController : MonoBehaviour {

    GameObject player;

    System.Collections.IEnumerator ControlSteve()
    {
        while (true)
        {
            GetComponent<NoiseWander>().Activate(true);
            GetComponent<Seek>().Activate(false);
            yield return new WaitForSeconds(Random.Range(0, 20.0f));
            Debug.Log("Seeking the player");
            GetComponent<Seek>().Activate(true);
            GetComponent<NoiseWander>().Activate(false);
            GetComponent<Seek>().targetGameObject = player;
            yield return new WaitForSeconds(Random.Range(0, 5.0f));
        }
    }

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ControlSteve());	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
