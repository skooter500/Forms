using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using BGE.Forms;

public class NoiseJobManager : MonoBehaviour
{
    TransformAccessArray transforms;
    
    int maxJobs = 30000;
    int numJobs = 0;

    JobHandle jh;

    public GameObject prefab;

    public static NoiseJobManager Instance;

    public float tt; 

    public void Awake()
    {
        transforms = new TransformAccessArray(0, -1);
        Instance = this;

    }

    public void Add(NoiseEffect ne)
    {
        transforms.capacity = transforms.length + 1;
        transforms.Add(ne.transform);
        numJobs++;
    }

    public void OnDestroy()
    {
        transforms.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        jh.Complete();
    }

    // Update is called once per frame
    void Update()
    {
        tt += prefab.GetComponent<NoiseEffect>().speed * Time.deltaTime;
        var job = new NoiseJob()
        {
            t = tt
            , scale = prefab.GetComponent<NoiseEffect>().scale
        };
        jh = job.Schedule(transforms);
    }
}

public struct NoiseJob : IJobParallelForTransform
{
    public float t;
    public float scale;

    public void Execute(int i, TransformAccess transform)
    {

        float nn = Utilities.Map(Perlin.Noise(transform.position.x + t, transform.position.y + t, transform.position.z + t ), -1, 1, 0, scale);
        Vector3 sc = new Vector3(nn, nn, nn);
        transform.localScale = sc;

    }
}


