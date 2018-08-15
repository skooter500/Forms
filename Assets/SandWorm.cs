﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWorm : MonoBehaviour {
    public int bodySegments = 10;
    public float radius = 50;
    public bool gravity = false;

    public float spring = 100;
    public float damper = 50;

    // Use this for initialization
	void Awake () {
        if (transform.childCount == 0)
        {
            Create();
        }
        Invoke("StartMoving", 10);
        //Animate();
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
                r = radius * Mathf.Pow(0.6f, (headtail - i));
                //g = false;
            }
            if (i > bodySegments - headtail - 1)
            {
                //r = radius * Mathf.Pow(2, - (headtail - i));
                r = radius * Mathf.Pow(0.8f, i - (bodySegments - headtail - 1));
               //g = false;
            }
            GameObject bodyPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = bodyPart.AddComponent<Rigidbody>();
            bodyPart.GetComponent<Renderer>().material.color = Color.black;
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
        /*
        // Add head and tail balls
        Transform neck = transform.GetChild(transform.childCount - 1);
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody rb1 = head.AddComponent<Rigidbody>();
        rb1.useGravity = gravity;
        Vector3 hp = transform.forward * (radius * 2 + depth * 3);
        hp = neck.transform.TransformPoint(hp);
        head.transform.position = hp;
        head.transform.localScale = new Vector3(radius * 1f, radius * 1f, radius * 1f);
        
        HingeJoint j1 = head.AddComponent<HingeJoint>();
        j1.connectedBody = neck.GetComponent<Rigidbody>();
        j1.autoConfigureConnectedAnchor = false;
        j1.axis = Vector3.right;
        j1.anchor = new Vector3(0, 0, -0.6f);
        j1.connectedAnchor = new Vector3(0, 0, 2f);
        j1.useSpring = true;
        JointSpring js1 = j1.spring;
        js1.spring = spring;
        js1.damper = damper;
        j1.spring = js1;
        */

    }

    public float force = 100;
    public float frequency = 2;
    public float pulsesPerSecond = 2;
    
   
    private float offset = 0;
    public float speed = 1f;
    public int headtail = 2;

    float current = 0;
    int start = 3;

    bool moving = false;

    public void StartMoving()
    {
        moving = true;
    }

    public void FixedUpdate()
    {
        if (moving)
        {
            Animate();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moving = !moving;
        }
    }

    public void Animate()
    {
        float f = force;
        Rigidbody rb = transform.GetChild((int) current).GetComponent<Rigidbody>();
        if (current >= transform.childCount - start)
        {
            f *= .3f;
        }
        rb.AddTorque(- rb.transform.right * f);
        current += speed * Time.deltaTime;


        if (current >= transform.childCount)
        {
            current = 0;
        }

        /*
        int count = transform.childCount;
        float tm = Mathf.PI / (float)count;
        float thetaInc = (Mathf.PI * frequency) / (transform.childCount);
        int t = 0;
        for (int i = 0; i < count; i++)
        {
            float theta = ((thetaInc * t) + offset) % Mathf.PI;
            Rigidbody rb = transform.GetChild(i).GetComponent<Rigidbody>();
            rb.AddTorque(rb.transform.right * force * Mathf.Sin(theta));
            t++;
        }
        offset -= speed * Time.deltaTime;
        */
    }
}
