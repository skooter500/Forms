using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;

public class SpineAnimatorManager : MonoBehaviour
{
    TransformAccessArray transforms;
    SpineAnimatorJob job;
    JobHandle jh;

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
        transforms[numBones] = sa.gameObject.transform;
        bondDamping[numBones] = sa.bondDamping;
        angularBondDamping[numBones] = sa.angularBondDamping;
        for (int i = 0; i < sa.boneTransforms.Count; i++)
        {
            transforms[numBones + i + 1] = sa.boneTransforms[i];
            offsets[numBones + i + 1] = sa.offsets[i];
        }
        numJobs++;
        numBones += sa.boneTransforms.Count + 1;
        return numJobs-1;
    }

    public void Awake()
    {
        roots = new NativeArray<int>(maxJobs, Allocator.Persistent);
        bondDamping = new NativeArray<float>(maxJobs, Allocator.Persistent);
        angularBondDamping = new NativeArray<float>(maxJobs, Allocator.Persistent);
        transforms = new TransformAccessArray(maxBones);
        offsets = new NativeArray<Vector3>(maxBones, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        jh.Complete();
        transforms.Dispose();
        bondDamping.Dispose();
        angularBondDamping.Dispose();
        offsets.Dispose();
    }

    public void LateUpdate()
    {
        jh.Complete();
    }

    public void Update()
    {
        job = new SpineAnimatorJob()
        {
            deltaTime = Time.deltaTime,
            transforms = this.transforms
            , offsets = this.offsets
            , bondDamping = this.bondDamping
            , angularBondDamping = this.angularBondDamping
            , roots = this.roots
        };

        jh = job.Schedule<SpineAnimatorJob>(numJobs, 1);
    }
}

public struct SpineAnimatorJob : IJobParallelFor
{
    public TransformAccessArray transforms;
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
            Transform current = transforms[root + i - 1];
            Vector3 bondOffset = offsets[root + i];
            Vector3 wantedPosition = previous.TransformPointUnscaled(bondOffset);
            Vector3 newPos = Vector3.Lerp(current.position, wantedPosition, deltaTime * bondDamping[i]);
            current.transform.position = newPos;
            Quaternion wantedRotation;
            Quaternion newRotation = Quaternion.identity;
            wantedRotation = Quaternion.LookRotation(previous.position - newPos, previous.up);

            current.transform.rotation = Quaternion.Slerp(
                current.transform.rotation
                , wantedRotation
                , deltaTime * angularBondDamping[i]
                );
        }
    }
}




