
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

        private Vector3 segmentSize;

        public float closeness = 500;

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
            segment.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            segment.layer = this.gameObject.layer;
            segment.transform.localScale = segmentSize;

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

        private void TranslateMesh(GameObject go, Vector3 trans)
        {
            Mesh mesh = go.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = mesh.vertices;
            for(int i = 0; i < vertices.Length; i ++)
            {
                vertices[i] += trans;
            }
            mesh.vertices = vertices;
            go.GetComponent<MeshFilter>().mesh = mesh;
        }


        public void Start()
        {

            if (transform.childCount != 3)
            {
                segmentSize = new Vector3(segmentExtents * 0.5f, segmentExtents, segmentExtents);

                head = InstiantiateDefaultShape();
                body = InstiantiateDefaultShape();
                tail = InstiantiateDefaultShape();
                TranslateMesh(head, new Vector3(0, 0, 0.5f));
                TranslateMesh(tail, new Vector3(0, 0, -0.5f));

                head.name = "head";
                body.name = "body";
                tail.name = "tail";

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

            //FishAnimatorManager.Instance.AddFish(this.transform, headField, tailField);
        }

        private void LayoutSegments()
        {
            body.transform.position = transform.position;
            body.transform.rotation = transform.rotation;
            float headOffset = (segmentSize.z / 2) + gap;
            head.transform.position = transform.TransformPoint(new Vector3(0, 0, headOffset));
            head.transform.rotation = transform.rotation;


            float tailOffset = (segmentSize.z / 2) + gap;
            tail.transform.position = transform.TransformPoint(new Vector3(0, 0, -tailOffset));
            tail.transform.rotation = transform.rotation;

            head.transform.parent = transform;            
            body.transform.parent = transform;
            tail.transform.parent = transform;

            head.transform.rotation = transform.rotation;
            body.transform.rotation = transform.rotation;
            tail.transform.rotation = transform.rotation;
        }

        int jobIndex;
        
        public void LateUpdate()
        {
            // Replace this with a Boid system at some stage
            //FishAnimatorManager.Instance.speed[jobIndex] = boid.speed;
            
            if (boid.distanceToPlayer > closeness)
            {
                return;
            }

            // Animate the head            
            float headRot = Mathf.Sin(theta) * headField;
            head.transform.localRotation = Quaternion.AngleAxis(headRot, Vector3.up);

            // Animate the tail
            float tailRot = Mathf.Sin(theta) * tailField;
            tail.transform.localRotation = Quaternion.AngleAxis(tailRot, Vector3.up);
            float speed = boid.velocity.magnitude;
            theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;
        }
    }
}
