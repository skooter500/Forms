using UnityEngine;
using System.Collections;
using System;

namespace BGE.Forms
{
    public class RouteFollower : SteeringBehaviour {
        public Route route;
        public bool ignoreHeight = false;
        public float waypointDistance = 20.0f;
        // Update is called once per frame
        public void Start()
        {
            if (route == null)
            {
                route = GetComponent<Route>();
            }
        }

        public override Vector3 Calculate()
        {
            float dist;
            Vector3 nextWayPoint = route.NextWaypoint();

            if (ignoreHeight)
            {
                nextWayPoint.y = boid.position.y;
            }

            dist = (boid.position - route.NextWaypoint()).magnitude;

            if (dist < waypointDistance)
            {
                route.AdvanceToNext();
            }
            if ((!route.looped) && route.IsLast())
            {
                return boid.ArriveForce(route.NextWaypoint());
            }
            else
            {
                return boid.SeekForce(route.NextWaypoint());
            }
        }    
    }
}