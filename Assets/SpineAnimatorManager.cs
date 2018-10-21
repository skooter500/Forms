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
    CopyToMeJob ctmJob;
    CopyFromMeJob cfmJob;

    JobHandle saJH;
    JobHandle ctmJH;
    JobHandle cfmJH;

    NativeArray<Vector3> pos;
    NativeArray<Quaternion> rotations;

    NativeArray<int> roots;
    NativeArray<int> boneCount;
    NativeArray<Vector3> offsets;
    NativeArray<float> bondDamping;
    NativeArray<float> angularBondDamping;

    int maxJobs = 5000;
    int maxBones = 20000;
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
        boneCount[numJobs] = sa.boneTransforms.Count + 1;
        for (int i = 0; i < sa.boneTransforms.Count; i++)
        {
            transforms.Add(sa.boneTransforms[i]);
            offsets[numBones + i + 1] = sa.offsets[i]; // No offset for the 0th
        }
        numJobs++;
        numBones += sa.boneTransforms.Count + 1;
        return numJobs - 1;
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
        saJH.Complete();
        transforms.Dispose();
        bondDamping.Dispose();
        angularBondDamping.Dispose();
        boneCount.Dispose();
        offsets.Dispose();
        roots.Dispose();
        pos.Dispose();
        rotations.Dispose();
    }

    public void LateUpdate()
    {
        
    }

    public void FixedUpdate()
    {
        CreatureManager.Log("Spine jobs: " + numJobs);
        if (numJobs == 0)
        {
            return;
        }

        ctmJob = new CopyToMeJob()
        {
            pos = this.pos
            , rotations = this.rotations
        };

        saJob = new SpineAnimatorJob()
        {
            deltaTime = Time.deltaTime
            , offsets = this.offsets
            , bondDamping = this.bondDamping
            , angularBondDamping = this.angularBondDamping
            , boneCount = this.boneCount
            , roots = this.roots
            , pos = this.pos
            , rotations = this.rotations
        };

        cfmJob = new CopyFromMeJob()
        {
            pos = this.pos
            , rotations = this.rotations
            , roots = this.roots
            , root = 0
        };

        ctmJH = ctmJob.Schedule(transforms);
        saJH = saJob.Schedule(numJobs, 1, ctmJH);
        cfmJH = cfmJob.Schedule(transforms, saJH);
        cfmJH.Complete();
    }
}

public struct CopyToMeJob : IJobParallelForTransform
{
    public NativeArray<Vector3> pos;
    public NativeArray<Quaternion> rotations;
    
    public void Execute(int i, TransformAccess t)
    {
        pos[i] = t.position;
        rotations[i] = t.rotation;
    }
}

public struct CopyFromMeJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> roots;
    public NativeArray<Vector3> pos;
    public NativeArray<Quaternion> rotations;

    public int root;
    
    public void Execute(int i, TransformAccess t)
    {
        if (i == roots[root])
        {
            root++;
        }
        else
        {
            t.position = pos[i];
            t.rotation = rotations[i];
        }
    }
}

public struct SpineAnimatorJob : IJobParallelFor
{
    public NativeArray<int> roots;
    public NativeArray<int> boneCount;

    
    public NativeArray<float> bondDamping;
    public NativeArray<float> angularBondDamping;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> pos;

    [NativeDisableParallelForRestriction]
    public NativeArray<Quaternion> rotations;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> offsets;

    public float deltaTime;

    public void Execute(int j)
    {
        //
        int root = roots[j];
        for (int i = 1; i < boneCount[j]; i++)
        {
            
            Vector3 bondOffset = offsets[root + i];
            Vector3 wantedPosition = rotations[root + i - 1] * bondOffset + pos[root + i - 1]; // Transform no scale
            pos[root + i] = Vector3.Lerp(pos[root + i], wantedPosition, deltaTime * bondDamping[j]);
            
            rotations[root + i] = Quaternion.Slerp(
                rotations[root + i]
                , Quaternion.LookRotation(pos[root + i - 1] - pos[root + i], rotations[root + i - 1] * Vector3.up)
                , deltaTime * angularBondDamping[j]
                );
        }
    }
}




