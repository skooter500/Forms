using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class SpineAnimator : MonoBehaviour {

        public bool autoAssignBones = true;

        public enum AlignmentStrategy { LookAt, AlignToHead, LocalAlignToHead }
        public AlignmentStrategy alignmentStrategy = AlignmentStrategy.LookAt;

        public List<GameObject> bones = new List<GameObject>();
        public List<Transform> boneTransforms = new List<Transform>();

        List<Vector3> bondOffsets = new List<Vector3>();
        
        
        public float bondDamping = 10;
        public float angularBondDamping = 12;

        
        public bool suspended = false;

        private Boid boid;
        private float time = 0;

        void Start()
        {
            Transform prevFollower;
            bondOffsets.Clear();

            if (autoAssignBones)
            {
                bones.Clear();
                Transform parent;
                parent = (transform.parent.childCount > 1) ? transform.parent : transform.parent.parent;
                for (int i = 0; i < parent.childCount; i++)
                {
                    GameObject child = parent.GetChild(i).gameObject;
                    if (child != this.gameObject)
                    {
                        bones.Add(child);
                        boneTransforms.Add(child.transform);
                    }
                }
            }

            for (int i = 0; i < bones.Count; i++)
            {
                if (i == 0)
                {
                    prevFollower = this.transform;
                }
                else
                {
                    prevFollower = boneTransforms[i - 1];
                }

                Transform follower = boneTransforms[i];
                Vector3 offset = follower.position - prevFollower.position;
                offset = Quaternion.Inverse(prevFollower.rotation) * offset;
                bondOffsets.Add(offset);
            }

            boid = Utilities.FindBoidInHierarchy(this.gameObject);
        }

        public void OnEnable()
        {
            //StartCoroutine(Animate());
        }

        

        int skippedFrames = 0;
        
        public void FixedUpdate()
        {
            if (suspended)
            {
                return;
            }
            if (! boid.inFrontOfPlayer && boid.distanceToPlayer > 1000 && skippedFrames < 10)
            {
                skippedFrames++;
                //CreatureManager.Log("Skipping a frame");
                return;
            }
            if (skippedFrames == 10)
            {
                
                skippedFrames = 0;
                time = Time.deltaTime * 10.0f;
            }
            else
            {
                time = Time.deltaTime;
            }
            Transform prevFollower;
            for (int i = 0 ; i < bones.Count; i++)
            {
                if (i == 0)
                {
                    prevFollower = this.transform;
                }
                else
                {
                    prevFollower = boneTransforms[i - 1];
                }

                Transform follower = boneTransforms[i];

                DelayedMovement(prevFollower, follower, bondOffsets[i], i);
            }
        }
    
        void DelayedMovement(Transform target, Transform follower, Vector3 bondOffset, int i)
        {
            Vector3 wantedPosition = target.TransformPointUnscaled(bondOffset);
            Vector3 newPos = Vector3.Lerp(follower.position, wantedPosition, time * bondDamping);
            follower.transform.position = newPos;
            Quaternion wantedRotation;
            Quaternion newRotation = Quaternion.identity;
            wantedRotation = Quaternion.LookRotation(target.position - newPos, target.up);
            switch (alignmentStrategy)
            {

                case AlignmentStrategy.LookAt:
                    wantedRotation = Quaternion.LookRotation(target.position - newPos, target.up);
                    follower.transform.rotation = Quaternion.Slerp(follower.transform.rotation, wantedRotation, time * angularBondDamping);
                    break;
                case AlignmentStrategy.AlignToHead:
                    wantedRotation = target.transform.rotation;
                    follower.transform.rotation = Quaternion.Slerp(follower.transform.rotation, wantedRotation, time * angularBondDamping);
                    break;
            }
        }
    }
}
