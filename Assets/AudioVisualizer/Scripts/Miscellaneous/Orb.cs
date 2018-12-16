using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    /// <summary>
    /// Used in the "BeatBasedGame" scene, a colored orb that falls down the screen.
    /// </summary>
    public class Orb : MonoBehaviour {

        public float speed = 0;

        float disableHeight = -2f;
        TrailRenderer trail;

        
        private void OnEnable()
        {
            //clear out the old trailRenderer
            if(trail != null)
            {
                trail.Clear();
            }

            AudioSampler.AudioUpdate += AudioUpdate;
        }
        private void OnDisable()
        {
            //Debug.Log("Time: " + timer);
            AudioSampler.AudioUpdate -= AudioUpdate;
        }

        private void Start()
        {
            trail = this.GetComponent<TrailRenderer>();
        }

        // Update is called once per frame
        void AudioUpdate(float audioTime, float deltaTime)
        {
            //move downward in sync with the audio
            this.transform.position -= Vector3.up * speed * deltaTime;

            if(this.transform.position.y < disableHeight)
            {
                this.gameObject.SetActive(false);
            }
        }


    
    }
}
