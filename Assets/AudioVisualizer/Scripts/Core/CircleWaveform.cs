using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// This class places game objects in a circle, rotates them, and adjusts the radius using audiodata.
    /// The gameobjects moved typically have trail renderers attached to them in our examples.
    /// </summary>
    public class CircleWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("Adjusts height of the waveform")]
        public float amplitude = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("Objects to move around in a circle.\n usually these are TrailRenderers")]
        public List<GameObject> objects; // the objects we want to move in a circle
        [Tooltip("How fast does the circle rotate?")]
        public float rotationSpeed = 10; // rotate objects around at this speed.
        [Tooltip("Radius of the circle")]
        public float radius = 10; // radius of the circle
        [Tooltip("Rotation lerp speed")]
        public float lerpSpeed = 10; // lerp between current rotation and next rotation.
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file
        [Tooltip("Adjust circle radius using audio?")]
        public bool useWaveform = true; //if true: lerp up and down, and around. If false, just around

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private List<float> startAngles; // starting angle of each object
		private float angle = 0; // current angle
		float rotSpeed; //temp container for rotation speed, used in the "Bump" method
		private bool bumped = false; // state container
		private float startRadius; // initial radius
		private float sign = 1;


        /*________________Monobehaviour Methods________________*/

        void Awake()
		{
			//get the starting angle for each object
			startAngles = new List<float> ();
			float anglePerObject = 360f / objects.Count;
			for(int i = 0; i < objects.Count; i++)
			{
				startAngles.Add(i*anglePerObject);
			}

			rotSpeed = rotationSpeed; // place rotation speed in a temp container
			startRadius = radius;

		}

		void Start()
		{
			PositionObjects();
		}
		
		// Update is called once per frame
		void Update () {
			PositionObjects();

		}

		//draw a circle around this object in scene view
		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawSphere (this.transform.position, 1);
		}

        /*________________Public Methods________________*/

        /// <summary>
        /// Make everything rotate faster for a short period of time.
        /// Call from AudioEvent.OnBeat()
        /// </summary>
        /// <param name="multiplier"></param>
        public void Boost(float multiplier)
		{
			Invoke ("ResetSpeed",.1f); //reset the speed in .1 seconds
			rotationSpeed = rotationSpeed*multiplier; // set the new rotation speed		
		}

        /// <summary>
        /// Set the radius to the current sample average 0-1.
        /// Kind of like 'useWaveform' but doesn't happen every update, only when it's called
        /// If switchSign = true, the sign of the audio is switched from 1 to -1 or vise versa every call.
        /// </summary>
        /// <param name="switchSign"></param>
        public void Bump(bool switchSign)
		{
			//float avg = AudioSampler.instance.GetAvg(audioSource,sensitivity,absoluteVal);
			float value;
			if(frequencyRange == FrequencyRange.Decibal)
			{
				value = AudioSampler.instance.GetRMS(audioIndex,useAudioFile);//get the noise level 0-1 of the audio right now
			}
			else
			{
				value = AudioSampler.instance.GetFrequencyVol(audioIndex,frequencyRange,useAudioFile);
			}

			if(switchSign)
			{
				sign = -sign;
			}
			radius = startRadius + value * sign * amplitude * AudioSampler.instance.globalSensitivity;
		}

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        /// <summary>
        /// Position each object around this gameobject, based on rotation speed.
        /// </summary>
        void PositionObjects()
		{
			//position the object accourding to the average
			angle += Time.smoothDeltaTime * rotationSpeed; // current rotation over time
			for(int i = 0; i < objects.Count; i++)
			{
				//angle of this object, = angle + startAngle
				float rad = (angle+startAngles[i])*Mathf.Deg2Rad; // radians
				float newRad = radius; // new radius
				if(useWaveform) // if using radius, update the radius based on the current average decibal level
				{
					float value;
					if(frequencyRange == FrequencyRange.Decibal)
					{
						value = AudioSampler.instance.GetRMS(audioIndex,useAudioFile);//get the noise level 0-1 of the audio right now
					}
					else
					{
						value = AudioSampler.instance.GetFrequencyVol(audioIndex,frequencyRange,useAudioFile);
					}
                    
                    newRad = radius + value * amplitude*AudioSampler.instance.globalSensitivity;
                    //Debug.Log("Value: " + value + " Radius: " + newRad);
                }
				float x = Mathf.Cos(rad)*newRad; //update x and y pos
				float y = Mathf.Sin(rad)*newRad;
				//position each object around this one.
				Vector3 newPos = this.transform.position + this.transform.right*x + this.transform.up*y; 
				objects[i].transform.position = Vector3.Lerp(objects[i].transform.position,newPos,lerpSpeed*Time.smoothDeltaTime);
			}
		}

        /// <summary>
        /// Reset rotation speed.
        /// </summary>
        void ResetSpeed()
        {
            rotationSpeed = rotSpeed;
        }
    }
}
