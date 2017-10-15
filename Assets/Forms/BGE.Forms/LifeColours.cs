using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class LifeColours : MonoBehaviour
    {
        Renderer[] children;
        Texture2D lifeTexture;
        GameOfLifeTextureGenerator textureGenerator;

        public float updatesPerSecond = 20.0f;

        // Use this for initialization
        void Start()
        {
            children = GetComponentsInChildren<Renderer>();

            textureGenerator = FindObjectOfType<GameOfLifeTextureGenerator>();
            lifeTexture = textureGenerator.texture;
            StartCoroutine(UpdateColours());
        }

        System.Collections.IEnumerator UpdateColours()
        {
            yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
            while (true)
            {
                foreach (Renderer child in children)
                {
                    if (child.material.color.a != 1.0f)
                    {
                        continue;
                    }

                    RaycastHit hit;
                    if (Physics.Raycast(child.transform.position, -Vector3.up, out hit))
                    {
                        Color c = lifeTexture.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
                        if (c == textureGenerator.backGround)
                        {
                            c.r = 1.0f - c.r;
                            c.g = 1.0f - c.g;
                            c.b = 1.0f - c.b;
                        }
                        child.material.color = Color.Lerp(child.material.color, c, 0.1f);
                    }
                    //yield return WaitFor.Frames(1);
                }
                yield return new WaitForSeconds(1.0f / updatesPerSecond);
            }
        }
    }
}
