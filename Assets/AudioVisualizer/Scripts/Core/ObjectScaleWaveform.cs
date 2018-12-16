using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// Object scale waveform.
    /// Scale objects along a given axis, to create an audio waveform. Objects are typically arranged in a line or close together.
    /// </summary>
    public class ObjectScaleWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("How sensitive to the audio are we?")]
        public float sensitivity = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("What objects should we scale to create a waveform?\n Usually objects are placed in a line")]
        public List<GameObject> objects; // objects to scale with the music
        [Tooltip("Scale objects along this axis")]
        public Vector3 scaleAxis = Vector3.up; // the direction we scale each object.
        [Tooltip("Max scale along 'scaleAxis'")]
        public float maxHeight = 10; // the max scale along the scaleAxis. i.e. if scaleAxis is(0,1,0), this is our maxHeight
        [Tooltip("How fast do we move the objects?")]
        public float lerpSpeed = 1; // speed at which we scale, from currentScale to nextScale.
        [Tooltip("Take the absolute value of samples?")]
        public bool absoluteVal = true; // /use absolute value of audio decibal levels
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private List<Vector3> startingScales; // the scale of each object on game start
		private List<Vector3> startingPositions; // the position of each object on game start

        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void Start () {

			//initialize starting scales and positions
			startingScales = new List<Vector3> ();
			startingPositions = new List<Vector3> ();
			foreach(GameObject obj in objects)
			{
				startingScales.Add(obj.transform.localScale);
				startingPositions.Add(obj.transform.position);
			}


		}
		
		// Update is called once per frame
		void Update () {
			ScaleObjects();
		}

        /*________________Public Methods________________*/

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        void ScaleObjects()
		{


            float[] audioSamples;
            if (frequencyRange == FrequencyRange.Decibal)
            {
                audioSamples = AudioSampler.instance.GetAudioSamples(audioIndex, objects.Count, absoluteVal,useAudioFile);
            }
            else
            {
                audioSamples = AudioSampler.instance.GetFrequencyData(audioIndex, frequencyRange, objects.Count, absoluteVal,false);
            }
            

			//for each object
			for(int i = 0; i < objects.Count; i++)
			{
                float scale = audioSamples[i] * sensitivity * AudioSampler.instance.globalSensitivity;

                float sampleScale = Mathf.Min(scale, 1); // % of maxHiehgt, via teh audio sample
				float currentHeight = sampleScale*maxHeight; 

				Vector3 desiredScale = startingScales[i] + currentHeight*transform.InverseTransformDirection(scaleAxis); // get desired scale, in correct direction, using audio
				objects[i].transform.localScale = Vector3.Lerp(objects[i].transform.localScale,desiredScale ,Time.smoothDeltaTime*lerpSpeed); // lerp from current scale to desired scale

				//reposition the object after scaling, so that it's seemingly in the same place.
				float distanceScaled = (objects[i].transform.localScale - startingScales[i]).y; // get change in scale
				Vector3 direction = objects[i].transform.TransformDirection(scaleAxis); // get movement direction, relative to object
				objects[i].transform.position = startingPositions[i] + distanceScaled*direction*.5f; // move the object
			
			}
		}
	}
}
