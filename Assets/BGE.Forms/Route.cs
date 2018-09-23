using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BGE.Forms
{
    public class Route:MonoBehaviour
    {
        void Start()
        {
            Waypoints.Clear();
            int children = transform.childCount;
            for (int i = 0; i < children; i ++)
            {
                Transform child = transform.GetChild(i);
                Waypoints.Add(child.position);
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            int children = transform.childCount;
            //Gizmos.DrawSphere(transform.position, 2.5f);

            for (int i = 0; i < children; i++)
            {
                Transform child = transform.GetChild(i);
                if (i > 0)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i - 1).position);
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(child.position, 1.5f);
                if (looped && (i == transform.childCount - 1) && transform.childCount > 1)
                {
                    Gizmos.DrawLine(transform.GetChild(0).position, transform.GetChild(i).position);
                }
            }
        }


        private List<Vector3> waypoints = new List<Vector3>();
        private int next = 0;
        public bool draw;

        public int Next
        {
            get { return next; }
            set { next = value; }
        }

        public List<Vector3> Waypoints
        {
            get { return waypoints; }
            set { waypoints = value; }
        }

        public bool looped;
                
        public Vector3 NextWaypoint()
        {
            return waypoints[next];
        }

        public bool IsLast()
        {
            return (next == waypoints.Count() - 1);
        }

        public void AdvanceToNext()
        {
            if (looped)
            {
                next = (next + 1) % waypoints.Count();
            }
            else
            {
                if (next != waypoints.Count() - 1)
                {
                    next = next + 1;
                }
            }
        }
    }
}