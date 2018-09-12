using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;

public struct FishAnimatorJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<float> theta;

    float deltaTime;
    float speedMultiplier;
    float frequency;
    float speed;

    public float headField;
    public float tailField;


    public void Execute(int i, TransformAccess t)
    {
        float rot = (i < theta.Length / 2)
            ? Mathf.Sin(theta[i]) * headField
            : Mathf.Sin(theta[i]) * tailField;
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

    void Start()
    {
        theta = new NativeArray<float>(1000, Allocator.Persistent);
    }

    private void OnDisable()
    {
        jh.Complete();
        transforms.Dispose();
    }
}


