using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// Object position waveform.
    /// Positions game objects up and down along a given axis, to create an audio waveform.
    /// For example if positionAxis is Vector3.up, the script will move all the objects up and down using the audio.
    /// Objects are typically arranged in a line or close together.
    /// </summary>
    public class ObjectPositionWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("How sensitive to the audio are we?")]
        public float sensitivity = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("What objects should we move to create a waveform?\n Usually objects are placed in a line")]
        public List<GameObject> objects; // list of objects to be moved up and down
        [Tooltip("Move objects along this axis")]
        public Vector3 positionAxis = Vector3.up;// move the objects along this axis
        [Tooltip("Max distance along 'postitionAxis' we'll move an object")]
        public float maxHeight = 10; // the max distance along the positionAxis. i.e. if positionAxis is(0,1,0), this is our maxHeight
        [Tooltip("How fast do we move the objects?")]
        public float lerpSpeed = 1; // speed at which we move each object from it's current position, to it's next position.
        [Tooltip("Take the absolute value of samples?")]
        public bool absoluteVal = true; // take the abs value
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private List<Vector3> startingPositions; // the position of each object on game start
        private float maxValue = 0;
        

        /*________________Monobehaviour Methods________________*/


        // Use this for initialization
        void Start () {

			//initialize startingPositions list
			startingPositions = new List<Vector3> ();
			foreach(GameObject obj in objects)
			{
				Vector3 relativePos = obj.transform.position - this.transform.position;
				startingPositions.Add(relativePos);
			}

		}
		
		// Update is called once per frame
		void Update ()
        {
            PositionObjects(); // move objects along positionAxis, accourding to the music
#if UNITY_EDITOR
            for (int i = 0; i < objects.Count; i++)
			{
				Debug.DrawLine(startingPositions[i], startingPositions[i] + objects[i].transform.up*maxHeight);
			}
#endif
        }

        /*________________Public Methods________________*/

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        // move objects along positionAxis, accourding to the music
        void PositionObjects()
		{
			float[] audioSamples;
			if (frequencyRange == FrequencyRange.Decibal)
            {
				audioSamples = AudioSampler.instance.GetAudioSamples (audioIndex, objects.Count, absoluteVal,useAudioFile);
			}
			else
			{
				audioSamples = AudioSampler.instance.GetFrequencyData (audioIndex, frequencyRange, objects.Count, absoluteVal, useAudioFile);
			}

			for(int i = 0; i < objects.Count; i++)//for each object
			{
                //position the object accourding to the average
                float height = audioSamples[i] * sensitivity * AudioSampler.instance.globalSensitivity;

                float samplePosition = Mathf.Min(height, 1); // % of maxHiehgt, via the audio sample
				float currentHeight = samplePosition*maxHeight; 

				Vector3 direction = objects[i].transform.TransformDirection(positionAxis); // get positionAxis relative to each object
				Vector3 desiredPos = this.transform.position + startingPositions[i] + currentHeight*direction;
                
				//reposition the objects, lerp between current position and desired position.
				objects[i].transform.position = Vector3.Lerp(objects[i].transform.position,desiredPos ,Time.smoothDeltaTime*lerpSpeed);
			}
			
		}

       

    }
}

