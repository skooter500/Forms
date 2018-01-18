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

        Color[,] colorGrid;
        int size;

        public float colorMapScaling = 250;

        public Texture2D texture;
        public float colorScale = 0.7f;

        // Use this for initialization
        void Start()
        {
            children = GetComponentsInChildren<Renderer>();

            textureGenerator = FindObjectOfType<GameOfLifeTextureGenerator>();
            lifeTexture = textureGenerator.texture;

            size = textureGenerator.size;
            colorGrid = new Color[size, size];
            texture = new Texture2D(size, size);
            
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float hue = (row / (float) size * colorScale) + (col / (float) size * colorScale) / 2.0f;
                    colorGrid[row, col] = Color.HSVToRGB(hue, 1, 0.8f);

                    texture.SetPixel(row, col, colorGrid[row, col]);
                }
            }
            texture.Apply();            
        }

        private void OnEnable()
        {
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

                    /*
                    int col = ((int)(Mathf.Abs(child.transform.position.x / colorMapScaling))) % size;
                    int row = ((int)(Mathf.Abs(child.transform.position.z / colorMapScaling))) % size;

                    Color c; = colorGrid[row, col];
                    */


                    int col = ((int)(Mathf.Abs(child.transform.position.x / colorMapScaling))) % size;
                    int row = ((int)(Mathf.Abs(child.transform.position.z / colorMapScaling))) % size;

                    Color c = colorGrid[row, col];
                    child.material.color = Color.Lerp(child.material.color, c, 0.01f);

                    /*
                    RaycastHit hit;
                    if (Physics.Raycast(child.transform.position, -Vector3.up, out hit))
                    {

                        Color c = Color.black;
                        int col = ((int)(Mathf.Abs(child.transform.position.x / 200))) % size;
                        int row = ((int)(Mathf.Abs(child.transform.position.z / 200))) % size;


                        //c = lifeTexture.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
                        if (c == textureGenerator.backGround)
                        {
                            c = colorGrid[row, col];
                            child.material.color = Color.Lerp(child.material.color, c, 0.01f);
                        }
                        else
                        {
                            c =  lifeTexture.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
                            child.material.color = Color.Lerp(child.material.color, c, 0.1f);

                        }
                    }     
                    */               
                    //yield return WaitFor.Frames(1);
                }
                yield return new WaitForSeconds(1.0f / updatesPerSecond);
            }
        }
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class LifeColours : MonoBehaviour
    {
        Renderer[] children;
        Texture2D lifeTexture;
        GameOfLifeTextureGenerator textureGenerator;

        Color[,] colourGrid;

        public float updatesPerSecond = 20.0f;
        int size;
        // Use this for initialization
        void Start()
        {
            children = GetComponentsInChildren<Renderer>();

            textureGenerator = FindObjectOfType<GameOfLifeTextureGenerator>();
            lifeTexture = textureGenerator.texture;
            size = textureGenerator.size;
            colourGrid = new Color[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    colourGrid[row, col] = textureGenerator.RandomColor();
                }
            }

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

                        //int col = ((int)(Mathf.Abs(child.transform.position.x))) % size;
                        //int row = ((int)(Mathf.Abs(child.transform.position.z))) % size;

                        //child.material.color = Color.Lerp(child.material.color, colourGrid[row, col], 0.1f);

                        if (c == textureGenerator.backGround)
                        {
                            c.r = 1.0f - c.r;
                            c.g = 1.0f - c.g;
                            c.b = 1.0f - c.b;
                        }
                        child.material.color = Color.Lerp(child.material.color, c, 0.1f);
                    }
                    
                    //yield return WaitFor.Frames(1);
                    yield return new WaitForSeconds(1.0f / updatesPerSecond);
                }
            }
        }
    }
}
*/
