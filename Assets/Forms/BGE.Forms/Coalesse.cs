using UnityEngine;
using System.Collections;
using BGE.Forms;

public class Coalesse : MonoBehaviour {
    School flock;
    public GameObject player;
	// Use this for initialization
	void Start () {
        flock = GetComponent<School>();
        StartCoroutine("CoalesseBoids");
	}

    System.Collections.IEnumerator CoalesseBoids()
    {
        while (true)
        {
            Debug.Log("Schooling");
            foreach (Boid boid in flock.boids)
            {
                boid.GetComponent<Seperation>().Activate(true);
                boid.GetComponent<Cohesion>().Activate(true);
                boid.GetComponent<Alignment>().Activate(true);
                boid.GetComponent<JitterWander>().Activate(true);
                boid.GetComponent<Seek>().Activate(false);
                boid.GetComponent<SceneAvoidance>().Activate(true);
            }
            yield return new WaitForSeconds(Random.Range(20.0f, 30.0f));
            Debug.Log("Coalessing");
            foreach (Boid boid in flock.boids)
            {
                // Only affect boids in front of the player
                Vector3 toBoid = boid.transform.position - player.transform.position;
                if ((Vector3.Dot(player.transform.forward, toBoid) >= 0) && (Random.Range(0, 0.5f) < 0.5f) && toBoid.magnitude < flock.radius * 5.0f)
                {
                    boid.GetComponent<Seperation>().Activate(true);
                    boid.GetComponent<SceneAvoidance>().Activate(false);
                    boid.GetComponent<Cohesion>().Activate(true);
                    boid.GetComponent<Alignment>().Activate(true);
                    boid.GetComponent<JitterWander>().Activate(true);
                    boid.GetComponent<Seek>().Activate(true);
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    boid.GetComponent<Seek>().targetGameObject = player;
                    boid.GetComponent<Seek>().target.y += 10;
                }
                else
                {
                    Debug.Log("Too far");
                }
            }
            yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
