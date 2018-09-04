using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using BGE.Forms;

public struct FishAnimatorJob : IJobParallelForTransform
{
    float deltaTime;
    float theta;
    float angle;

    public void Execute(int index, TransformAccess t)
    {

    }
}