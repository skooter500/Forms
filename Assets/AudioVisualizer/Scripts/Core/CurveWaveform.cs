using UnityEngine;
using System.Collections;


namespace AudioVisualizer
{
    /// <summary>
    /// Curve waveform, just like LineWaveform, but also uses a user set curve to affect amplitude of the line.
    /// </summary>
	public class CurveWaveform : LineWaveform
    {
        //____________Public Variables

        [Tooltip("Adjust the shape of the curve\nAmplitude over distance")]
        public AnimationCurve shapeCurve; // control amplitude over distance, multiplied by the audio waveform.
        [Tooltip("Curve is multiplied by this amplitude")]
        public float curveAmplitude = 5;
        [Tooltip("What direction does the audio waveform go?")]
        public Vector3 curveDirection = Vector3.up; // what direction does the audio g

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        /*________________Monobehaviour Methods________________*/

        void Update () 
		{
			DrawLines ();
		}

        /*________________Public Methods________________*/

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        //move points in teh lineRendrer accourding to the decibal level of the audio, * the amplitude Curve
        void DrawLines()
        {
            for (int i = 0; i < (points.Count - 1); i++)
            {
                Vector3 start = points[i].position;
                Vector3 end = points[i + 1].position;
                Vector3 toTarget = end - start; // vector from me to my target
                int startIndex = 0;
                int endIndex = lineAtt.lineSegments;

                float[] audioSamples;
                if (frequencyRange == FrequencyRange.Decibal)
                {
                    audioSamples = AudioSampler.instance.GetAudioSamples(audioIndex, lineAtt.lineSegments, abs,useAudioFile);
                }
                else
                {
                    audioSamples = AudioSampler.instance.GetFrequencyData(audioIndex, frequencyRange, lineAtt.lineSegments, abs,false);
                }
                

                if (snapEndpoints)
                {
                    //set the start and end of the line to draw perfectly at teh start and end
                    lines[i].SetPosition(0, start);
                    lines[i].SetPosition(lineAtt.lineSegments - 1, end);
                    //change our indicies for the forloop to draw the rest of the line
                    startIndex = 1;
                    endIndex = lineAtt.lineSegments - 1;
                }
                //draw the rest of the line, using the waveform and curve
                for (int j = startIndex; j < endIndex; j++) // for each line segment
                {
                    float percent = (float)j / (lineAtt.lineSegments - 1); // percent complete
                    Vector3 position = start + toTarget * percent; // position = myPos + toTarget*percent
                    float yOffset; //add this y offset to the position for this position in the line
                    float sampleHeight = audioSamples[j]* AudioSampler.instance.globalSensitivity;
                    if (abs)
                    {
                        sampleHeight = Mathf.Abs(sampleHeight);
                    }

                    if (shapeCurve != null)
                    {
                        yOffset = shapeCurve.Evaluate(percent) * curveAmplitude + sampleHeight * waveformAmplitude;// use amplitude curve
                    }
                    else
                    {
                        Debug.LogWarning("Warning: No Shape Curve set for CurveWaveform.cs on " + this.gameObject.name);
                        yOffset = sampleHeight * waveformAmplitude;
                    }


                    position += points[i].transform.TransformDirection(curveDirection) *yOffset; // add in the y offset
                    lines[i].SetPosition(j, position); // set the position for this line segment
                }
            }
        }
	}
}
