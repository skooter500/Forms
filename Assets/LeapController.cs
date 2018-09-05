using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;
using BGE.Forms;

public class LeapController : MonoBehaviour {
    public LeapProvider lp;
    Transform head;
    public LayerMask lm;
    LineRenderer lr;
	// Use this for initialization
	void Start () {
        lp = FindObjectOfType<LeapProvider>();
        head = Camera.main.transform;
        lr = GetComponent<LineRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
        Frame frame = lp.CurrentFrame;
        List<Hand> hands = frame.Hands;
        Vector3[] poss = new Vector3[2];
        foreach (Hand h in hands)
        {            
            //if (h.PalmPosition.y > head.position.y)
            {
                Vector3 pp = h.PalmPosition.ToVector3();
                Vector3 dir = pp - head.transform.position;
                dir.Normalize();
                CreatureManager.Log("Palm Position" + pp);
                CreatureManager.Log("Head pos: " + head.position);
                CreatureManager.Log("Distance: " + Vector3.Distance(head.position, pp));
                CreatureManager.Log(h.IsRight ? "Right hand in the air" : "Left hand in the air");
                Vector3 pn = h.PalmarAxis();
                CreatureManager.Log("Palmar Axis: " + pn);
                if (Vector3.Dot(pn, head.transform.forward) > 0)
                {
                    CreatureManager.Log("Facing out. Raycasting!");
                    RaycastHit rch;
                    lr.enabled = true;
                    lr.positionCount = 2;
                    lr.widthMultiplier = 0.2f;
                    poss[0] = head.position;
                    poss[1] = head.position + (dir * 10000);

                    lr.SetPositions(poss);
                    if (Physics.Raycast(head.transform.position
                        , dir
                        , out rch
                        , 100000f
                        , lm
                        )
                        )
                    {
                        CreatureManager.Log("Hit creature! " + rch.transform.gameObject);
                        Boid boid = Utilities.FindBoidInHierarchy(rch.transform.gameObject);
                        if (boid != null)
                        {
                            CreatureManager.Log("Boid: " + boid);
                            Seek seek = boid.GetComponent<Seek>();
                            NoiseWander nw = boid.GetComponent<NoiseWander>();
                            if (nw != null)
                            {
                                Utilities.SetActive(nw, false);
                            }
                            JitterWander jw = boid.GetComponent<JitterWander>();
                            if (nw != null)
                            {
                                Utilities.SetActive(jw, false);
                            }
                            Harmonic ha = boid.GetComponent<Harmonic>();
                            if (ha != null)
                            {
                                //Utilities.SetActive(ha, false);
                            }
                            Constrain con  = boid.GetComponent<Constrain>();
                            if (con != null)
                            {
                                Utilities.SetActive(con, false);
                            }

                            seek.target = pp;
                            Utilities.SetActive(seek, true);
                        }
                    }
                }
                else
                {
                    lr.enabled = false;
                }
            }
        }

    }
}
