using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGen : MonoBehaviour {
    public GameObject branchPrefab;
    public GameObject nodePrefab;
    
    public float size = 100;
    public float angle = 30;
    public float branchRatio = 0.4f;
    public int depth = 3;
    public int children = 3;
    public bool stocastic = false;

    GameObject CreateBranch(Vector3 position, Quaternion rotation, float size, int depth)
    {
        GameObject branch = GameObject.Instantiate(branchPrefab);
        branch.transform.position = position;
        branch.transform.rotation = rotation;
        branch.transform.parent = this.transform;
        branch.SetActive(true);
        branch.transform.localScale = new Vector3(size * 0.2f, size * 0.5f, size * 0.2f);
        Vector3 top = position + (branch.transform.up * size * 0.5f);
        GameObject sphere = GameObject.Instantiate(nodePrefab);
        sphere.transform.position = top;
        sphere.isStatic = true;
        sphere.transform.parent = this.transform;
        sphere.SetActive(true);
        sphere.transform.localScale = new Vector3(size * 0.3f, size * 0.3f, size * 0.3f);
        if (depth < this.depth)
        {
            float thetaInc = 360.0f / (float)children;

            float branchsize = size * branchRatio;
            Vector3 p = top + (branch.transform.rotation * Vector3.up * branchsize * 0.7f);
            GameObject b = CreateBranch(p, branch.transform.rotation, branchsize, depth + 1);

            for (int i = 0; i < children; i++)
            {
                float theta = thetaInc * i;

                Quaternion q = branch.transform.rotation * Quaternion.Euler(
                    stocastic  ? Random.Range(angle - 30, angle + 30) : angle
                    , theta, 0);

                p = top + (q * Vector3.up * branchsize * 0.7f);
                b = CreateBranch(p, q, branchsize, depth + 1);                
            }
        }
        return branch;
    }

	// Use this for initialization
	void Awake () {
        Vector3 pos = Vector3.zero;
        pos.y += (size / 2);
        CreateBranch(pos, Quaternion.identity, size, 1).transform.parent = this.transform;
        CombineMeshes();

    }

    private void Start()
    {
        
    }

    public bool addCollider = true;

    public void CombineMeshes()
    {
        MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>();

        CombineInstance[] cms = new CombineInstance[mfs.Length];
        for(int i = mfs.Length - 1; i >= 0 ; i --)
        {
            cms[i].subMeshIndex = 0;
            cms[i].mesh = mfs[i].sharedMesh;
            cms[i].transform = mfs[i].transform.localToWorldMatrix;
            GameObject.Destroy(mfs[i].gameObject);
        }

        Mesh mesh = new Mesh();
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mesh.CombineMeshes(cms);
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mf.sharedMesh = mesh;
        mr.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        if (addCollider)
        {
            BoxCollider c = gameObject.AddComponent<BoxCollider>();
        }
        //c.bounds = mesh.bound
        //Debug.Log("Vertex count: " + mesh.vertexCount);
        //MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        //mc.sharedMesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
