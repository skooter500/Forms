using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class RandomTextureGenerator:TextureGenerator
    {
        [HideInInspector]
    

        public override void GenerateTexture()
        {
            texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
                }
            }

            texture.Apply();
        }


        void Awake()
        {
            GenerateTexture();
        }    
    }
}