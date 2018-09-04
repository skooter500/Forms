using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using BGE.Forms;

public struct FishAnimatorJob : IJobParallelForTransform
{
    float deltaTime;
    float theta;
    float angle;
    float speedMultiplier;
    float speed;
    float angularVelocity;

    public void Execute(int index, TransformAccess t)
    {
        float headRot = Mathf.Sin(theta) * angle;
        t.localRotation = Quaternion.AngleAxis(headRot, Vector3.up);
        theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;
    }
}