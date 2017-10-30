using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{
    public class SuspendIfInvisible : MonoBehaviour {

        public Renderer[] renderers;
        public Boid[] boids;
        public Animator[] animators;
        public SpineAnimator[] spineAnimators;
        public FishParts[] fishParts;

        public bool visible = false;

        public int updatesPerSecond = 10;

        // Use this for initialization
        void Start() {
            renderers = GetComponentsInChildren<Renderer>();
            boids = GetComponentsInChildren<Boid>();
            animators = GetComponentsInChildren<Animator>();
            spineAnimators = GetComponentsInChildren<SpineAnimator>();
            StartCoroutine(CheckVisibility());
        }

        public float visibleBehindDistance = 1000;
        public float visibleInFrontDistance = 8000;
        

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
                    visibleThisFrame = (distToPlayer < visibleInFrontDistance);
                }
                else
                {
                    visibleThisFrame = (distToPlayer < visibleBehindDistance);
                }                

                if (visibleThisFrame != visible)
                {
                    foreach (Boid b in boids)
                    {
                        b.suspended = !visibleThisFrame;
                    }

                    foreach (SpineAnimator sa in spineAnimators)
                    {
                        sa.suspended = !visibleThisFrame;
                    }

                    foreach (FishParts fp in fishParts)
                    {
                        fp.suspended = !visibleThisFrame;
                    }
                }

                visible = visibleThisFrame;
                yield return new WaitForSeconds(1.0f / (float)updatesPerSecond);
            }
        }
    }
}
