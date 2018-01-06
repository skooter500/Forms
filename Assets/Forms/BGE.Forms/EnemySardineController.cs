using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{
    public class EnemySardineController : MonoBehaviour
    {
        public School school;
        Seek seek;
        Boid targetBoid = null;
        System.Collections.IEnumerator EnemyController()
        {             
            while (true)
            {
                // Wandering
                Debug.Log("Enemy wandering");
                seek.SetActive(false);
                GetComponent<JitterWander>().SetActive(true);
                GetComponent<Constrain>().SetActive(true);
                yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
                // Seeking
                Debug.Log("Enemy seeking prey");
                targetBoid = school.boids[Random.Range(0, school.boids.Count)];
                seek.SetActive(true);                
                seek.targetGameObject = targetBoid.gameObject;
                GetComponent<Constrain>().SetActive(false);
                GetComponent<JitterWander>().SetActive(false);
                yield return new WaitForSeconds(Random.Range(10.0f, 20.0f));
            }
        }


        // Use this for initialization
        void Start()
        {
            seek = GetComponent<Seek>();
            StartCoroutine(EnemyController());
        }
    }
}
