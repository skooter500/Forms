using UnityEngine;
using System.Collections;
using BGE.Forms;

namespace Bge.Forms
{
    public class Lift : MonoBehaviour {

        Harmonic harmonic;
        Boid boid;
        float theta;
        // Use this for initialization
        void Start () {
            harmonic = GetComponent<Harmonic>();
            boid = GetComponent<Boid>();
        }
	
        // Update is called once per frame
        void Update () {
            theta = (harmonic.theta - (Mathf.PI / 2))  % (Mathf.PI * 2.0f);
            if (theta < Mathf.PI)
            {
                boid.position += (boid.up * Mathf.Abs(theta) * 10);
            }
        }
    }
}