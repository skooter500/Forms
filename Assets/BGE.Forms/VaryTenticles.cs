using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class VaryTenticles : MonoBehaviour {
        float initialSpeed;
        float initialAmplitude;

        [Range(0, 1)]
        public float speedVariation = 0.5f;

        [Range(0, 1)]
        public float amplitudeVariation = 0.5f;

        FinAnimator[] animators;

        // Use this for initialization
        void Start () {
            animators = GetComponentsInChildren<FinAnimator>();
            Vary();
        }

        public void Vary()
        {
            foreach (FinAnimator ani in animators)
            {
                ani.rotationOffset = Random.Range(100, 300);
                ani.amplitude = Random.Range(20, 60);
            }
        }

        public void UnVary()
        {
            foreach (FinAnimator ani in animators)
            {
                ani.rotationOffset = 0;
                ani.amplitude = 60;
            }
        }




        // Update is called once per frame
        void Update () {
	
        }
    }
}