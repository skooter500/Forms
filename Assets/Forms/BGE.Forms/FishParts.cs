
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BGE.Forms;
using UnityEngine;

namespace BGE.Forms
{
    public class FishParts : MonoBehaviour
    {
        [HideInInspector]
        public GameObject head;
        [HideInInspector]
        public GameObject body;
        [HideInInspector]
        public GameObject tail;

        List<GameObject> segments;

        public float segmentExtents = 3;
        public float gap;

        // Animation stuff
        float theta;
        float angularVelocity = 5.00f;

        private Vector3 headRotPoint;
        private Vector3 tailRotPoint;

        private Vector3 headSize;
        private Vector3 bodySize;
        private Vector3 tailSize;

        public float closeness = 100;

        public Color[] segmentColors = {
            Color.blue
            , Color.cyan
            , Color.blue
        };

        public float speedMultiplier;

        public float headField;
        public float tailField;

        public GameObject boidGameObject;

        [HideInInspector]
        public Boid boid;

        public FishParts()
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
            segment.GetComponent<Renderer>().receiveShadows = false;
            segment.layer = this.gameObject.layer;
            Vector3 scale = new Vector3(segmentExtents *0.5f, segmentExtents, segmentExtents);
            segment.transform.localScale = scale;

            return segment;
        }

        public void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                float radius = (1.5f*segmentExtents) + gap;
                Gizmos.color = Color.yellow;
            }
            //Gizmos.DrawWireSphere(transform.position, radius);
        }


        public void Awake()
        {

            if (transform.childCount != 3)
            {
                head = InstiantiateDefaultShape();
                body = InstiantiateDefaultShape();
                tail = InstiantiateDefaultShape();

                head.GetComponent<Renderer>().material.color = segmentColors[0];
                body.GetComponent<Renderer>().material.color = segmentColors[1];
                tail.GetComponent<Renderer>().material.color = segmentColors[2];

                LayoutSegments();
            }
            else
            {
                head = transform.GetChild(0).gameObject;
                body = transform.GetChild(1).gameObject;
                tail = transform.GetChild(2).gameObject;
            }

            segments.Add(head);
            segments.Add(body);
            segments.Add(tail);

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

        private void LayoutSegments()
        {
            bodySize = body.GetComponent<Renderer>().bounds.size;
            headSize = head.GetComponent<Renderer>().bounds.size;
            tailSize = tail.GetComponent<Renderer>().bounds.size;

            body.transform.position = transform.position;
            body.transform.rotation = transform.rotation;
            float headOffset = (bodySize.z / 2.0f) + gap + (headSize.z / 2.0f);
            head.transform.position = transform.position + new Vector3(0, 0, headOffset);

            float tailOffset = (bodySize.z / 2.0f) + gap + (tailSize.z / 2.0f);
            tail.transform.position = transform.position + new Vector3(0, 0, -tailOffset);

            head.transform.parent = transform;
            tail.transform.parent = transform;
            body.transform.parent = transform;

            headRotPoint = head.transform.localPosition;
            headRotPoint.z -= headSize.z / 2;

            tailRotPoint = tail.transform.localPosition;
            tailRotPoint.z += tailSize.z / 2;


            head.transform.rotation = transform.rotation;
            body.transform.rotation = transform.rotation;
            tail.transform.rotation = transform.rotation;
        }

        float oldHeadRot = 0;
        float oldTailRot = 0;

        private float fleeColourWait;
        private bool fleeColourStarted;

        public bool suspended = false;

        public void Update()
        {
            if (!boid.isVisible() || boid.dis)
            {
                return;
            }

            // Animate the head            
            float headRot = Mathf.Sin(theta) * headField;
            head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, headRot - oldHeadRot);

            oldHeadRot = headRot;

            // Animate the tail
            float tailRot = Mathf.Sin(theta) * tailField;
            tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, tailRot - oldTailRot);
            oldTailRot = tailRot;

            float speed = boid.velocity.magnitude;
            theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;
        }
    }
}
