﻿using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class GenerateTerrain : MonoBehaviour {

        int heightScale = 5;
        float detailScale = 5.0f;

        // Use this for initialization
        void Start () 
        {
            Mesh mesh = this.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            for(int v = 0; v < vertices.Length; v++)
            {
                vertices[v].y = Mathf.PerlinNoise((vertices[v].x + this.transform.position.x)/detailScale,
                                    (vertices[v].z + this.transform.position.z)/detailScale)*heightScale;
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            this.gameObject.AddComponent<MeshCollider>();
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}