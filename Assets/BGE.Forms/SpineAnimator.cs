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

        public List<Vector3> offsets = new List<Vector3>();
                
        public float bondDamping = 10;
        public float angularBondDamping = 12;
        
        public bool suspended = false;

        public Boid boid;
        private float time = 0;

        public bool useSpineAnimatorSystem = true;
        public int spineAnimatorSystemToUse = 0;

        void Start()
        {
            Transform prevFollower;
            offsets.Clear();

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
                offsets.Add(offset);
            }

            boid = Utilities.FindBoidInHierarchy(this.gameObject);

            if (useSpineAnimatorSystem)
            {
                SpineAnimatorManager.Instance.AddSpine(this, spineAnimatorSystemToUse);
            }
        }

        int skippedFrames = 0;
        
        
        public void FixedUpdate()
        {
            if (useSpineAnimatorSystem)
            {
                return;
            }
            if (suspended)
            {
                return;
            }
            if (! boid.inFrontOfPlayer && boid.distanceToPlayer > 1000 && skippedFrames < 10)
            {
                skippedFrames++;
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
            Transform previous;
            for (int i = 0 ; i < bones.Count; i++)
            {
                if (i == 0)
                {
                    previous = this.transform;
                }
                else
                {
                    previous = boneTransforms[i - 1];
                }

                Transform current = boneTransforms[i];

                DelayedMovement(previous, current, offsets[i], i);
            }
            
        }
        
    
        void DelayedMovement(Transform previous, Transform current, Vector3 bondOffset, int i)
        {
            Vector3 wantedPosition = previous.TransformPointUnscaled(bondOffset);
            Vector3 newPos = Vector3.Lerp(current.position, wantedPosition, time * bondDamping);
            current.transform.position = newPos;
            Quaternion wantedRotation;
            Quaternion newRotation = Quaternion.identity;
            switch (alignmentStrategy)
            {

                case AlignmentStrategy.LookAt:
                    wantedRotation = Quaternion.LookRotation(previous.position - newPos, previous.up);
                    current.transform.rotation = Quaternion.Slerp(current.transform.rotation, wantedRotation, time * angularBondDamping);
                    break;
                case AlignmentStrategy.AlignToHead:
                    wantedRotation = previous.transform.rotation;
                    current.transform.rotation = Quaternion.Slerp(current.transform.rotation, wantedRotation, time * angularBondDamping);
                    break;
            }
        }
    }
}
