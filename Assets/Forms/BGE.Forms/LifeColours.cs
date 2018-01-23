using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class LifeColours : MonoBehaviour
    {
        public Material material;
        Renderer[] children;
        int size = 256;

        public float colorMapScaling = 50;

        public Texture2D texture;
        public float colorScale = 0.7f;

        void OnValidate()
        {
        }

        // Use this for initialization
        void Start()
        {
            children = GetComponentsInChildren<Renderer>();
            texture = new Texture2D(size, size);

            int halfSize = size / 2;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float hue = ((row / (float)size) * colorScale) + ((col / (float)size) * colorScale) / 2.0f;
                    texture.SetPixel(row, col, Color.HSVToRGB(hue, 1, 0.8f));
                }
            }
            texture.Apply();
            texture.wrapMode = TextureWrapMode.Mirror;

            foreach (Renderer child in children)
            {
                if (child.material.name.Contains("Trans"))
                {
                    continue;
                }
                child.material = material;
                child.material.SetFloat("_PositionScale", colorMapScaling);
                child.material.mainTexture = texture;                
            }
        }
    }
}
