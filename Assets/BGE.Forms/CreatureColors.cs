using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class CreatureColors : MonoBehaviour
    {
        public GameObject root;
        public int cSeed = 42;
        public int bSeed = 42;

        [Range(0.0f, 1.0f)]
        public float h = 0.25f;

        [Range(2, 20)]
        public int numCols = 5;

        [Range(0, 20)]
        public int rotation = 0;

        // Use this for initialization
        void Start()
        {
            RecolorScene();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                bSeed++;
                RecolorScene();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                bSeed--;
                RecolorScene();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                cSeed++;
                RecolorScene();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                cSeed--;
                RecolorScene();
            }


        }

        private List<Renderer> renderers;

        List<Renderer> GetRenderers()
        {
            return renderers ?? (renderers = Utilities.GetRenderersinChildrenRecursive(root));
        }

        private TrailRenderer[] trailRenderers;

        TrailRenderer[] GetTrailRendereres()
        {
            return trailRenderers ?? (trailRenderers = FindObjectsOfType<TrailRenderer>());
        }

        void RecolorScene()
        {
            List<Renderer> rs = GetRenderers();
            Palette p = new Palette(cSeed, bSeed, 10);
            
            foreach (Renderer r in rs)
            {

                if (r.materials[0].name.Contains("Trans"))
                {
                    continue;
                }

                // The square fish
                if (r.gameObject.layer == 9)
                {
                    r.material.color = p.creatureColors[1];
                }

                // The big blues
                if (r.gameObject.layer == 10)
                {
                    r.material.color = p.creatureColors[0];
                }

                // The Tenticle creatures
                if (r.gameObject.layer == 12)
                {
                    r.material.color = p.creatureColors[2];
                }

                // The Formation
                if (r.gameObject.layer == 13)
                {
                    r.material.color = p.creatureColors[3];
                }

                // The Flying Creatures
                if (r.gameObject.layer == 14)
                {
                    r.material.color = p.creatureColors[4];
                }
                // The Tenticle Flowers
                if (r.gameObject.layer == 15)
                {
                    r.material.color = p.creatureColors[5];
                }

                // The Sardines
                if (r.gameObject.layer == 16)
                {
                    r.material.color = p.creatureColors[8];
                    TrailRenderer[] trs = GetTrailRendereres();
                    foreach (var tr in trs)
                    {
                        tr.material.SetColor("_TintColor", p.creatureColors[9]);
                    }
                }

            }
            GameOfLifeTextureGenerator tg = FindObjectOfType<GameOfLifeTextureGenerator>();
            if (tg != null)
            {
                //tg.backGround = p.backColors[0];
                //tg.foreGround = p.backColors[1];
            }

            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (var c in cameras)
            {
                c.backgroundColor = p.backColors[2];
                RenderSettings.fogColor = p.backColors[2];
            }
        }
    }
}