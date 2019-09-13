using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGE.Forms
{
    public class LifeColours : MonoBehaviour
    {
        public Material transMaterial;
        public Material opaqueMaterial;
        Renderer[] children;
        int size = 256;

        public float colorMapScaling = 50;

        public Texture texture;
        public static Texture genTexture;
        public Texture2D particleTexture;
        public float colorScale = 0.7f;

        //public RenderTexture texture;
        private RenderTexture buffer;

        public enum TextureMode { Shader, CSharp }

        public TextureMode textureMode = TextureMode.CSharp;

        public float targetAlpha = 1.0f;

        public float targetFade = 1.0f;
        public float startFade = 0.0f;
        
        public bool waitAFrame = false;

        private TextureGenerator tg;

        public void UpdateTexture()
        {
            Graphics.Blit(texture, buffer, transMaterial);
            Graphics.Blit(buffer, (RenderTexture)texture);
        }

        /*
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
        */

        private void InitializeProgrammableTexture()
        {
            if (texture == null)
            {
                
                Texture2D programmableTexture = new Texture2D(size, size);
                genTexture = programmableTexture;

                int halfSize = size / 2;
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {
                        float hue = Utilities.Map(row + col, 0, (size * 2) - 2, 0, colorScale);
                        // ((row / (float)size) * colorScale) + ((col / (float)size) * colorScale) / 2.0f;
                        programmableTexture.SetPixel(row, col, Color.HSVToRGB(hue, 0.9f, 0.8f));
                    }
                }
                programmableTexture.Apply();
                genTexture.wrapMode = TextureWrapMode.Mirror;
            }

            switch (textureSource)
            {
                case TextureSource.Gradient:
                    texture = genTexture;
                    break;
                case TextureSource.PrimaryLife:
                    texture = GameOfLifeTextureGenerator.Instance.texture;
                    break;
                case TextureSource.SecondaryLife:
                    texture = GameOfLifeTextureGenerator.Instance.secondaryTexture;
                    break;
            }
        }

        public enum TextureSource { Gradient, PrimaryLife, SecondaryLife}
        public TextureSource textureSource = TextureSource.Gradient;
        
        // Use this for initialization
        void Start()
        {
            //tg = GetComponent<TextureGenerator>();
            InitializeProgrammableTexture();
         
            FadeIn();
        }


        private Coroutine fadeInCoroutine;

        public void FadeIn()
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
            }
            startFade = 0;
            targetFade = targetAlpha;
            fadeInCoroutine = StartCoroutine(FadeInCoRoutine());
        }

        public void FadeOut()
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
            }
            startFade = targetAlpha;
            targetFade = 0.0f;
            fadeInCoroutine = StartCoroutine(FadeInCoRoutine());
        }

        System.Collections.IEnumerator FadeInCoRoutine()
        {
            if (waitAFrame) yield return null;
            children = GetComponentsInChildren<Renderer>();            
            foreach (Renderer child in children)
            {
                child.enabled = true;
                if (child.material.name.Contains("Trans"))
                {
                    continue;
                }
                child.material = transMaterial;
                child.material.SetFloat("_PositionScale", colorMapScaling);
                if (child.material.HasProperty("_ColorTex"))
                {
                    child.material.mainTexture = particleTexture;
                    child.material.SetTexture("_ColorTex", texture);
                }
                else
                {
                    child.material.mainTexture = texture;
                }
                child.material.SetTexture("_EmissionMap", texture);
                child.material.SetFloat("_Fade", startFade);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            float fade = startFade;
            float delta = 0.1f;
            if (startFade == 1)
            {
                delta = -0.1f;
            }
            while (Mathf.Abs(fade - targetFade) > 0.01f)
            {
                foreach (Renderer child in children)
                {
                    if (child.material.name.Contains("Trans"))
                    {
                        continue;
                    }
                    else
                    {
                        child.material.SetFloat("_Fade", fade);
                    }
                }
                fade += delta / 3.0f;
                yield return new WaitForSeconds(delta);
            }
            if (targetFade == 1)
            {
                foreach (Renderer child in children)
                {
                    if (child.material.name.Contains("Trans"))
                    {
                        continue;
                    }
                    else
                    {
                        float offs = child.material.GetFloat("_Offset");
                        child.material = opaqueMaterial;
                        child.material.SetFloat("_Offset", offs);
                        child.material.SetFloat("_PositionScale", colorMapScaling);
                        //child.material.SetTexture("_EmissionMap", texture);
                        child.material.mainTexture = texture;
                        child.material.SetFloat("_Fade", targetFade);
                    }
                }
            }
        }
    }
}
