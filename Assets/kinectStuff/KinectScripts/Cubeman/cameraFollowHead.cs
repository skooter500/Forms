using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowHead : MonoBehaviour {

    public GameObject head;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //set the position of this object to the position of an object defined in the inspector, used as a workaround to get the camera following the players head position from the kinect
        Vector3 tempPos = head.transform.position;
        transform.position = tempPos;
	}
}
