using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetColors : MonoBehaviour
{

    public Material material;
    public float speed = 1.0f;

    float offset = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Offset());   
    }

    
    System.Collections.IEnumerator Offset()
    {
        yield return new WaitForSeconds(0.1f);
        Renderer r = GetComponent<Renderer>();
        while (true)
        {
            material = r.material;
            material.SetFloat("_Offset", offset);
            offset += Time.deltaTime * speed;
            yield return null;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        //Shader.SetGlobalFloat("_Offset", offset);
        //material.SetGlobalFloat("_Offset", offset);
        offset += Time.deltaTime * speed;
    }
}
