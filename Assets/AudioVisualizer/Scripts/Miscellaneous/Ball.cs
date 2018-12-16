using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    //used in the BeatAnticipation scene. Launched at a cube
    public class Ball : MonoBehaviour
    {

        public AnimationCurve flightPath;
        public float amplitude = 1;

        //launch the ball at a target, it will get there in "travelTime" seconds
        public void LaunchBall(Transform target, float travelTime)
        {
            StopAllCoroutines();
            StartCoroutine(Launch(target, travelTime));
        }

        //the launch coroutine
        IEnumerator Launch(Transform target, float travelTime)
        {
            
            float timer = 0;
            Vector3 startPos = this.transform.position;
            // the vector from this ball to the target
            Vector3 toTarget = target.transform.position - this.transform.position;
            
            while (timer < travelTime)
            {
                //get the % we should have traveled
                float percentTraveled = timer / travelTime;

                //thisPos = startPos + toTarget*percentTraveled + heightOffset
                this.transform.position = startPos + toTarget*percentTraveled + Vector3.up * flightPath.Evaluate(percentTraveled) * amplitude;

                //increment the timer
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
