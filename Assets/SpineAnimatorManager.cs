using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;
using Unity.Jobs.LowLevel.Unsafe;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;

public class SpineAnimatorManager : MonoBehaviour
{
    TransformAccessArray transforms;
    SpineAnimatorJob saJob;
    CopyFromMeJob ctmJob;
    CopyToMeJob cfmJob;
    JobHandle animationJH;
    JobHandle copyToJH;
    JobHandle copyFromJH;

    NativeArray<Vector3> pos;
    NativeArray<Quaternion> rotations;

    NativeArray<int> roots;
    NativeArray<int> boneCount;
    NativeArray<Vector3> offsets;
    NativeArray<float> bondDamping;
    NativeArray<float> angularBondDamping;

    int maxJobs = 5000;
    int maxBones = 5000;
    int numJobs = 0;
    int numBones = 0;

    public static SpineAnimatorManager Instance = null;

    public SpineAnimatorManager()
    {
        Instance = this;
    }

    public int AddSpine(SpineAnimator sa)
    {
        roots[numJobs] = numBones;
        transforms.Add(sa.gameObject.transform);
        bondDamping[numJobs] = sa.bondDamping;
        angularBondDamping[numJobs] = sa.angularBondDamping;
        for (int i = 0; i < sa.boneTransforms.Count; i++)
        {
            transforms.Add(sa.boneTransforms[i]);
            offsets[numBones + i + 1] = sa.offsets[i];
        }
        numJobs++;
        numBones += sa.boneTransforms.Count + 1;
        return numJobs-1;
    }

    public void Awake()
    {
        roots = new NativeArray<int>(maxJobs, Allocator.Persistent);
        boneCount = new NativeArray<int>(maxJobs, Allocator.Persistent);
        bondDamping = new NativeArray<float>(maxJobs, Allocator.Persistent);
        angularBondDamping = new NativeArray<float>(maxJobs, Allocator.Persistent);
        transforms = new TransformAccessArray(maxBones);
        offsets = new NativeArray<Vector3>(maxBones, Allocator.Persistent);
        pos = new NativeArray<Vector3>(maxBones, Allocator.Persistent);
        rotations = new NativeArray<Quaternion>(maxBones, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        animationJH.Complete();
        transforms.Dispose();
        bondDamping.Dispose();
        angularBondDamping.Dispose();
        boneCount.Dispose();
        offsets.Dispose();
        roots.Dispose();
        pos.Dispose();
    }

    public void LateUpdate()
    {
        animationJH.Complete();
    }

    public void Update()
    {
        /*
        job = new SpineAnimatorJob()
        {
            deltaTime = Time.deltaTime
            , offsets = this.offsets
            , bondDamping = this.bondDamping
            , angularBondDamping = this.angularBondDamping
            , boneCount = this.boneCount
            , roots = this.roots
            , transforms = this.transforms
        };

        jh = job.Schedule<SpineAnimatorJob>(numJobs, 1);
        */
    }
}

public struct CopyToMeJob : IJobParallelForTransform
{
    public NativeArray<Vector3> pos;
    public NativeArray<Quaternion> rotations;
    public TransformAccessArray transforms;

    public void Execute(int i, TransformAccess t)
    {
        pos[i] = t.position;
        rotations[i] = t.rotation;
    }    
}

public struct CopyFromMeJob : IJobParallelForTransform
{
    public NativeArray<Vector3> pos;
    public NativeArray<Quaternion> rotations;
    public TransformAccessArray transforms;

    public void Execute(int i, TransformAccess t)
    {
        t.position = pos[i];
        t.rotation = rotations[i];
    }
}

public struct SpineAnimatorJob : IJobParallelFor
{
    public NativeArray<int> roots;
    public NativeArray<int> boneCount;
    public NativeArray<Vector3> offsets;
    public NativeArray<float> bondDamping;
    public NativeArray<float> angularBondDamping;

    public float deltaTime;

    public void Execute(int j)
    {
        int root = roots[j];
        for (int i = 1; i < boneCount[j]; i++)
        {
            Transform previous = transforms[root + i - 1];
            Transform current = transforms[root + i];
            Vector3 bondOffset = offsets[root + i];
            Vector3 wantedPosition = previous.TransformPointUnscaled(bondOffset);
            Vector3 newPos = Vector3.Lerp(current.position, wantedPosition, deltaTime * bondDamping[j]);
            current.transform.position = newPos;
            Quaternion wantedRotation;
            Quaternion newRotation = Quaternion.identity;
            wantedRotation = Quaternion.LookRotation(previous.position - newPos, previous.up);

            current.transform.rotation = Quaternion.Slerp(
                current.transform.rotation
                , wantedRotation
                , deltaTime * angularBondDamping[j]
                );
        }
    }
}




