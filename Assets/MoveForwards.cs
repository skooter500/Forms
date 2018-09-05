using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwards : MonoBehaviour {
    public float speed = 1f;
    Vector3 startPos;
	// Use this for initialization

    private void OnEnable()
    {
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(0, speed * Time.deltaTime, 0);
	}
}
