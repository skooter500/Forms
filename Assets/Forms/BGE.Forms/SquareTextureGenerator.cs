using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class SquareTextureGenerator : TextureGenerator{

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public override void GenerateTexture()
        {
            texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < size; x++)
            {
                //texture.SetPixel(x, x, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
                //texture.SetPixel(x, size - x, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
                texture.SetPixel(x, 0, Color.blue);
                texture.SetPixel(0, x, Color.blue);
                texture.SetPixel(x, size - 1, Color.red);
                texture.SetPixel(size - 1, x, Color.red);
            }

            texture.Apply();
        }

        void Awake()
        {
            GenerateTexture();
        }
    }
}