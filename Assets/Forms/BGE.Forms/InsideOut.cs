using UnityEngine;
using System.Collections;
using System.Linq;

namespace BGE.Forms
{
    public class InsideOut : MonoBehaviour {

        // Use this for initialization
        void Start () {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}