using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class FormationGenerator : School
    {
        public int minSideWith = 10;
        public float gap = 50;

        [Range(0.0f, 1.0f)]
        public float variance = 0.1f;

        public GameObject leaderPrefab;
        public GameObject followerPrefab;

        private List<Vector3> positions = new List<Vector3>();

        [HideInInspector]
        public GameObject leader;
        [HideInInspector]
        public List<GameObject> followers = new List<GameObject>();

        private int sideWidth;

        private Vector3 VaryLocalPosition(Vector3 pos)
        {
            pos.x *= Random.Range(1.0f, 1.0f - variance);
            pos.y += gap * Random.Range(-variance, variance);
            pos.z *= Random.Range(1.0f - variance, 1.0f + variance);

            return pos;
        }
        
        public void GeneratePositions()
        {
            sideWidth = minSideWith;
            positions.Clear();
            positions.Add(transform.position);
            for (int i = 1; i <= sideWidth; i++)
            {
                Vector3 offset = VaryLocalPosition(new Vector3(gap * i, 0, -gap * i));
                positions.Add(transform.TransformPoint(offset));

                offset = VaryLocalPosition(new Vector3(-gap * i, 0, -gap * i));
                positions.Add(transform.TransformPoint(offset));
            }
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                GeneratePositions();
                if (isActiveAndEnabled)
                {
                    foreach (Vector3 pos in positions)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawCube(pos, Vector3.one * gap * 0.5f);
                    }
                }
            }
        }

        GameObject ClosestChild(GameObject go)
        {
            float closest = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != go)
                {
                    float distance = Vector3.Distance(go.transform.position, child.transform.position);
                    if (distance < closest)
                    {
                        closestIndex = i;
                        closest = distance;
                    }
                }
            }
            return transform.GetChild(closestIndex).gameObject;
        }

        public void Teleport()
        {
            leader.transform.position = positions[0];
            Boid b = Utilities.FindBoidInHierarchy(leader);
            b.suspended = false;
            b.position = positions[0];
            /* float y = WorldGenerator.Instance.SamplePos(b.position.x, b.position.z);
            if (b.position.y < y + height)
            {
                b.position.y = y + height;
            }
            */
            b.transform.position = b.position;
            b.desiredPosition = b.position;
            int i = 1;
            foreach (GameObject follower in followers)
            {
                follower.transform.position = positions[i];
                Boid bb = Utilities.FindBoidInHierarchy(follower);
                bb.suspended = false;
                bb.position = positions[i];
                /* float y = WorldGenerator.Instance.SamplePos(b.position.x, b.position.z);
                if (b.position.y < y + height)
                {
                    b.position.y = y + height;
                }
                */
                i++;
                bb.transform.position = bb.position;
                bb.desiredPosition = bb.position;
                if (bb.GetComponent<Formation>())
                {
                    bb.GetComponent<Formation>().RecalculateOffset();
                }
            }
        }


        // Use this for initialization
        void Start()
        {
            GeneratePositions();

            for (int i = 0; i < positions.Count; i++)
            {
                Boid boid;
                if (i == 0)
                {
                    leader = GameObject.Instantiate<GameObject>(leaderPrefab);
                    leader.transform.position = positions[i];
                    leader.transform.rotation = this.transform.rotation;
                    leader.transform.parent = transform;
                    leader.SetActive(true);
                    boid = leader.GetComponentInChildren<Boid>();
                }
                else
                {
                    GameObject follower = GameObject.Instantiate<GameObject>(followerPrefab);
                    follower.transform.position = positions[i];
                    follower.transform.rotation = this.transform.rotation;
                    follower.transform.parent = transform;
                    follower.SetActive(true);
                    boid = follower.GetComponentInChildren<Boid>();
                    followers.Add(follower);
                    Formation formation = follower.GetComponentInChildren<Formation>();
                    if (formation == null)
                    {
                        formation = follower.GetComponentInChildren<Boid>().gameObject.AddComponent<Formation>();
                        formation.weight = 100.0f;
                    }
                    formation.leader = leader;
                    formation.leaderBoid = leader.GetComponentInChildren<Boid>();
                    follower.GetComponentInChildren<Boid>().school = this;

                }
                boids.Add(boid);
            }

            Utilities.SetLayerRecursively(this.gameObject, this.gameObject.layer);
        }
    }
}