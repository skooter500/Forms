using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using BGE.Forms;

public class LeapController : MonoBehaviour {
    public LeapProvider lp;
	// Use this for initialization
	void Start () {
        lp = FindObjectOfType<LeapProvider>();
	}
	
	// Update is called once per frame
	void Update () {
        Frame frame = lp.CurrentFrame;
        List<Hand> hands = frame.Hands;
        foreach (Hand h in hands)
        {
            CreatureManager.Log("Palm Position" + h.PalmPosition);
        }
    }
}
