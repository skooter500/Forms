using UnityEngine;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class FormationGenerator : School {
        public int minSideWith = 10;
        public int maxSideWith = 10;
        public float gap = 50;

        [Range(0.0f, 1.0f)]
        public float variance = 0.1f;

        public GameObject leaderPrefab;
        public GameObject followerPrefab;

        private List<Vector3> positions = new List<Vector3>();


        void GenerateCreaturePosition(Vector3 pos, Vector3 startPos, int current, int depth)
        {
            positions.Add(pos);
            pos.z = startPos.z;
            if (current < depth)
            {
                if (pos.x <= transform.position.x)
                {
                    Vector3 left = new Vector3(-1, 0, -1) * gap;
                    left.x *= Random.Range(1.0f, 1.0f - variance);
                    left.y += gap * Random.Range(-variance, variance);
                    left.z *= - Random.Range(1.0f - variance, 1.0f + variance);
                    GenerateCreaturePosition(pos + left, startPos, current + 1, depth);
                
                }
                if (pos.x >= transform.position.x)
                {
                    Vector3 right = new Vector3(1, 0, -1) * gap;
                    right.x *= Random.Range(1.0f, 1.0f - variance);
                    right.y += gap * Random.Range(-variance, variance);
                    right.z *= - Random.Range(1.0f - variance, 1.0f + variance);
                    GenerateCreaturePosition(pos + right, startPos, current + 1, depth);
                }
            }
        }

        void GeneratePositions()
        {
            positions.Clear();
            int sideWidth = Random.Range(minSideWith, maxSideWith + 1);
            GenerateCreaturePosition(transform.position, transform.position, 0, sideWidth);
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


        // Use this for initialization
        void Start () {
            GeneratePositions();
            GameObject leader = null;

            for (int i = 0; i < positions.Count; i++)
            {
                Boid boid;
                if (i == 0)
                {
                    leader = GameObject.Instantiate<GameObject>(leaderPrefab);
                    leader.transform.position = positions[i];
                    leader.transform.parent = transform;
                    leader.SetActive(true);
                    boid = leader.GetComponentInChildren<Boid>();
                }
                else
                {
                    GameObject follower = GameObject.Instantiate<GameObject>(followerPrefab);
                    follower.transform.position = positions[i];
                    follower.transform.parent = transform;
                    follower.SetActive(true);
                    boid = follower.GetComponentInChildren<Boid>();
                    Formation formation = follower.GetComponentInChildren<Formation>();
                    if (formation == null)
                    {
                        formation = follower.GetComponentInChildren<Boid>().gameObject.AddComponent<Formation>();
                        formation.weight = 100.0f;
                    }
                    formation.leader = leader;
                    follower.GetComponentInChildren<Boid>().school = this;
                
                }
                boids.Add(boid);
            }

            Utilities.SetLayerRecursively(this.gameObject, this.gameObject.layer);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}