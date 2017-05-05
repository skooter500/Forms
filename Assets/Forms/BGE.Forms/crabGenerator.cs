using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGE.Forms
{
    public class crabGenerator : MonoBehaviour
    {

        public GameObject headPrefab;
        public GameObject bodyPrefab;
        public GameObject ClawPrefab;
        public GameObject leftLegPrefab;
        public GameObject rightLegPrefab;
        public GameObject tailPrefab;
        public GameObject tailFinPrefab;

        [Range(0.1f, 5000.0f)]//We want to be able to create very small creatures
        public float verticalSize = 1.0f;

        public bool scaleLimbs = true;

        public float limbOffset = 0.0f;

        public float partOffset = 0.0f;

        private float theta = 0.1f;

        private float lengthVariation = 0;

        [Range(0.0f, 10.0f)]
        public float frequency = 1.0f;
        //How many extensions of the tail do we want
        [Range(0, 1000)]
        public int numPartsTail = 0;

        [Range(0, 1000)]
        public int numTailFin = 0;

        //body part gaps
        [Range(-1000.0f, 1000.0f)]
        public float gap = 1;

        [Range(1, 1000.0f)]
        public float numLimbPairs = 1;

        public string clawList;//The limbs that will have claws

        public Color color = Color.red;
        public bool assignColors = true;

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

        //Where we assemble our creatures using the values given
        void CreateCreature()
        {
            string[] cla = { "" };
            if (clawList != null)
            {
                cla = clawList.Split(',');

                for (int i = 0; i < cla.Length; i++)
                {
                    if (Convert.ToInt32(cla[i]) > numLimbPairs)
                    {
                        print("Value exceeds limb numbers. Removing");
                        cla[i].Remove(i);
                    }
                }
            }

            //ensure there are not claws beyond limbs

            //now lets create a list of the creatures parts
            List<CreaturePart> creatureParts = CreateCreatureParams();

            Gizmos.color = Color.red;

            Boid boid = null;

            for (int i = 0; i < creatureParts.Count; i++)
            {
                CreaturePart cp = creatureParts[i];
                GameObject part = GameObject.Instantiate<GameObject>(cp.prefab);
                part.transform.position = cp.position;
                if (i != 0)
                {
                    part.transform.Translate(0, 0, gap);
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

                if (numLimbPairs > 1 && i == 1)//i == 1 is the body component
                {

                    float legScale = cp.size / numLimbPairs * 2; //for our limbs we want them to be longer when the crab has less and shorter when the crab has many limbs

                    for (int x = 0; x < numLimbPairs; x++)
                    {
                        //Spacing is how we will place our limbs based on the body block.
                        bool claw = false;
                        //Now we need to check if this set has claws!
                        if (cla != null || cla[0] != "")
                        {
                            if (System.Array.Find(cla, p => p == "" + i) != null)
                            {
                                claw = true;
                            }
                        }
                        //Scaling the limbs is done via number of limbs, more legs = smaller legs
                        float scale = (cp.size*2)/(numLimbPairs*0.5f);

                        //To give our limbs some randomness we will alter the limboffset
                        limbOffset = limbOffset * UnityEngine.Random.Range(1.0f, 5.0f);
                        GameObject leftLimb = GenerateLimb(scale, cp, boid, (x * limbOffset), part, limbAnimator.Side.left, x, claw);
                        GameObject rightLimb = GenerateLimb(scale, cp, boid, (x * limbOffset), part, limbAnimator.Side.right, x, claw);
                    }
                }
            }
        }
                        

        private GameObject GenerateLimb(float scale, CreaturePart cp, BGE.Forms.Boid boid, float rotationOffset, GameObject part, limbAnimator.Side side, int spacing, bool claw)
        {
            GameObject limb = null;
            GameObject cpc = part.transform.GetChild(0).gameObject;
            Vector3 pos = cpc.transform.position;

            //to find out where to place the legs we need to know the length of the body gameObject
            Bounds bodyBounds = cpc.GetComponent<Renderer>().bounds;
            float bodylength = bodyBounds.extents.magnitude*2;
            float limbPlace = bodylength / (numLimbPairs*2);
            pos.z = pos.z - bodylength/(bodyBounds.extents.magnitude*8);
            switch (side)
            {
                case limbAnimator.Side.left :
                    limb = GameObject.Instantiate<GameObject>(leftLegPrefab);
                    pos += (transform.right * (cp.size+0.5f) / 2);//Put the limbs on the left side
                    pos.z = pos.z + (limbPlace*spacing);
                   // pos = (transform.up + spacing * cp.size);//Puts the limb further down the body
                    break;
                case limbAnimator.Side.right:
                    limb = GameObject.Instantiate<GameObject>(rightLegPrefab);
                    pos -= (transform.right * (cp.size + 0.5f) / 2);//Put the limbs on the right side
                    pos.z = pos.z + (limbPlace * spacing);
                    // pos = (transform.up + spacing * cp.size);//Puts the limb further down the body
                    break;
            }
            limb.transform.localScale = new Vector3(scale, scale, scale);
            limb.transform.position = pos;
            limb.transform.rotation = limb.transform.rotation * transform.rotation;

            limb.GetComponentInChildren<limbAnimator>().boid = boid;
            limb.GetComponentInChildren<limbAnimator>().rotationOffset -= rotationOffset;
            limb.GetComponentInChildren<limbAnimator>().wigglyness = limb.GetComponentInChildren<limbAnimator>().wigglyness * numLimbPairs;
            limb.transform.parent = part.transform;

            //Now that we have the Limb complete we need to attach the claw or lower limb
            //THIS WAS NOT IMPLEMENTED
            if(claw)
            {
                GameObject clawGO = null;
                Vector3 clawPos = (limb.transform.up);//The bottom of the limb
                switch (side)
                {
                    case limbAnimator.Side.left:
                        break;
                    case limbAnimator.Side.right:
                        break;
                }
                clawGO.transform.position = clawPos;
                clawGO.transform.rotation = clawGO.transform.rotation * limb.transform.rotation;
                clawGO.GetComponentInChildren<clawAnimator>().boid = boid;
                clawGO.GetComponentInChildren<clawAnimator>().rotationOffset -= rotationOffset;
                clawGO.transform.parent = limb.transform;
            }
            else
            {
                //Now we add the next Leg piece
            }
            return limb;
        }

        List<CreaturePart> CreateCreatureParams()
        {
            List<CreaturePart> cps = new List<CreaturePart>();
            
            //As we need to add the head and body we redefine the num parts
            int totalParts = numPartsTail + 2;
            float thetaInc = (Mathf.PI * frequency) / (totalParts);
            
            float theta = this.theta;
            float lastGap = 0;
            Vector3 pos = transform.position;

            for (int i = 0; i < totalParts; i++)
            {
                float partSize = 0;

                partSize = verticalSize * Mathf.Abs(Mathf.Sin(theta)) + (verticalSize * lengthVariation * UnityEngine.Random.Range(0.0f, 1.0f));
                theta += thetaInc;

                pos -= ((((lastGap + partSize) / 2.0f) + gap) * transform.forward);
                lastGap = partSize;
                //No need for seat

                //OLDER GENERATOR
               /* cps.Add(new CreaturePart(pos
                           , partSize
                           , (i == 0) ? CreaturePart.Part.head : (i < totalParts - 1) ? CreaturePart.Part.body : CreaturePart.Part.tail
                           , (i == 0) ? headPrefab : (i < totalParts - 1) ? bodyPrefab : (tailPrefab != null) ? tailPrefab : bodyPrefab
                           , Quaternion.identity));*/

                cps.Add(new CreaturePart(pos
                           , partSize
                           , (i == 0) ? CreaturePart.Part.head : (i == 1) ? CreaturePart.Part.body : CreaturePart.Part.tail
                           , (i == 0) ? headPrefab : (i == 1) ? bodyPrefab : (tailPrefab != null && i < totalParts) ? tailPrefab : bodyPrefab
                           , Quaternion.identity));
            }
            return cps;
        }

        // Use this for initialization
        void Awake()
        {
            if (transform.childCount == 0)
            {
                CreateCreature();
            }
        }

        void Start()
        {
            Utilities.SetLayerRecursively(this.gameObject, this.gameObject.layer);
            if(assignColors)
            {
                Utilities.RecursiveSetColor(this.gameObject, color);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        struct CreaturePart
        {
            public Vector3 position;
            public Quaternion rotation;
            public float size;
            public enum Part { head, body, limb, tail, seat };
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
}