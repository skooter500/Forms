using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGen : MonoBehaviour {

    public float size = 100;
    public float angle = 30;
    public float branchRatio = 0.4f;
    public int depth = 3;
    public int children = 3;

    GameObject CreateBranch(Transform parent, Vector3 position, Quaternion rotation, float size, int depth)
    {
        GameObject branch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);        
        branch.transform.position = position;
        branch.transform.rotation = rotation;
        branch.isStatic = true;
        branch.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        branch.GetComponent<Renderer>().receiveShadows = false;
        branch.transform.localScale = new Vector3(size * 0.2f, size * 0.5f, size * 0.2f);
        Vector3 top = position + (branch.transform.up * size * 0.5f);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = top;
        sphere.isStatic = true;
        sphere.transform.parent = this.transform;
        sphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sphere.GetComponent<Renderer>().receiveShadows = false;

        sphere.transform.localScale = new Vector3(size * 0.3f, size * 0.3f, size * 0.3f);
        if (depth < this.depth)
        {
            float thetaInc = 360.0f / (float)children;
            for (int i = 0; i < children; i++)
            {
                float theta = thetaInc * i;
                Quaternion q = branch.transform.rotation * Quaternion.Euler(angle, theta, 0);

                float branchsize = size * branchRatio;
                Vector3 p = top + (q * Vector3.up * branchsize * 0.5f);
                GameObject b = CreateBranch(branch.transform, p, q, branchsize, depth + 1);
                b.transform.parent = this.transform;
            }
        }
        return branch;
    }

	// Use this for initialization
	void Awake () {
        CreateBranch(this.transform, transform.position, transform.rotation, size, 1).transform.parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
