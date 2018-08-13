using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWorm : MonoBehaviour {
    public int bodySegments = 10;
    public float radius = 50;
    public bool gravity = false;

    public float spring = 100;
    public float damper = 50;

	// Use this for initialization
	void Start () {
        Vector3 p = Camera.main.transform.position + Camera.main.transform.forward * radius * 10;
        p.y = 5;
        transform.position = p;
        Create();
        //StartCoroutine(Move());

    }

    void Create()
    { 
        float depth = radius * 0.1f;
        Vector3 start = - Vector3.forward * bodySegments * depth * 2;

        GameObject previous = null;
        for (int i = 0; i < bodySegments; i++)
        {
            float r = radius;
            float d = damper;
            bool g = gravity;
            if (i < headtail)
            {
                //r = radius * Mathf.Pow(2, - (headtail - i));
                r = radius * Mathf.Pow(0.2f, (headtail - i));
                g = false;
            }
            if (i > bodySegments - headtail - 1)
            {
                //r = radius * Mathf.Pow(2, - (headtail - i));
                r = radius * Mathf.Pow(0.9f, i - (bodySegments - headtail - 1));
                g = false;
            }
            GameObject bodyPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = bodyPart.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = g;
            Vector3 pos = start + (Vector3.forward * depth * 4 * i);
            bodyPart.transform.position = transform.TransformPoint(pos);
            Quaternion rot = Quaternion.AngleAxis(0, Vector3.right);
            bodyPart.transform.rotation = transform.rotation * rot;
            bodyPart.transform.parent = this.transform;           

            bodyPart.transform.localScale = new Vector3(r * 2, r * 2, depth);

            bodyPart.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            bodyPart.GetComponent<Renderer>().receiveShadows = false;

            if (previous != null)
            {
                HingeJoint j = bodyPart.AddComponent<HingeJoint>();
                j.connectedBody = previous.GetComponent<Rigidbody>();
                j.autoConfigureConnectedAnchor = false;
                j.axis = Vector3.right;
                Debug.Log(depth);
                j.anchor = new Vector3(0, 0, - 2f);
                j.connectedAnchor = new Vector3(0, 0, 2f);
                j.useSpring = true;
                JointSpring js = j.spring;
                js.spring = spring;
                js.damper = d;
                j.spring = js;
                //j.useSpring = false;

                /*SpringJoint j = bodyPart.AddComponent<SpringJoint>();
                j.connectedBody = previous.GetComponent<Rigidbody>();
                j.autoConfigureConnectedAnchor = true;
                j.spring = 60;
                j.damper = 10;
                //j.anchor = new Vector3(0, 0, -1);
                j.axis = Vector3.right;
                //j.useSpring = false;
                */
            }            
            previous = bodyPart;
        }
        // Add head and tail balls



    }

    public float force = 100;
    public float frequency = 2;
    public float pulsesPerSecond = 2;
    
    System.Collections.IEnumerator Move()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            float thetaInc = (Mathf.PI * 2 * frequency) / (transform.childCount);

            for (int i = 1; i < transform.childCount - 1; i++)
            {
                float theta = thetaInc * i ;
                Rigidbody rb = transform.GetChild(i).GetComponent<Rigidbody>();
                rb.AddTorque(rb.transform.right * force * Mathf.Cos(theta));
            }
            yield return new WaitForSeconds(1.0f / pulsesPerSecond);
            ////for (int i = 0; i < transform.childCount; i += m)
            ////{
            ////    Rigidbody rb = transform.GetChild(i).GetComponent<Rigidbody>();
            ////    rb.AddTorque(-rb.transform.right * force);
            ////}
            //yield return new WaitForSeconds(1.0f / pulsesPerSecond);
        }
    }

    private float offset = 0;
    public float speed = 1f;
    public int headtail = 2;

    public void Update()
    {
        int count = transform.childCount;
        float tm = Mathf.PI / (float)count;
        float thetaInc = (Mathf.PI * 2 * frequency) / (transform.childCount);
        for (int i = 0; i < count; i++)
        {
            float theta = (thetaInc * i) + offset;
            Rigidbody rb = transform.GetChild(i).GetComponent<Rigidbody>();
            rb.AddTorque(-rb.transform.right * force * Mathf.Sin(theta) * Mathf.Sin(tm * i));
        }
        offset -= speed * Time.deltaTime;
    }
}
