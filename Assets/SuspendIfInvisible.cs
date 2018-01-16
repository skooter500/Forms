using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{

    public struct SuspendDistance
    {
        float behind;
        float inFront;
    }

    public class SuspendIfInvisible : MonoBehaviour {

        [HideInInspector]
        public Renderer[] renderers;
        [HideInInspector]
        public Boid[] boids;
        [HideInInspector]
        public Animator[] animators;
        [HideInInspector]
        public SpineAnimator[] spineAnimators;
        [HideInInspector]
        public FishParts[] fishParts;

        public bool visible = false;

        public int updatesPerSecond = 10;

        public bool suspendBoids = false;
        public float boidsInFront, boidsBehind;

        public SuspendDistance suspendDistance;

        // Use this for initialization
        void Start() {
            renderers = GetComponentsInChildren<Renderer>();
            boids = GetComponentsInChildren<Boid>();
            animators = GetComponentsInChildren<Animator>();
            spineAnimators = GetComponentsInChildren<SpineAnimator>();            
        }

        private void OnEnable()
        {
            StartCoroutine(CheckVisibility());
        }

        // Update is called once per frame
        System.Collections.IEnumerator CheckVisibility() {
            yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
            while (true)
            {    
                Transform cam = Camera.main.transform;
                float distToPlayer = Vector3.Distance(boids[0].transform.position, cam.position);

                bool visibleThisFrame;
                if (Vector3.Dot(boids[0].transform.position - cam.position, cam.forward) > 0)
                {
                    visibleThisFrame = true;
                }
                else
                {
                    visibleThisFrame = false;
                }                

                if (visibleThisFrame != visible)
                {
                    /*
                    foreach (Boid b in boids)
                    {
                        b.suspended = !visibleThisFrame;
                    }

                    foreach (SpineAnimator sa in spineAnimators)
                    {
                        sa.suspended = !visibleThisFrame;
                    }
                    */
                }

                visible = visibleThisFrame;
                yield return new WaitForSeconds(1.0f / (float)updatesPerSecond);
            }
        }
    }
}
