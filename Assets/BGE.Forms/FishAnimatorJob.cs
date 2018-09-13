using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;

public struct FishAnimatorJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<float> theta;

    [ReadOnly]
    public NativeArray<float> angle;

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

public class FishAnimatorManager : MonoBehaviour
{
    TransformAccessArray transforms;
    FishAnimatorJob job;
    JobHandle jh;

    NativeArray<float> theta;
    NativeArray<float> angle;

    int maxJobs = 5000;
    int numJobs = 0;

    void Start()
    {
        theta = new NativeArray<float>(maxJobs, Allocator.Persistent);
        angle = new NativeArray<float>(maxJobs, Allocator.Persistent);
        transforms = new TransformAccessArray(maxJobs);
    }

    private void OnDisable()
    {
        jh.Complete();
        transforms.Dispose();
    }

    public void LateUpdate()
    {
        jh.Complete();
    }

    public void Update()
    {
        job = new FishAnimatorJob()
        {
            deltaTime = Time.deltaTime;
            
        }
    }
}


