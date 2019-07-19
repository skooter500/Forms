using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewToad : MonoBehaviour {

    public void Toad()
    {
        if (toading)
        {
            return;
        }
        toading = true;
        targetFOV = (targetFOV == startFOV) ? toadFOV : startFOV;
        StartCoroutine(ToadCoroutine());
    }

    bool toading = false;
    float targetFOV;
    public Camera cam;

    System.Collections.IEnumerator ToadCoroutine()
    {
        while (Mathf.Abs(targetFOV - cam.fieldOfView) > 0.01f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime);
            yield return null;
        }
        cam.fieldOfView = targetFOV;
        toading = false;
    }

    private float startFOV;
    public float toadFOV = 170;

    // Use this for initialization
    void Start () {
        //cam = Camera.main;
        startFOV = cam.fieldOfView;
        targetFOV = startFOV;
    }
	
}
