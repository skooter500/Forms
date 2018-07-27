using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class HarmonicController : MonoBehaviour {

        public bool modifySpeed = false;
        Harmonic harmonic;
        Boid boid;

        [HideInInspector]
        public float initialSpeed;
        float initialBoidSpeed;

        [HideInInspector]
        public float initialAmplitude;

        [Range(0, 1)]
        public float speedVariation = 0.5f;

        [Range(0, 1)]
        public float amplitudeVariation = 0.5f;

        public bool glide = false;
        public bool running = false;

        // Use this for initialization
        void Start () {
            harmonic = GetComponent<Harmonic>();
            boid = GetComponent<Boid>();
            initialBoidSpeed = boid.maxSpeed;
            initialAmplitude = harmonic.amplitude;
            initialSpeed = harmonic.speed;
        }

        public void OnEnable()
        {
            StartCoroutine("VaryWiggleInterval");
        }


        public void OnDisable()
        {
            running = false;
        }

        System.Collections.IEnumerator VaryWiggleInterval()
        {
            running = true;
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            while (running)
            {
                //Debug.Log("Accelerated");
                harmonic.enabled = true;
                harmonic.amplitude = Random.Range(initialAmplitude - (initialAmplitude * amplitudeVariation), initialAmplitude + (initialAmplitude * amplitudeVariation));
                harmonic.speed = Random.Range(initialSpeed - (initialSpeed * speedVariation), initialSpeed + (initialSpeed * speedVariation));


                if (modifySpeed)
                {
                    float variationThisTime = harmonic.speed / initialSpeed;
                    boid.maxSpeed = initialBoidSpeed * variationThisTime;
                }
                yield return new WaitForSeconds(Random.Range(3, 7));
                if (glide)
                {
                    harmonic.amplitude = initialAmplitude * 0.2f;
                    harmonic.speed = initialSpeed;
                    yield return new WaitForSeconds(Random.Range(3, 7));
                }                
            }
        }
    }
}