using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    /// <summary>
    /// Used in the BeatBasedGame scene, a ring that intercepts falling orbs.
    /// </summary>
    public class Ring : MonoBehaviour
    {

        public string key; // the key to press
        public Color readyColor; // color of the sprite when it's clickable
        public string successEffect; // an effect to grab from the object pooler

        SpriteRenderer spriteRenderer;
        bool clickAble = false; //is there currently an orb inside me?
        GameObject lastOrb; // track the last orb to pass through the ring
        Vector3 startScale;
        float highlightScale = 1.25f;

        // Use this for initialization
        void Start()
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
            startScale = this.transform.localScale;
        }

        
        private void Update()
        {
            //detect input, if there is an orb in our vicinity
            if (clickAble)
            {
                if (Input.GetKeyDown(key))
                {
                    //Debug.Log("Got it!");
                    //enable a particle effect
                    GameObject effect = ObjectPooler.instance.GetObject(successEffect);
                    effect.transform.position = this.transform.position;
                    effect.SetActive(true);
                    //disable the orb
                    lastOrb.SetActive(false);

                    Highlight(false);
                }
            }
        }

        //detect collisions with orbs
        void OnTriggerEnter(Collider col)
        {
            lastOrb = col.gameObject;
            Highlight(true);
        }


        void OnTriggerExit(Collider col)
        {
            lastOrb = null;
            Highlight(false);
        }

        //highlight the ring to indicate it's ready to be clicked
        void Highlight(bool highlight)
        {
            if(highlight)
            {
                spriteRenderer.color = readyColor;
                clickAble = true;
                this.transform.localScale = startScale * highlightScale;
            }
            else
            {
                spriteRenderer.color = Color.white;
                clickAble = false;
                this.transform.localScale = startScale;
            }
        }



    }
}
