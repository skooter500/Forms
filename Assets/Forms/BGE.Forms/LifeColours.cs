using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class LifeColours : MonoBehaviour
    {
        public Material creatureTextureMaker;
        public Material material;
        Renderer[] children;
        int size = 256;

        public float colorMapScaling = 50;

        public Texture texture;
        public float colorScale = 0.7f;

        //public RenderTexture texture;
        private RenderTexture buffer;

        public enum TextureMode { Shader, CSharp }

        public TextureMode textureMode = TextureMode.Shader;

        public void UpdateTexture()
        {
            Graphics.Blit(texture, buffer, material);
            Graphics.Blit(buffer, (RenderTexture)texture);
        }

        private void InitializeShaderTexture()
        {
            RenderTexture renderTexture = (RenderTexture)texture;
            buffer = new RenderTexture(renderTexture.width
                , renderTexture.height
                , renderTexture.depth
                , renderTexture.format
                );

            // Run the shader to generate the texture
            creatureTextureMaker.SetFloat("_ColourScale", colorScale);
            Graphics.Blit(texture, buffer, creatureTextureMaker);
            Graphics.Blit(buffer, renderTexture);
        }

        private void InitializeProgrammableTexture()
        {
            Texture2D programmableTexture = new Texture2D(size, size);
            texture = programmableTexture;

            int halfSize = size / 2;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float hue = Utilities.Map(row + col, 0, (size * 2) - 2, 0, colorScale);
                    // ((row / (float)size) * colorScale) + ((col / (float)size) * colorScale) / 2.0f;
                    programmableTexture.SetPixel(row, col, Color.HSVToRGB(hue, 1, 0.8f));
                }
            }
            programmableTexture.Apply();
            texture.wrapMode = TextureWrapMode.Mirror;

        }


        // Use this for initialization
        void Start()
        {
            children = GetComponentsInChildren<Renderer>();

            if (textureMode == TextureMode.Shader)
            {
                InitializeShaderTexture();
            }
            else
            {
                InitializeProgrammableTexture();
            }

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
            FadeIn();
        }

        public void FadeIn()
        {
            StartCoroutine(FadeInCoRoutine());
        }

        System.Collections.IEnumerator FadeInCoRoutine()
        {
            float alpha = 0;
            while (alpha < 1.0f)
            {
                foreach (Renderer child in children)
                {
                    if (child.material.name.Contains("Trans"))
                    {
                        continue;
                    }
                    else
                    {
                        child.material.SetFloat("_Fade", alpha);
                    }
                }
                alpha += Time.deltaTime;
                CreatureManager.Log("Alpha:" + alpha);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
