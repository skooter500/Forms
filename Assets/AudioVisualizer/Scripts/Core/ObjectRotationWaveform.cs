using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioVisualizer
{
    /// <summary>
    /// Rotates Gameobjects around a given axis.
    /// </summary>
    public class ObjectRotationWaveform : MonoBehaviour
    {
        //________ Public Variables ________

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("How reactive are we to the music?")]
        public float sensitivity = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("What objects should we rotate?")]
        public List<GameObject> objects; // list of objects to be moved up and down
        [Tooltip("The axis of rotation")]
        public Vector3 rotationAxis = Vector3.up;
        [Tooltip("Minimum rotation")]
        public float minAngle = 0;
        [Tooltip("Maximum rotation")]
        public float maxAngle = 360;
        [Tooltip("Smooth the rotation at this rate")]
        public float lerpSpeed = 10f;
        [Tooltip("If true, objects rotate around their local positions \nIf false, they use this object as a pivot point")]
        public bool localRotation = true;
        [Tooltip("If true, each object will have a unique random rotation axis")]
        public bool randomAxis = false;
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file


        //________ Private Variables ________

        List<Vector3> randomAxes;
        float currentAngle = 0; // the current rotation angle for each object

        //________ Monobehaviour Methods ________

        private void Start()
        {
            randomAxes = new List<Vector3>();
            for(int i = 0; i < objects.Count; i++)
            {
                randomAxes.Add(Random.insideUnitSphere);
            }
        }

        void Update()
        {
            RotateObjects();
        }

        //________ Public Methods ________

        //________ Private Methods________

        /// <summary>
        /// Use audio to rotate objects.
        /// </summary>
        void RotateObjects()
        {
            float value;
            if (frequencyRange == FrequencyRange.Decibal)
            {
                value = AudioSampler.instance.GetRMS(audioIndex, useAudioFile) * sensitivity;//get the noise level 0-1 of the audio right now
            }
            else
            {
                value = AudioSampler.instance.GetFrequencyVol(audioIndex, frequencyRange, useAudioFile) * sensitivity;
            }

            value = Mathf.Clamp(value, 0, 1);

            float range = maxAngle - minAngle;
            float desiredAngle = minAngle + range * value;

            //lerp our current angle
            float newAngle = Mathf.Lerp(currentAngle, desiredAngle, Time.smoothDeltaTime * lerpSpeed);
            //see how much we lerped by
            float delta = newAngle - currentAngle;
            currentAngle = newAngle;
            

            for (int i = 0; i < objects.Count; i++)
            {
                GameObject go = objects[i];
                Vector3 axis = randomAxis ? randomAxes[i] : rotationAxis;
                if (localRotation)
                {
                    go.transform.RotateAroundLocal(axis, delta);
                }
                else
                {
                    go.transform.RotateAround(this.transform.position, axis, delta);
                }
            }

        }
    }
}
