using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class ResetPosition : MonoBehaviour {

        // Use this for initialization
        void Start () {
            orig = transform.position;
        }

        Vector3 orig;
        public GameObject plane;
        // Update is called once per frame
        void Update () {
            if (transform.position.y > plane.transform.position.y + 100)
            {
                transform.position = orig;
                GetComponent<Boid>().desiredPosition = orig;
            }
        }
    }
}