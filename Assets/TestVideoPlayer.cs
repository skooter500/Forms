using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVideoPlayer : MonoBehaviour {

    public RenderTexture texture;
    public Camera camera;

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        UnityEngine.Video.VideoPlayer vp = GetComponent<UnityEngine.Video.VideoPlayer>();
        vp.url = "../vjvids/gf1.mp4";
        vp.frame = 0;
        vp.isLooping = true;
        //vp.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
        //vp.targetCamera = camera;
        vp.Play();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
