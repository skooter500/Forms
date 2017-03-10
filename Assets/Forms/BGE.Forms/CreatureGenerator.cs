using UnityEngine;
using System.Collections.Generic;
using System;

namespace BGE.Forms
{
    public class CreatureGenerator : MonoBehaviour {

        public bool makeRotator = false;

        public bool scaleFins = true;

        // Changes the position of the fins
        public float finRotatorOffset = 0.0f;

        public float partOffset = 0.0f;

        [Range(0.0f, Mathf.PI * 2.0f)]
        public float theta = 0.1f;

        [Range(0.0f, 10.0f)]
        public float frequency = 1.0f;


        [Range(1, 1000)]
        public int numParts = 5;

        [Range(-1000.0f, 1000.0f)]
        public float gap = 1;

        public Color color = Color.blue;
        public bool assignColors = true;

        [Range(1.0f, 5000.0f)]
        public float verticalSize = 1.0f;

        public bool flatten = false;

        public GameObject headPrefab;
        public GameObject bodyPrefab;
        public GameObject tailPrefab;
        public GameObject leftFinPrefab;
        public GameObject rightFinPrefab;
        public GameObject seatPrefab;

        public float finRotationOffset = 20.0f;

        public string finList;

        public float lengthVariation = 0;
    
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                List<CreaturePart> creatureParts = CreateCreatureParams();
                Gizmos.color = Color.yellow;
                foreach (CreaturePart cp in creatureParts)
                {
                    Gizmos.DrawWireSphere(cp.position, cp.size * 0.5f);
                }
            }
        }

        void CreateCreature()
        {
            string[] fla = finList.Split(',');
            List<CreaturePart> creatureParts = CreateCreatureParams();
            Gizmos.color = Color.yellow;
            Boid boid = null;

            int finNumber = 0;
            for (int i = 0; i < creatureParts.Count; i ++)
            {
                CreaturePart cp = creatureParts[i];
                GameObject part = GameObject.Instantiate<GameObject>(cp.prefab);
                part.transform.position = cp.position;
                if (i != 0)
                {
                    part.transform.Translate(0, 0, partOffset);
                }
            
                if (i == 0)
                {
                    boid = part.GetComponent<Boid>();
                }
                else
                {
                    part.transform.parent = transform;
                }

                Utilities.SetUpAnimators(part, boid);
            
                part.transform.localScale = new Vector3(cp.size * part.transform.localScale.x, cp.size * part.transform.localScale.y, cp.size * part.transform.localScale.z);
                part.transform.rotation = transform.rotation;
                part.transform.parent = transform;
              

                // Make fins if required            
                if (System.Array.Find(fla, p => p == "" + i) != null)
                {
                    float scale = cp.size / ((finNumber / 2) + 1);
                    GameObject leftFin = GenerateFin(scale, cp, boid, (finNumber * finRotationOffset), part, FinAnimator.Side.left);
                    GameObject rightFin = GenerateFin(scale, cp, boid, (finNumber * finRotationOffset), part, FinAnimator.Side.right);
                    finNumber++;
                }
            }
        }

        private GameObject GenerateFin(float scale, CreaturePart cp, Boid boid, float rotationOffset, GameObject part, FinAnimator.Side side)
        {
            GameObject fin = null; 
            Vector3 pos = cp.position;
            switch (side)
            {
                case FinAnimator.Side.left:
                    fin = GameObject.Instantiate<GameObject>(leftFinPrefab);
                    pos -= (transform.right * cp.size / 2);                
                    break;
                case FinAnimator.Side.right:
                    fin = GameObject.Instantiate<GameObject>(rightFinPrefab);
                    pos += (transform.right * cp.size / 2);
                    break;
            }
            fin.transform.position = pos;
            fin.transform.rotation = fin.transform.rotation * transform.rotation;
            if (scaleFins)
            {
                fin.transform.localScale = new Vector3(scale, scale, scale);
            }
            fin.GetComponentInChildren<FinAnimator>().boid = boid;
            //fin.GetComponentInChildren<FinAnimator>().side = side;
            fin.GetComponentInChildren<FinAnimator>().rotationOffset -= rotationOffset;
            fin.transform.parent = part.transform;
        
            return fin;
        }



        List<CreaturePart> CreateCreatureParams()
        {
            List<CreaturePart> cps = new List<CreaturePart>();
            float thetaInc = (Mathf.PI * frequency) / (numParts);
            float theta = this.theta;
            float lastGap = 0;
            Vector3 pos = transform.position;

            int half = (numParts / 2) - 1; 

            for (int i = 0; i < numParts; i++)
            {
                float partSize = 0;
                if (makeRotator && i == 0)
                {
                    partSize = 0.03f;
                }
                else
                {
                    partSize = verticalSize  * Mathf.Abs(Mathf.Sin(theta)) + (verticalSize * lengthVariation * UnityEngine.Random.Range(0.0f, 1.0f));
                    theta += thetaInc;
                }
                pos -= ((((lastGap + partSize) / 2.0f) + gap) * transform.forward);
                if (flatten)
                {
                    pos.y -= (partSize / 2);
                }
                lastGap = partSize;
                if (i == half && seatPrefab != null)
                {
                    cps.Add(new CreaturePart(pos
                        , partSize
                        , CreaturePart.Part.seat
                        , seatPrefab
                        , Quaternion.identity));
                }
                else
                {
                    cps.Add(new CreaturePart(pos
                        , partSize
                        , (i == 0) ? CreaturePart.Part.head : (i < numParts - 1) ? CreaturePart.Part.body : CreaturePart.Part.tail
                        , (i == 0) ? headPrefab : (i < numParts - 1) ? bodyPrefab : (tailPrefab != null) ? tailPrefab : bodyPrefab
                        , Quaternion.identity));

                }

            }
            return cps;
        }

        // Use this for initialization
        void Awake() {
            // For some reason this method is being called twice
            if (transform.childCount == 0)
            {
                //Debug.Log(gameObject + " called awake. I have " + transform.childCount + " children");
                CreateCreature();
            }
        }

        void Start()
        {
            Utilities.SetLayerRecursively(this.gameObject, this.gameObject.layer);
            if (assignColors)
            {
                Utilities.RecursiveSetColor(this.gameObject, color);
            }
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }

    struct CreaturePart
    {
        public Vector3 position;
        public Quaternion rotation;
        public float size;
        public enum Part { head, body, fin , tail, tenticle, seat};
        public Part part;
        public GameObject prefab;
    
        public CreaturePart(Vector3 position, float scale, Part part, GameObject prefab, Quaternion rotation)
        {
            this.position = position;
            this.size = scale;
            this.part = part;
            this.prefab = prefab;
            this.rotation = rotation;
        }
    }
}