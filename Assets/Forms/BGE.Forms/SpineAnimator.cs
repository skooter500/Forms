using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class SpineAnimator : MonoBehaviour {

        public Transform boneParentForAutoAssiging;
        public enum AlignmentStrategy { LookAt, AlignToHead, LocalAlignToHead }
        public AlignmentStrategy alignmentStrategy = AlignmentStrategy.LookAt;
        public bool autoAssignBones = true;    

        public List<GameObject> bones = new List<GameObject>();

        List<Vector3> bondOffsets = new List<Vector3>();
        List<Quaternion> startRotations = new List<Quaternion>();

        public List<JointParam> jointParams = new List<JointParam>();

        public float bondDamping = 10;
        public float angularBondDamping = 12;

        [HideInInspector]
        public Vector3 centerOfMass;
        [HideInInspector]
        public Quaternion averageRotation;

        public bool suspended = false;

        public enum JointType { Lerping, Physics };
        public JointType jointType = JointType.Lerping;

        private Boid boid;
        private float time = 0;

        void Start()
        {
            Transform prevFollower;
            bondOffsets.Clear();

            if (autoAssignBones)
            {
                bones.Clear();
                startRotations.Add(transform.rotation);
                Transform parent;
                if (boneParentForAutoAssiging != null)
                {
                    parent = boneParentForAutoAssiging;
                }
                else
                {
                    parent = (transform.parent.childCount > 1) ? transform.parent : transform.parent.parent;
                }
                for (int i = 0; i < parent.childCount; i++)
                {
                    GameObject child = parent.GetChild(i).gameObject;
                    if (child != this.gameObject)
                    {
                        bones.Add(child);
                        startRotations.Add(child.transform.rotation);
                    }
                }
            }

            if (jointType == JointType.Lerping)
            {
                for (int i = 0; i < bones.Count; i++)
                {
                    if (i == 0)
                    {
                        prevFollower = this.transform;
                    }
                    else
                    {
                        prevFollower = bones[i - 1].transform;
                    }

                    Transform follower = bones[i].transform;
                    Vector3 offset = follower.position - prevFollower.position;
                    offset = Quaternion.Inverse(prevFollower.transform.rotation) * offset;
                    bondOffsets.Add(offset);
                }
            }
            else
            {
                for (int i = 0; i < bones.Count; i++)
                {

                }
            }

            boid = Utilities.FindBoidInHierarchy(this.gameObject);
        }

        public void OnEnable()
        {
            //StartCoroutine(Animate());
        }

        System.Collections.IEnumerator Animate()
        {
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                //Integrate();
                /*
                float toWait = Utilities.Map(boid.distanceToPlayer, 0, 10000, 0.02f, 0.05f);
                time = toWait;
                yield return new WaitForSeconds(toWait);
                */

                time = Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

                /*
                if ((boid.inFrontOfPlayer && boid.distanceToPlayer < 10000)
                    || (!boid.inFrontOfPlayer && boid.distanceToPlayer < 2000))
                {
                    time = Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                else
                {
                    time = 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }
                */
            }
        }

        int skippedFrames = 0;
        
        public void FixedUpdate()
        {
            if (suspended || jointType == JointType.Physics)
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
            centerOfMass = Vector3.zero ; 
            Transform prevFollower;
            for (int i = 0 ; i < bones.Count; i++)
            {
                if (i == 0)
                {
                    prevFollower = this.transform;
                }
                else
                {
                    prevFollower = bones[i - 1].transform;
                }

                Transform follower = bones[i].transform;

                DelayedMovement(prevFollower, follower, bondOffsets[i], i);
                centerOfMass += follower.position;
            }
            centerOfMass /= bones.Count;
        }
    
        void DelayedMovement(Transform target, Transform follower, Vector3 bondOffset, int i)
        {
            float bondDamping;
            float angularBondDamping;
            

            if (jointParams.Count > i)
            {
                JointParam jp = jointParams[i];
                bondDamping = jp.bondDamping;
                angularBondDamping = jp.angularBondDamping;
            }
            else
            {
                bondDamping = this.bondDamping;
                angularBondDamping = this.angularBondDamping;
            }


            Vector3 wantedPosition = target.transform.TransformPointUnscaled(bondOffset);
            Vector3 newPos = Vector3.Lerp(follower.transform.position, wantedPosition, time * bondDamping);
            follower.transform.position = newPos;
            Quaternion wantedRotation;
            Quaternion newRotation = Quaternion.identity;
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
                case AlignmentStrategy.LocalAlignToHead:
                    wantedRotation = target.transform.localRotation;
                    follower.transform.localRotation = Quaternion.Slerp(follower.transform.localRotation, wantedRotation, time * angularBondDamping);
                    break;
            }
            //follower.SetPositionAndRotation(newPos, newRotation);
        }
    }

    [System.Serializable]
    public class JointParam
    {
        public float bondDamping = 25;
        public float angularBondDamping = 2;    

        public JointParam(float bondDamping, float angularBondDamping)
        {
            this.bondDamping = bondDamping;
            this.angularBondDamping = angularBondDamping;
        }
    }
}