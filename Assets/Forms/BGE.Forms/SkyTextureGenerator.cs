using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class SkyTextureGenerator : MonoBehaviour {

        // Use this for initialization
        void Start () {
            GameOfLifeTextureGenerator gen = GetComponent<GameOfLifeTextureGenerator>();
            //gen.Randomise();
            Renderer r = GetComponent<Renderer>();
            Texture2D tex = gen.texture;
            r.material.SetTexture("_MainTex", tex);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}