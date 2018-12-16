using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{

    /// <summary>
    /// Line waveform.
    /// This object uses a lineRenderer to display an audio waveform. 
    /// </summary>
    public class LineWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("Draw the line between these two points")]
        public List<Transform> points; // draw between these points.
        [Tooltip("LineRenderer attributes")]
        public LineAttributes lineAtt; // lineRenderer attributes
        [Tooltip("Height of the waveform")]
        public float waveformAmplitude = 5; // height of the waveform.
        [Tooltip("How big are the in-editor gizmos?")]
        public float gizmosSize = 1;
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // if true, it will use the audioFile inside teh AudioSampler audioFiles array
        [Tooltip("Take the absolute value of samples?")]
        public bool abs = false; // use absolute value or not
        [Tooltip("Snap the waveform to the end points?")]
        public bool snapEndpoints = true; // snap the first and last point on the line to equal the start and end positions of the line

        //____________Delegates/Actions

        //____________Protected Variables

        protected List<LineRenderer> lines;

        //____________Private Variables

        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void Start () 
		{
            lines = new List<LineRenderer>();
            for (int i = 0; i < (points.Count-1); i++)
            {
               LineRenderer line = NewLine(lineAtt.startColor, lineAtt.endColor, lineAtt.startWidth, lineAtt.endWidth, lineAtt.lineSegments);
               lines.Add(line);
            }
		}
        // Update is called once per frame
        void Update()
        {
            DrawLines();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < (points.Count - 1); i++)
            {
                Vector3 start = points[i].position;
                Vector3 end = points[i + 1].position;
                Gizmos.DrawLine(start, end);
            }
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i].position, gizmosSize);
            }
        }


        /*________________Public Methods________________*/


        /// <summary>
        /// Create a new lineRenderer
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="startWidth"></param>
        /// <param name="endWidth"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        public LineRenderer NewLine(Color color1, Color color2, float startWidth, float endWidth, int segments)
		{
			GameObject go = new GameObject(); 
			go.transform.SetParent (this.transform);
			go.name = "Line";
			LineRenderer line = go.AddComponent<LineRenderer>();
			line = go.GetComponent<LineRenderer>();
			if(lineAtt.lineMat == null)
			{
				line.material = new Material(Shader.Find("Particles/Additive"));
			}
			else
			{
				line.material = lineAtt.lineMat;
			}
			line.SetColors(color1, color2);
			line.SetWidth(startWidth, endWidth);
			line.SetVertexCount(segments);
			
			return line;
		}

        /// <summary>
        /// Make each point rotate to look at the next point in the list
        /// </summary>
        public void OrientPoints()
        {
            for (int i = 0; i < (points.Count - 1); i++)
            {
                points[i].LookAt(points[i + 1].position);
            }
        }

        /// <summary>
        /// Rename the points in the points list.
        /// </summary>
        /// <param name="name"></param>
        public void RenamePoints(string name)
        {

            for (int i = 0; i < points.Count; i++)
            {
                points[i].gameObject.name = name + i.ToString();
            }
        }

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        /// <summary>
        /// Move points in the lineRendrer accourding to the decibal level of the audio
        /// </summary>
        void DrawLines()
        {
            for (int i = 0; i < (points.Count - 1); i++)
            {

                Vector3 start = points[i].position; // the start point of this line
                Vector3 end = points[i + 1].position; // the end point of this line
                Vector3 toTarget = end - start; // vector from me to my target
                float[] audioSamples;
                //start and end index for our line draw for loop
                int startIndex = 0;
                int endIndex = lineAtt.lineSegments;


                if (frequencyRange == FrequencyRange.Decibal)
                {
                    audioSamples = AudioSampler.instance.GetAudioSamples(audioIndex, lineAtt.lineSegments, abs, useAudioFile);
                }
                else
                {
                    audioSamples = AudioSampler.instance.GetFrequencyData(audioIndex, frequencyRange, lineAtt.lineSegments, abs, useAudioFile);
                }


                if (snapEndpoints)
                {
                    //snap the first and last point on this line
                    lines[i].SetPosition(0, start);
                    lines[i].SetPosition(lineAtt.lineSegments - 1, end);
                    //change our indicies for the forloop to draw the rest of the line
                    startIndex = 1;
                    endIndex = lineAtt.lineSegments - 1;
                }
                for (int j = startIndex; j < endIndex; j++) // for each line segment
                {
                    float percent = (float)j / (lineAtt.lineSegments - 1); // percent complete
                    int index = (int)(percent * (audioSamples.Length - 1));
                    Vector3 position = start + toTarget * percent; // position = myPos + toTarget*percent
                    float yOffset; //add this y offset to the position for this position in the line
                    if (abs)
                    {
                        yOffset = Mathf.Abs(audioSamples[index]) * waveformAmplitude * AudioSampler.instance.globalSensitivity;
                    }
                    else
                    {
                        yOffset = audioSamples[index] * waveformAmplitude * AudioSampler.instance.globalSensitivity;

                    }
                    position += points[i].up * yOffset; // add in the y offset
                    lines[i].SetPosition(j, position); // set the position for this line segment
                }
            }
        }
    }

    /// <summary>
    /// Hold attributes for the line renderer that gets created at runtime
    /// </summary>
	[System.Serializable]
	public class LineAttributes 
	{
		public Material lineMat;
		public Color startColor = Color.cyan;
		public Color endColor = Color.magenta;
		public float startWidth = .1f;
		public float endWidth = .1f;
		public int lineSegments = 36;
	}
}
