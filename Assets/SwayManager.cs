using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;

public class SwayManager : MonoBehaviour {

    TransformAccessArray transforms;
    SwayJob job;
    JobHandle jh;

    
    NativeArray<Vector3> axis;

    public float theta;
    public float frequency;
    public float angle;

    int maxJobs = 5000;
    int numJobs = 0;

    public static SwayManager Instance = null;

    public SwayManager()
    {
        Instance = this;
    }

    public int Add(Transform t, Vector3 axis)
    {
        transforms.Add(t); // The head
        this.axis[numJobs] = axis;
        numJobs ++;
        return numJobs - 1;
    }

    public void Awake()
    {
        axis = new NativeArray<Vector3>(maxJobs, Allocator.Persistent);
        transforms = new TransformAccessArray(maxJobs);
    }

    private void OnDestroy()
    {
        jh.Complete();
        transforms.Dispose();
        axis.Dispose();
    }

    public void LateUpdate()
    {
        jh.Complete();
        theta += frequency * Time.deltaTime * Mathf.PI * 2.0f;
    }

    public void Update()
    {
        job = new SwayJob()
        {
            deltaTime = Time.deltaTime,
            frequency = this.frequency,
            axis = this.axis,
            angle = this.angle,
            theta = this.theta
        };

        jh = job.Schedule(transforms);
    }
}

public struct SwayJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<Vector3> axis;

    public float deltaTime;
    public float frequency;
    public float theta;
    public float angle;

    public void Execute(int i, TransformAccess t)
    {
        if (angle != 0)
        {
            t.localRotation = Quaternion.AngleAxis(
                BGE.Forms.Utilities.Map(Mathf.Sin(theta), -1, 1, -angle, angle)
                , axis[i]
                );
        }
    }
}
