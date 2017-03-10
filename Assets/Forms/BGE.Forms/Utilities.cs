using UnityEngine;
using System;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class Utilities
    {
        public const float TWO_PI = Mathf.PI * 2.0f;
        
        private static System.Random Random = new System.Random(Guid.NewGuid().GetHashCode());

        public static float RandomRange(System.Random r, float min, float max)
        {
            double f = 0.0f;
            lock (r)
            {
                f = r.NextDouble();
            }
            return Map((float)f, 0.0f, 1.0f, min, max);
        }

        // Cant use the Unity one on a thread
        public static float RandomRange(float min, float max)
        {
            return RandomRange(Random, min, max);
        }


        public static Vector3 RandomInsideUnitSphere()
        {
            Vector3 p = new Vector3(RandomRange(-1, 1), RandomRange(-1, 1), RandomRange(-1, 1));
            p.Normalize();
            return p;
        }

        public static float RadToDegrees(float rads)
        {
            return rads * Mathf.Rad2Deg;
        }

        public static float DegreesToRads(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        public static Boid FindBoidInHierarchy(GameObject root, int startDepth = 0)
        {
            Boid boid = root.GetComponentInChildren<Boid>();
            if (boid == null)
            {
                return FindBoidInHierarchy(root.transform.parent.gameObject, startDepth ++);
            }
            else
            {
                return boid;
            }
        }

        public static void RecursiveSetColor(GameObject boid, Color color)
        {
            if (boid != null)
            {
                Renderer renderer = boid.GetComponent<Renderer>();
                if (renderer != null && ! renderer.materials[0].name.Contains("Trans"))
                {
                    renderer.material.color = color;
                }

                for (int j = 0; j < boid.transform.childCount; j++)
                {
                    RecursiveSetColor(boid.transform.GetChild(j).gameObject, color);
                }
            }            
        }

        public static float Map(float value, float r1, float r2, float m1, float m2)        
        {
            float dist = value - r1;
            float range1 = r2 - r1;
            float range2 = m2 - m1;
            return m1 + ((dist / range1) * range2);
        }
        public static Vector3 RandomPosition(float range)
        {
            Vector3 pos = new Vector3();
            pos.x = UnityEngine.Random.Range(-range, range);
            pos.y = UnityEngine.Random.Range(-range, range);
            pos.z = UnityEngine.Random.Range(-range, range);
            return pos;
        }

        public static float Interpolate(float alpha, float x0, float x1)
        {
            return x0 + ((x1 - x0) * alpha);
        }

        public static Vector3 Interpolate(float alpha, Vector3 x0, Vector3 x1)
        {
            return x0 + ((x1 - x0) * alpha);
        }


        /// <summary>
        /// Constrain a given value (x) to be between two (ordered) bounds min
        /// and max.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Returns x if it is between the bounds, otherwise returns the nearer bound.</returns>
        public static float Clip(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        internal static void SetUpAnimators(GameObject part, Boid boid)
        {
            // Tail animator setup
            Animator[] animators = part.GetComponentsInChildren<Animator>();
            foreach (Animator a in animators)
            {
                a.boid = boid;
            }
        }

        // ----------------------------------------------------------------------------
        // remap a value specified relative to a pair of bounding values
        // to the corresponding value relative to another pair of bounds.
        // Inspired by (dyna:remap-interval y y0 y1 z0 z1)
        public static float RemapInterval(float x, float in0, float in1, float out0, float out1)
        {
            // uninterpolate: what is x relative to the interval in0:in1?
            float relative = (x - in0) / (in1 - in0);

            // now interpolate between output interval based on relative x
            return Interpolate(relative, out0, out1);
        }

        // Like remapInterval but the result is clipped to remain between
        // out0 and out1
        public static float RemapIntervalClip(float x, float in0, float in1, float out0, float out1)
        {
            // uninterpolate: what is x relative to the interval in0:in1?
            float relative = (x - in0) / (in1 - in0);

            // now interpolate between output interval based on relative x
            return Interpolate(Clip(relative, 0, 1), out0, out1);
        }

        // ----------------------------------------------------------------------------
        // classify a value relative to the interval between two bounds:
        //     returns -1 when below the lower bound
        //     returns  0 when between the bounds (inside the interval)
        //     returns +1 when above the upper bound
        public static int IntervalComparison(float x, float lowerBound, float upperBound)
        {
            if (x < lowerBound) return -1;
            if (x > upperBound) return +1;
            return 0;
        }

        //public static float ScalarRandomWalk(float initial, float walkspeed, float min, float max)
        //{
        //    float next = initial + (((Random() * 2) - 1) * walkspeed);
        //    if (next < min) return min;
        //    if (next > max) return max;
        //    return next;
        //}

        public static float Square(float x)
        {
            return x * x;
        }

        /// <summary>
        /// blends new values into an accumulator to produce a smoothed time series
        /// </summary>
        /// <remarks>
        /// Modifies its third argument, a reference to the float accumulator holding
        /// the "smoothed time series."
        /// 
        /// The first argument (smoothRate) is typically made proportional to "dt" the
        /// simulation time step.  If smoothRate is 0 the accumulator will not change,
        /// if smoothRate is 1 the accumulator will be set to the new value with no
        /// smoothing.  Useful values are "near zero".
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="smoothRate"></param>
        /// <param name="newValue"></param>
        /// <param name="smoothedAccumulator"></param>
        /// <example>blendIntoAccumulator (dt * 0.4f, currentFPS, smoothedFPS)</example>
        public static void BlendIntoAccumulator(float smoothRate, float newValue, ref float smoothedAccumulator)
        {
            smoothedAccumulator = Interpolate(Clip(smoothRate, 0, 1), smoothedAccumulator, newValue);
        }

        public static void BlendIntoAccumulator(float smoothRate, Vector3 newValue, ref Vector3 smoothedAccumulator)
        {
            smoothedAccumulator = Interpolate(Clip(smoothRate, 0, 1), smoothedAccumulator, newValue);
        }

        public static Vector3 TransformPointNoScale(Vector3 localPoint, Transform transform)
        {
            Vector3 p = transform.rotation * localPoint;
            p += transform.position;
            return p;
        }


        static public bool checkNaN(ref Vector3 v, Vector3 def)
        {
            if (float.IsNaN(v.x))
            {
                Debug.LogError("Nan");
                v = def;
                return true;
            }
            if (float.IsNaN(v.y))
            {
                Debug.LogError("Nan");
                v = def;
                return true;
            }
            if (float.IsNaN(v.z))
            {
                Debug.LogError("Nan");
                v = def;
                return true;
            }
            return false;
        }

        static public bool checkNaN(Vector3 v)
        {
            if (float.IsNaN(v.x))
            {
                System.Console.WriteLine("Nan");
                return true;
            }
            if (float.IsNaN(v.y))
            {
                System.Console.WriteLine("Nan");
                return true;
            }
            if (float.IsNaN(v.z))
            {
                System.Console.WriteLine("Nan");
                return true;
            }
            return false;
        }

        public static Color ColorFromRGB(String rgb)
        {
            if (rgb[0] == '#')
            {
                rgb = rgb.Substring(1);
            }
            Color color = new Color();

            int i = int.Parse(rgb.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);

            color.r = (float) Convert.ToInt32(rgb.Substring(0, 2), 16) / 255.0f;
            color.g = (float) Convert.ToInt32(rgb.Substring(2, 4), 16) / 255.0f;
            color.b = (float) Convert.ToInt32(rgb.Substring(4), 16) / 255.0f;
            color.a = 1;
            return color;
        }

        public static List<Renderer> GetRenderersinChildrenRecursive(GameObject root)
        {
            List<Renderer> ret = new List<Renderer>();
            return GetRenderersinChildrenRecursive(root, ret);
        }

        private static List<Renderer> GetRenderersinChildrenRecursive(GameObject root, List<Renderer> list)
        {
            Renderer[] rs = root.GetComponents<Renderer>();
            foreach (Renderer r in rs)
            {
                list.Add(r);
            }
            for (int i = 0; i < root.transform.childCount; i++)
            {
                GetRenderersinChildrenRecursive(root.transform.GetChild(i).gameObject, list);
            }
            return list;
        }
     

        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}