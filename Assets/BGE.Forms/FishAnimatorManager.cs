using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;

public class FishAnimatorManager : MonoBehaviour
{
    TransformAccessArray transforms;
    FishAnimatorJob job;
    JobHandle jh;

    NativeArray<float> theta;
    NativeArray<float> angle;

    int maxJobs = 5000;
    int numJobs = 0;

    public static FishAnimatorManager Instance = null;

    public FishAnimatorManager()
    {
        Instance = this;
    }

    public void AddFish(Transform fish, float headAngle, float tailAngle)
    {
        transforms.Add(fish.GetChild(0)); // The head
        transforms.Add(fish.GetChild(2)); // The tail
        angle[numJobs] = headAngle;
        angle[numJobs + 1] = tailAngle;

        theta[numJobs] = 0;
        theta[numJobs + 1] = 0;
        
        numJobs += 2;
    }

    public void Awake()
    {
        theta = new NativeArray<float>(maxJobs, Allocator.Persistent);
        angle = new NativeArray<float>(maxJobs, Allocator.Persistent);
        transforms = new TransformAccessArray(maxJobs);
    }

    private void OnDestroy()
    {
        jh.Complete();
        transforms.Dispose();
        theta.Dispose();
        angle.Dispose();
    }

    public void LateUpdate()
    {
        jh.Complete();
    }

    public void Update()
    {
        job = new FishAnimatorJob()
        {
            deltaTime = Time.deltaTime,
            frequency = 1.0f,
            speed = 1.0f,
            theta = this.theta,
            angle = this.angle

        };

        jh = job.Schedule(transforms);
    }
}

public struct FishAnimatorJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<float> angle;

    public NativeArray<float> theta;

    public float deltaTime;
    public float frequency;
    public float speed;

    public void Execute(int i, TransformAccess t)
    {
        float rot = Mathf.Sin(theta[i]) * angle[i];
        t.localRotation = Quaternion.AngleAxis(rot, Vector3.up);
        theta[i] += speed * frequency * deltaTime * Mathf.PI * 2.0f;
    }
}




