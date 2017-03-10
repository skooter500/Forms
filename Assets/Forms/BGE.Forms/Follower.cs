using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class Follower : MonoBehaviour {

        [Range(0, 100)]
        public float bondDamping = 30.0f;

        [Range(0, 50)]
        public float angularBondDamping = 7.0f;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}