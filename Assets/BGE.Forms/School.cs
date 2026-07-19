
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

namespace BGE.Forms
{
    public class SpatialGrid
    {
        private readonly Dictionary<int, List<Boid>> cells = new Dictionary<int, List<Boid>>();
        private float inverseCellSize;

        public void Rebuild(List<Boid> boids, int count, float cellSize)
        {
            inverseCellSize = 1f / cellSize;
            foreach (var kvp in cells)
                kvp.Value.Clear();

            for (int i = 0; i < count; i++)
            {
                Boid b = boids[i];
                if (b == null || b.suspended) continue;
                int key = HashPos(b.position);
                List<Boid> cell;
                if (!cells.TryGetValue(key, out cell))
                {
                    cell = new List<Boid>(8);
                    cells[key] = cell;
                }
                cell.Add(b);
            }
        }

        public void Query(Vector3 pos, float range, List<Boid> results, Boid exclude)
        {
            results.Clear();
            float rangeSq = range * range;
            int x0 = (int)Math.Floor((pos.x - range) * inverseCellSize);
            int x1 = (int)Math.Floor((pos.x + range) * inverseCellSize);
            int y0 = (int)Math.Floor((pos.y - range) * inverseCellSize);
            int y1 = (int)Math.Floor((pos.y + range) * inverseCellSize);
            int z0 = (int)Math.Floor((pos.z - range) * inverseCellSize);
            int z1 = (int)Math.Floor((pos.z + range) * inverseCellSize);

            for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            for (int z = z0; z <= z1; z++)
            {
                List<Boid> cell;
                if (cells.TryGetValue(HashCell(x, y, z), out cell))
                {
                    for (int i = 0; i < cell.Count; i++)
                    {
                        Boid b = cell[i];
                        if (b != exclude && (pos - b.position).sqrMagnitude < rangeSq)
                            results.Add(b);
                    }
                }
            }
        }

        private int HashPos(Vector3 pos)
        {
            return HashCell(
                (int)Math.Floor(pos.x * inverseCellSize),
                (int)Math.Floor(pos.y * inverseCellSize),
                (int)Math.Floor(pos.z * inverseCellSize));
        }

        private static int HashCell(int x, int y, int z)
        {
            unchecked { return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791); }
        }
    }

    public class School: MonoBehaviour
    {
        public float centerOfMassUpdatePerSecond = 1.0f;

        [HideInInspector]
        public Vector3 centerOfMass = Vector3.zero;

        public float neighbourDistance;

        public readonly SpatialGrid grid = new SpatialGrid();

        public void RebuildGrid()
        {
            if (neighbourDistance <= 0) return;
            int count = boids.Count; // snapshot count to avoid race if main thread adds boids
            grid.Rebuild(boids, count, neighbourDistance);
        }

        public float radius = 100;

        //[HideInInspector]
        public volatile List<Boid> boids = new List<Boid>();

        [Range(0, 2)]
        public float timeMultiplier = 1.0f;
 
        [Header("Debug")]
        public bool drawGizmos;

        public int targetCreatureCount = 100;

        [Range(0, 1)]
        public float preferredTimeDelta = 0;

        public virtual void Teleport(Vector3 newHome, Vector3 trans, Boid calculationBoid)
        {
            SchoolGenerator sg = GetComponent<SchoolGenerator>();
            foreach (Boid b in boids)
            {                
                if (this is SchoolGenerator)
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    Vector3 pos = newHome + unit * UnityEngine.Random.Range(0, radius * sg.spread);
                    WorldGenerator wg = WorldGenerator.Instance;
                    if (wg != null)
                    {
                        float groundHeight = wg.SamplePos(pos.x, pos.z);
                        if (pos.y < groundHeight)
                        {
                            pos.y = groundHeight + UnityEngine.Random.Range(10, radius * sg.spread);
                        }
                    }
                    b.position = pos;
                    b.desiredPosition = pos;
                    if (b.GetComponent<Constrain>() != null)
                    {
                        b.GetComponent<Constrain>().centre = pos;
                    }
                }
                else
                {
                    if (b != calculationBoid)
                    {
                        b.position += trans;
                        b.desiredPosition += trans;
                        if (b.GetComponent<Constrain>() != null)
                        {
                            b.GetComponent<Constrain>().centre += trans;
                        }
                    }
                }
                b.suspended = false;
                if (b.GetComponent<TrailRenderer>() != null)
                {
                    b.GetComponent<TrailRenderer>().Clear();
                }
            }
        }

        System.Collections.IEnumerator UpdateCenterOfMass()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 0.5f));
            while (true)
            {
                if (centerOfMassUpdatePerSecond == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(1.0f / centerOfMassUpdatePerSecond);
                }
                if (boids.Count == 0)
                {
                    continue;
                }
                Vector3 average = Vector3.zero;
                foreach (Boid boid in boids)
                {
                    average += boid.position;
                }
                average /= boids.Count;
                centerOfMass = average;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
		
        }

        void Start()
        {
            StartCoroutine(UpdateCenterOfMass());
        }
    }
}