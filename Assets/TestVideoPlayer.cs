using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TestVideoPlayer : MonoBehaviour {

    public RenderTexture texture;
    public Camera camera;

    public string path = "../vjvids/";

    [HideInInspector]
    public List<string> videos = new List<string>();

    void LoadVideoFileNames()
    {
        DirectoryInfo info = new DirectoryInfo(path);
        if (!info.Exists)
        {
            Directory.CreateDirectory(path);
        }
        FileInfo[] fileInfo = info.GetFiles();
        int last = 0;
        foreach (FileInfo file in fileInfo)
        {
            videos.Add(file.Name);
        }
    }

	// Use this for initialization
	void Start () {
	    vp =  GetComponent<UnityEngine.Video.VideoPlayer>();
        LoadVideoFileNames();
    }

    public void PlayVideo(int index)
    {
        Debug.Log("Playing video: " + index + " path: " + vp.url);
        vp.url = path + videos[index];
        vp.frame = 0;
        vp.Prepare();
        vp.isLooping = true;
        //vp.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
        //vp.targetCamera = camera;
        vp.Play();
    }

    public void OnDisable()
    {
        vp.Stop();
    }

    UnityEngine.Video.VideoPlayer vp;
    

    // Update is called once per frame
    void Update () {
        BGE.Forms.CreatureManager.Log("Videos: " + videos);
	}
}
