using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnellItem : MonoBehaviour {

   
    private Transform control;

    private void Awake ()
    {
		control = transform.GetChild(0);
	}
    //positioning an item requires a pipe, a curve rotation, and a ring rotation
    public void Position (Wormhole pipe, float curveRotation, float ringRotation)
    {
		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

        control.localPosition = new Vector3(0f, pipe.CurveRadius);
		control.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
	}
}