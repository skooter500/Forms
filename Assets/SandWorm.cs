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
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * radius * 10;
        Create();
        StartCoroutine(Move());

    }

    void Create()
    { 
        float depth = radius * 0.1f;
        Vector3 start = - Vector3.forward * bodySegments * depth * 2;
        GameObject previous = null;
        for (int i = 0; i < bodySegments; i++)
        {
            GameObject bodyPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = bodyPart.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = gravity;
            Vector3 pos = start + (Vector3.forward * depth * 4 * i);
            bodyPart.transform.position = transform.TransformPoint(pos);
            Quaternion rot = Quaternion.AngleAxis(0, Vector3.right);
            bodyPart.transform.rotation = transform.rotation * rot;
            bodyPart.transform.parent = this.transform;           

            bodyPart.transform.localScale = new Vector3(radius * 2, radius * 2, depth);

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
                js.damper = damper;
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

            for (int i = 0; i < transform.childCount; i++)
            {
                float theta = thetaInc * i ;
                Rigidbody rb = transform.GetChild(i).GetComponent<Rigidbody>();
                rb.AddTorque(rb.transform.right * force * Mathf.Sin(theta));
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
}
