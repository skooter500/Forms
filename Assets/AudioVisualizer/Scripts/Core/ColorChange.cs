using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// This class changes an object's material color based on the audio.
    /// </summary>
    public class ColorChange : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("How reactive are we to the music?")]
        public float sensitivity = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("When music is quiet, material is this color")]
        public Color lowColor = Color.white; // when music decibal level is low, material is this color.
        [Tooltip("When music is loud, material is this color")]
        public Color highColor = Color.white; // when music decibal level is high, material is this color.
        [Tooltip("How fast can we lerp between colors?")]
        public float lerpSpeed = 10; // lerp between current color, and desired color
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private Gradient gradient; // color gradient from lowColor to highColor
		private List<Material> materials; // this material
        private Light light;
		private float alpha = 0; // material opacity
        private Color audioColor;

        
        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void Awake () {
			gradient = PanelWaveform.GetColorGradient(lowColor,highColor);
            materials = new List<Material>();

            MeshRenderer mr = this.GetComponent<MeshRenderer>();
            if(mr != null)
            {
                materials.AddRange(mr.materials);
                alpha = materials[0].color.a;
            }

            light = this.GetComponent<Light>();
            audioColor = lowColor;

        }
		
		// Update is called once per frame
		void Update () 
		{
			LerpColors ();
		}
        
        /*________________Public Methods________________*/

        public void Reset()
		{
			gradient = PanelWaveform.GetColorGradient(lowColor,highColor);
		}

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        //color the panel, using the audio average decibal level to fade between colors.
        void LerpColors()
		{
			float value;
			if(frequencyRange == FrequencyRange.Decibal)
			{
				value = AudioSampler.instance.GetRMS (audioIndex,useAudioFile)*sensitivity;//get the noise level 0-1 of the audio right now
			}
			else
			{
				value = AudioSampler.instance.GetFrequencyVol(audioIndex,frequencyRange,useAudioFile)*sensitivity;
			}

            //Debug.Log("value: " + value);
			value = Mathf.Clamp (value, 0, 1);
            

			Color desiredColor = gradient.Evaluate (value); //evaluate the gradient, grab a color based on the rms value 0-1
            audioColor = Color.Lerp (audioColor, desiredColor, lerpSpeed * Time.deltaTime); // lerp from current color to desired color
			float desiredAlpha = lowColor.a + (highColor.a-lowColor.a)*value; //desired alpha based on rms
			alpha = Mathf.Lerp(alpha,desiredAlpha, lerpSpeed * Time.deltaTime); // lerp alpha
            audioColor.a = alpha; // assign alpha to our gradient color


            foreach (Material mat in materials)
            {
                mat.color = audioColor; // assign the color to our material
            }

            if(light != null)
            {
                light.color = audioColor;
            }
			
		}
	}
}
