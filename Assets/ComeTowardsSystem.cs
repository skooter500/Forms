
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComeTowardsSystem : MonoBehaviour
{

    public GameObject prefab;
    public Camera c;

    public int maxEntities = 50;
    public float spawnRate = 2;
    public float speed = 10;

    public List<Transform> transforms = new List<Transform>();
    public List<Vector3> targets = new List<Vector3>();
    public List<Vector3> axis = new List<Vector3>();

    System.Collections.IEnumerator SpawnStuff()
    {
        while (true)
        {
            if (transforms.Count < maxEntities)
            {
                GameObject thing = GameObject.Instantiate<GameObject>(prefab);

                transforms.Add(thing.transform);
                thing.transform.parent = this.transform;
                thing.transform.position = this.transform.position;
                Vector3 target = CalculateTarget();
                targets.Add(target);
                axis.Add(Random.insideUnitCircle);
                thing.GetComponent<BGE.Forms.LifeColours>().FadeIn();
            }
            yield return new WaitForSeconds(1.0f / spawnRate);
        }
    }

    private Vector3 CalculateTarget()
    {
        Vector3 t = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.up
                    * Random.Range(50, 100);
        t -= Vector3.forward * 10;
        t = c.transform.TransformPoint(t);
        return t;
    }

    void OnEnable()
    {
        StartCoroutine(SpawnStuff());
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < transforms.Count; i++)
        {
            Transform t = transforms[i];
            t.transform.position = Vector3.MoveTowards(t.transform.position, targets[i], speed * Time.deltaTime);
            t.transform.Rotate(axis[i], Time.deltaTime * 30);

            if (Vector3.Distance(t.transform.position, targets[i]) < 5)
            {
                t.transform.position = this.transform.position;
                t.GetComponent<BGE.Forms.LifeColours>().FadeIn();
                targets[i] = CalculateTarget();
            }
        }
    }
}
