using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class Test : MonoBehaviour {

        // Use this for initialization
        void Start () {
	
        }
        float speed = 5.0f;

        // Update is called once per frame
        void Update () {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }
}