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
        // Use this for initialization
        void Start() {
            renderers = GetComponentsInChildren<Renderer>();
            boids = GetComponentsInChildren<Boid>();
            animators = GetComponentsInChildren<Animator>();
            spineAnimators = GetComponentsInChildren<SpineAnimator>();
        }

        public float visibleBehindDistance = 1000;
        public float visibleInFrontDistance = 8000;

        // Update is called once per frame
        void Update() {
            visible = false;
            // Break as soon as one is viaible
            int maxNumToCheck = 20;
            int numToCheck = (renderers.Length < maxNumToCheck) ? renderers.Length : maxNumToCheck;
            int gap = renderers.Length / numToCheck;

            Transform cam = Camera.main.transform;
            float distToPlayer = Vector3.Distance(boids[0].position, cam.position);

            if (Vector3.Dot(boids[0].position - cam.position, cam.forward) > 0)
            {
                visible = (distToPlayer < visibleInFrontDistance);
            }
            else
            {
                visible = (distToPlayer < visibleBehindDistance);
            }

            
            /*
            for (int i = 0; i < numToCheck; i += gap)
            {
                Renderer r = renderers[i];
                if (r.isVisible)
                {
                    visible = true;
                    break;
                }
            }
            */
            CreatureManager.Log("Visible:" + visible);

            foreach (Boid b in boids)
            {
                b.suspended = !visible;
            }

            foreach (SpineAnimator sa in spineAnimators)
            {
                sa.suspended = !visible;
            }

            foreach (FishParts fp in fishParts)
            {
                fp.suspended = !visible;
            }


        }
    }
}
