using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
       //A tunnel section is a partial circle.Or a partial torus in 3D
    public float pipeRadius; ////A torus is defined by two radiuses pipeRadius and curveRadius
    public int pipeSegmentCount; ////i will use a mesh to create the pipe's surface

    public float minCurveRadius, maxCurveRadius;
    public int minCurveSegmentCount, maxCurveSegmentCount;

    private float curveRadius;
    private int curveSegmentCount;

    //procedurally generate colour with lerp
    public float speed = 1.0f;
    public Color StartColor;
    public Color EndColor;
    public bool repeat;
    private float startTime;
    public ItemGenerator[] generators;



    private Vector3 GetPointOnTorus(float u, float v) //find points on the surface using x = (R + r cos v) cos u
        //y = (R + r cos v) sin u
        //z = r cos v
    {
        Vector3 p;
        float r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);
        return p;
    }

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    public float ringDistance;
    private float curveAngle;
    private float relativeRotation;
    private Vector2[] surface;

 
   private void Start()
    {
       
        startTime = Time.time;
    }


    private void Update()
    {
        if(!repeat)
        { //repeat lerp every set amount of time
            float t = (Time.time - startTime) * speed;
            GetComponent<Renderer>().material.color = Color.Lerp(StartColor, EndColor, t);
            
        }
        else
        {
            float t = (Mathf.Sin(Time.time - startTime) * speed);
            GetComponent<Renderer>().material.color = Color.Lerp(StartColor, EndColor, t);
           
        }
    }
    
   
    private void Awake()
    {
        //create the mesh when the object awakens
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Wormhole";
      
       
    }

    public void Generate()
    {
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        // randomize the curve radius and segment count to each fall within a range
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
        mesh.Clear();
        SetVertices();
        SetSurface();
        SetTriangles();
        mesh.RecalculateNormals();
        //generators[Random.Range(0, generators.Length)].GenerateItems(this);
    }
    private void SetVertices() //give each quad its own four vertices
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        //setting the distance between rings to not cover entire torus
        float uStep = ringDistance / curveRadius;
        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        //apply to all vertices that are stuck at the origin
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        //create two vertices A and B
        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);
        //assign  vertices to the quads as it iterates through the segments
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    private void SetTriangles()//initialize the triangles. Each quad has two triangles = six vertex indices.
    {
            triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        //order vertices so triangles show  on the outside of the tunnel.
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
            {
                triangles[t] = i;
                triangles[t + 1] = triangles[t + 4] = i + 2;
                triangles[t + 2] = triangles[t + 3] = i + 1;
                triangles[t + 5] = i + 3;
            }

            mesh.triangles = triangles;
        }

    //copy the first two vertices per quad from those of the previous ring
    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }
    //align itself 
    public void AlignWith(Wormhole pipe)
    {
        //perform random relative rotation of tunnels
        relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;
        transform.SetParent(pipe.transform, false);
        //make sure that the pipe is at its parent's origin
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
        // align with the parent and move up so origin sits at the end point of parent tunnell
        transform.Translate(0f, pipe.curveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -curveRadius, 0f);
        transform.SetParent(pipe.transform.parent);
        transform.localScale = Vector3.one;
    }

  //set the UV of the surface to make the mesh display
private void SetSurface()
    {
        surface = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            surface[i] = Vector2.zero;
            surface[i + 1] = Vector2.right;
            surface[i + 2] = Vector2.up;
            surface[i + 3] = Vector2.one;
            
          
        }
        mesh.uv = surface;
    }

    public float CurveRadius
    {
        get
        {
            return curveRadius;
        }
    }

    public float CurveAngle
    {
        get
        {
            return curveAngle;
        }
    }

    public float RelativeRotation
    {
        get
        {
            return relativeRotation;
        }
    }

    public int CurveSegmentCount
    {
        get
        {
            return curveSegmentCount;
        }
    }
}
