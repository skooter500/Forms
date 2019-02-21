using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//creates multiple pipes together to generate the wormhole tunnel effect
public class Tunnell : MonoBehaviour {

    public Wormhole pipePrefab;

    public int pipeCount;

    private Wormhole[] pipes;

    private void Awake()
    {
        pipes = new Wormhole[pipeCount];
        for (int i = 0; i < pipes.Length; i++)
        {
            Wormhole pipe = pipes[i] = Instantiate<Wormhole>(pipePrefab);
            pipe.transform.SetParent(transform, false);
            pipe.Generate();
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
        AlignNextPipeWithOrigin();
    }


    public Wormhole SetupFirstPipe()
    //provide the first pipe
    {
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    public Wormhole SetupNextPipe()
    //this means Tunnell has to shift the pipes in its array
    {
        ShiftPipes();
        //align the next pipe with the origin, and reset its position
        AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate();
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    private void ShiftPipes()
    {//current first pipe becomes the new last pipe
        Wormhole temp = pipes[0];
        for (int i = 1; i < pipes.Length; i++)
        {
            pipes[i - 1] = pipes[i];
        }
        pipes[pipes.Length - 1] = temp;
    }
    private void AlignNextPipeWithOrigin()
    {//reset position and rotation of the new first pipe
        Transform transformToAlign = pipes[1].transform;
        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transformToAlign);
            }
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transform);
            }
        }
    }

}
