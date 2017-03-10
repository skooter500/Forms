using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {
    public GameObject prefab;
    public int gridWidth = 10;
    public float cellWidth;

    [Range(0.0f, 1.0f)]
    public float density;

    public void Awake()
    {
        int halfWidth = gridWidth / 2;

        float offs = gridWidth * cellWidth * 0.5f;
        Vector3 left = transform.position - new Vector3(offs, offs, offs);
        Vector3 halfCell = new Vector3(cellWidth, cellWidth, cellWidth) * 0.5f;
        for (int row = 0; row < gridWidth; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                for (int seg = 0; seg < gridWidth; seg++)
                {
                    float dice = Random.Range(0.0f, 1.0f);
                    if (dice < density)
                    {
                        GameObject newObject = GameObject.Instantiate<GameObject>(prefab);
                        Vector3 pos = left + new Vector3(col * cellWidth, seg * cellWidth, row * cellWidth) + halfCell;
                        Vector3 scale = newObject.transform.localScale;
                        scale *= Random.Range(0.1f, 3.0f);
                        newObject.transform.localScale = scale;
                        newObject.transform.position = pos;
                        newObject.layer = this.gameObject.layer;
                        newObject.transform.parent = this.transform;
                    }
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
