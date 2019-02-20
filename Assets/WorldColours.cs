using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldColours : MonoBehaviour {

    [Range (0, 1)]
    public float hue = 1.0f;

    [Range(0, 1)]
    public float saturation = 1.0f;

    [Range(0, 1)]
    public float brightness = 0.0f;

    float oh = 0, os  = 0, ob = 0;

    BGE.Forms.GameOfLifeTextureGenerator gol;

    // Use this for initialization
    void Start () {
        gol = FindObjectOfType<BGE.Forms.GameOfLifeTextureGenerator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (oh != hue || os != saturation || ob != brightness)
        {
            Color c = Color.HSVToRGB(hue, saturation, brightness);
            //gol.backGround = c;
            //RenderSettings.fogColor = c;
            //RenderSettings.fog = false;
            Camera[] cams = FindObjectsOfType<Camera>();
            foreach (Camera cam in cams)
            {
                cam.backgroundColor = c;
            }
        }
	}
}
