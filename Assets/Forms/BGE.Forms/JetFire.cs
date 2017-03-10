using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class JetFire : MonoBehaviour {

        public float fire = 0;
        public float speed = 1.0f;
        private Vector3 maxScale;
        // Use this for initialization
        void Start () {
            maxScale = transform.localScale;
        }
	
        // Update is called once per frame
        void Update () {
            Vector3 newScale = transform.localScale;
            newScale = Vector3.Lerp(newScale, maxScale * fire, Time.deltaTime * speed * 2);
            transform.localScale = newScale;
        }
    }
}