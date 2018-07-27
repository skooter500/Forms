using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public abstract class TextureGenerator : MonoBehaviour
    {
        public Texture2D texture;

        public int size;
        public abstract void GenerateTexture();    
    }
}