using UnityEngine;
using System.Collections;

public class TargetUpdater : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target.transform.position = transform.position;
        }
    }
}
