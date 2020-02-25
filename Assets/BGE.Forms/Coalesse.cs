using UnityEngine;
using System.Collections;
using BGE.Forms;

public class Coalesse : MonoBehaviour {
    School flock;
    public GameObject player;
	// Use this for initialization
	void OnEnable () {
        flock = GetComponent<School>();
        StartCoroutine("CoalesseBoids");
	}

    System.Collections.IEnumerator CoalesseBoids()
    {
        while (true)
        {
            foreach (Boid boid in flock.boids)
            {
                boid.GetComponent<Seperation>().SetActive(true);
                boid.GetComponent<Cohesion>().SetActive(true);
                boid.GetComponent<Alignment>().SetActive(true);
                boid.GetComponent<JitterWander>().SetActive(true);
                boid.GetComponent<Seek>().SetActive(false);
                boid.GetComponent<SceneAvoidance>().SetActive(true);
                boid.GetComponent<Flee>().SetActive(true);
            }
            yield return new WaitForSeconds(Random.Range(20.0f, 30.0f));
            foreach (Boid boid in flock.boids)
            {
                // Only affect boids in front of the player
                Vector3 toBoid = boid.transform.position - player.transform.position;
                if ((Vector3.Dot(player.transform.forward, toBoid) >= 0) && (Random.Range(0, 0.5f) < 0.5f) && toBoid.magnitude < flock.radius * 4.0f)
                {
                    boid.GetComponent<Seperation>().SetActive(true);
                    boid.GetComponent<SceneAvoidance>().SetActive(false);
                    boid.GetComponent<Cohesion>().SetActive(true);
                    boid.GetComponent<Alignment>().SetActive(true);
                    boid.GetComponent<JitterWander>().SetActive(true);
                    boid.GetComponent<Seek>().SetActive(true);
                    boid.GetComponent<Flee>().SetActive(false);
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    boid.GetComponent<Seek>().targetGameObject = player;
                    boid.GetComponent<Seek>().target.y += 10;
                }
            }
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));
        }
    }
}
