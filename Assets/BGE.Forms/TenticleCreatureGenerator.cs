using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class TenticleCreatureGenerator : MonoBehaviour {
        public GameObject tenticlePrefab;
        public GameObject headPrefab;

        public GameObject head;

        public int numTenticles = 8;
        public float radius = 20;
        public float headScale = 1.0f;
        public float tenticleScale = 1.0f;

        public float tenticleAngle = 0;

        public Color color;
        public bool assignColors = true;

        public void OnDrawGizmos()
        {
            List<CreaturePart> cps = CreateCreatureParams();
            Gizmos.color = Color.cyan;
            foreach (CreaturePart cp in cps)
            {
                switch (cp.part)
                {
                    case CreaturePart.Part.head:
                        Gizmos.DrawWireSphere(cp.position, cp.size);
                        break;
                    case CreaturePart.Part.tenticle:
                        Gizmos.DrawWireSphere(cp.position, cp.size / 10);
                        break;
                }
            }

        }

        List<CreaturePart> CreateCreatureParams()
        {
            List<CreaturePart> list = new List<CreaturePart>();

            if (headPrefab != null)
            {
                CreaturePart headPart = new CreaturePart(transform.position, headScale, CreaturePart.Part.head, headPrefab, headPrefab.transform.rotation);
                list.Add(headPart);
            }
            float thetaInc = Mathf.PI * 2.0f / (numTenticles);
            for (int i = 0; i < numTenticles; i++)
            {
                float theta = i * thetaInc;
                Vector3 pos = new Vector3();
                pos.x = Mathf.Sin(theta) * radius;
                pos.z = - Mathf.Cos(theta) * radius;
                pos.y = 0;
                pos += transform.position;

                Quaternion q = Quaternion.identity;
                q.eulerAngles = new Vector3(- tenticleAngle, Mathf.Rad2Deg * -theta, 0); // Quaternion.AngleAxis(Mathf.Rad2Deg * -theta, Vector3.up) * Quaternion.;
                CreaturePart cp = new CreaturePart(pos, tenticleScale
                    , CreaturePart.Part.tenticle
                    , tenticlePrefab, q);
                list.Add(cp);
            }

            return list;
        }

        void CreateCreature()
        {
            List<CreaturePart> parts = CreateCreatureParams();
            Boid boid = null;
            GameObject temp = new GameObject();
            temp.transform.position = transform.position;
            for(int i = 0; i < parts.Count; i ++)
            {
                CreaturePart part = parts[i];
            
                GameObject newPart = GameObject.Instantiate<GameObject>(part.prefab);            
                newPart.transform.position = part.position;
                newPart.transform.rotation = part.rotation;
                newPart.transform.localScale = new Vector3(part.size, part.size, part.size);
                
                newPart.transform.parent = temp.transform;          

                Boid thisBoid = newPart.GetComponentInChildren<Boid>();
                if (thisBoid != null)
                {
                    boid = thisBoid;
                    newPart.transform.parent = boid.transform.GetChild(0).transform;
                    head = boid.gameObject;
                }

                FinAnimator anim = newPart.GetComponentInChildren<FinAnimator>();
                if (anim != null)
                {
                    newPart.transform.parent = boid.transform.GetChild(0).transform;
                    anim.boid = boid;
                }
                newPart.SetActive(true);
            }
            // A hacky trick to rotate everything
            temp.transform.Rotate(transform.rotation.eulerAngles);
            temp.transform.parent = transform;

            /*
            for (int i = 0; i < temp.transform.childCount; i++)
            {
                temp.transform.GetChild(i).transform.parent = transform;
            }
            GameObject.Destroy(temp);
            */




            while(temp.transform.childCount > 0)
            {
                temp.transform.GetChild(0).transform.parent = transform;
            }
            GameObject.Destroy(temp);
        }

        void Awake()
        {
            CreateCreature();
        }

    

        // Use this for initialization
        void Start()
        {
            if (assignColors)
            {
                Utilities.RecursiveSetColor(this.gameObject, color);
            }
            Utilities.SetLayerRecursively(this.gameObject, this.gameObject.layer);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
 