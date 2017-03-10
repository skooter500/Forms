
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BGE.Forms;
using UnityEngine;

namespace BGE.Forms
{
    public class NewFishParts : MonoBehaviour
    {
        public GameObject head;
        public GameObject body;
        public GameObject tail;

        List<GameObject> segments;

        float segmentExtents = 3;
        public float gap;

        // Animation stuff
        float theta;
        float angularVelocity = 5.00f;

        public GameObject headRotGameObject;
        public GameObject tailRotGameObject;

        private Vector3 headSize;
        private Vector3 bodySize;
        private Vector3 tailSize;

        [Range(0.0f, 2.0f)]
        public float speedMultiplier;

        public float headField;
        public float tailField;

        public GameObject boidGameObject;

        [HideInInspector]
        public Boid boid;

        public bool boidSpeedToAnimationSpeed = true;

        public NewFishParts()
        {
            segments = new List<GameObject>();

            theta = 0;
            speedMultiplier = 1.0f;
            headField = 5;
            tailField = 50;
        }

        public GameObject InstiantiateDefaultShape()
        {

            GameObject segment = null;
            segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 scale = new Vector3(1, segmentExtents, segmentExtents);
            segment.transform.localScale = scale;
            return segment;
        }

        public void OnDrawGizmos()
        {
            float radius = (1.5f * segmentExtents) + gap;
            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, radius);
        }


        public void Start()
        {            
            if (head.GetComponent<Collider>() != null)
            {
                head.GetComponent<Collider>().enabled = false;
            }
            if (body.GetComponent<Collider>() != null)
            {
                body.GetComponent<Collider>().enabled = false;
            }
            if (tail.GetComponent<Collider>() != null)
            {
                tail.GetComponent<Collider>().enabled = false;
            }

            boid = (boidGameObject == null) ? GetComponent<Boid>() : boidGameObject.GetComponent<Boid>();

        }
        

        float oldHeadRot = 0;
        float oldTailRot = 0;

        private float fleeColourWait;
        private bool fleeColourStarted;

        public void Update()
        {
            // Animate the head            
            float headRot = Mathf.Sin(theta) * headField;
            head.transform.RotateAround(headRotGameObject.transform.position, headRotGameObject.transform.up, headRot - oldHeadRot);

            oldHeadRot = headRot;

            // Animate the tail
            float tailRot = Mathf.Sin(theta) * tailField;
            tail.transform.RotateAround(tailRotGameObject.transform.position, - tailRotGameObject.transform.up, tailRot - oldTailRot);
            oldTailRot = tailRot;

            float speed;

            speed = boidSpeedToAnimationSpeed ? boid.acceleration.magnitude : 1.0f; ;
            theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;
            if (theta >= Mathf.PI * 2.0f)
            {
                theta -= (Mathf.PI * 2.0f);
            }
        }
    }
}
