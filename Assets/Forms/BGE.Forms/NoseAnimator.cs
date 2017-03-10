using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class NoseAnimator : Animator {
        private Harmonic harmonic;
        public float theta = 0;
        float initialAmplitude;
        public float amplitude = 40.0f;

        [Range(0, 360)]
        public float rotationOffset = 220;

        [Range(0, 2)]
        public float wigglyness = 1;

        public bool xAxis = false;
        public bool yAxis = false;
        public bool zAxis = false;


        // Use this for initialization
        void Start () {
            if (boid != null)
            {
                harmonic = boid.GetComponent<Harmonic>();
                if (harmonic != null)
                {
                    initialAmplitude = harmonic.amplitude;
                    theta = harmonic.theta;
                }
            }
        }

        float angle = 1.0f;

        // Update is called once per frame
        void Update () {
            if (harmonic != null)
            {
                float offset = rotationOffset * Mathf.Deg2Rad;


                angle = Mathf.Lerp(angle, Utilities.Map(Mathf.Sin((harmonic.theta + offset)), -1.0f, 1.0f, 1.0f, amplitude), Time.deltaTime);

                transform.localScale = new Vector3(
                    xAxis ? angle : 1
                    , yAxis ? angle : 1
                    , zAxis ? angle : 1
                );
            }
        }
    }
}