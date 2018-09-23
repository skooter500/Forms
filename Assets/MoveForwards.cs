using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwards : MonoBehaviour {
    public float speed = 1f;
    Vector3 startPos;
    public float distance = 10;
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
        if (Vector3.Distance(startPos, transform.position) < distance)
        {
            transform.Translate(0, speed * Time.deltaTime, 0);
        }
	}
}
