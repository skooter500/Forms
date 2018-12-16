using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace AudioVisualizer
{

    /// <summary>
    /// Pad waveform.
    /// Create's a speaker pad waveform. Call the SendRipple() method to send an audio ripple across the pad.
    /// </summary>
    public class PadWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("Number of lines in the pad")]
        public int numLines = 20; // number of lines in the pad
        [Tooltip("Radius of the pad")]
        public float radius = 10f; // radius of the pad
        [Tooltip("Height of ripple effects")]
        public float maxHeight = 3f; // height or amplitude of effects
        [Tooltip("Redraw every x frames")]
        public int updateRate = 1; //re-draw every x updates
        [Tooltip("Different pad types have diffferent effects")]
        public PadType padType = PadType.Wave;
        [Tooltip("Color of ripples sent through the pad")]
        public Color rippleColor = Color.white; // color of ripples
        [Tooltip("For PadType.Ripple, # of lines in the ripple")]
        public int rippleWidth = 3; // only applies if padType = PadType.Ripple. Number of lines in each ripple
        [Tooltip("LineRenderer attributes")]
        public LineAttributes lineAttributes; // attributes for each line, in this case, start color is the middle of the pad, and end color is the outside of the pad.
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // if true, it will use the audioFile inside teh AudioSampler audioFiles array
        [Tooltip("Show gizmos in editor?")]
        public bool gizmos = true;
		public enum PadType{Ripple,DampWave,Wave,Bounce};


        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private List<LineRenderer> lines = new List<LineRenderer>(); // list of lineRenderers that make up the pad
		private Gradient padGradient; // color gradient, startcolor to endcolor
		private int updateCounter = 0; // count updates
		float fakeTimer = 0;

        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void Start () 
		{
			padGradient = PanelWaveform.GetColorGradient (lineAttributes.startColor, lineAttributes.endColor);

			for(int i = 0; i < numLines; i++)
			{
				float val = (float)i/(numLines-1);
				lines.Add(NewLine(padGradient.Evaluate(val)));
			}
			CreatePad ();

			//just in case someone changes the private var. rippleLines can't be > numLines
			if( rippleWidth > numLines)
			{
				rippleWidth = numLines;
			}
		}

		// Update is called once per frame
		void FixedUpdate () 
		{

			if(updateCounter >= updateRate) //only re-draw every 'updateRate' updates
			{
				switch(padType)
				{
				case PadType.Ripple:
					Ripple();
					break;
				case PadType.DampWave:
					DampWave();
					break;
				case PadType.Wave:
					Wave();
					break;
				case PadType.Bounce:
					Bounce();
					break;
				default:
					break;
				}
				
				updateCounter = 0;
			}
			
			updateCounter ++;
		}

        void OnDrawGizmos()
        {
            if (gizmos)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(this.transform.position, 1);
            }
        }


        /*________________Public Methods________________*/


        /// <summary>
        /// send a ripple through the pad
        /// propegationTime is how long it takes for the ripple to propegate through the entire pad	
        /// </summary>
        /// <param name="propegationTime"></param>
        public void SendRipple(float propegationTime)
		{
			float[] samples = AudioSampler.instance.GetAudioSamples (audioIndex,lineAttributes.lineSegments, true,useAudioFile); // get teh sames at the time of the ripple
            StartCoroutine(RipIt(propegationTime, samples));
            //Debug.Log("Ripple");
		}

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        //send a wave through the pad
        /*
		 * The basic algorithm for this is as follows:
		 * while(timer < propegationTime)
		 * {
		 * 		index through the rings. index = (timer/propegationTime)*(numRings)
		 * 		for( each ring)
		 * 		{
		 * 			-get the distance to the index
		 * 			-if we're "rippleLines" or less away from the index, we're a part of the ripple
		 * 			-if we're part of the ripple, draw the rippleSamples on this ring, damped by distance from the center.
		 * 		}
		 * }
		 * */
        IEnumerator RipIt(float propegationTime, float[] rippleSamples)
        {
            //Vector3 firstPos = Vector3.zero; // the first position we use
			float timer = 0;
			float radiusStep = radius / (numLines-1); // distance between each ring. i.e. ring0 has radius 0*radiusStep, ring10 had radius 10*radiusStep, etc.
            float angle = 0; 
			float angleStep = 360f / lineAttributes.lineSegments;//increase the angle by this much, for every point on every line, to draw each circle.
            float percent = 0;
            int maxIndex = numLines - 1;
            int halfWidth = rippleWidth / 2; // (int) on purpose
            float heightStep = (1f / (halfWidth + 1)); //ripple height step size

            // another gradient, between start color and rippleColor
            Gradient lineGradient = PanelWaveform.GetColorGradient(lineAttributes.startColor, rippleColor); 
            Color[] rippleColors = new Color[maxIndex];
            float step = 1f / (rippleWidth - 1);
            for (int i = 0; i < rippleWidth; i++)
            {
                percent = i * step;
                rippleColors[i] = lineGradient.Evaluate(percent);
            }

            Color[] lineColors = new Color[numLines];//color of each line using the gradient
            float[] heightDamp = new float[numLines];//height damp array, how much we'll damp the ripple as it travels across the pad.
            float scaledHeight = maxHeight * AudioSampler.instance.globalSensitivity;
            float dampStep = scaledHeight / (numLines-1);
            step = 1f / (numLines - 1);
            for (int i = 0; i < numLines; i++)
            {
                percent = i * step;
                lineColors[i] = padGradient.Evaluate(percent);
                heightDamp[i] = scaledHeight - i * dampStep;
            }
           

			//Debug.Log ("Ripple from " + rippleLines + " to " + numLines);
            while (timer <= propegationTime)
            {
                //what line are we on, in the range rippleLines to numLines, based on the timer
                percent =  (timer / propegationTime);
                int lineIndex =(int)( percent * maxIndex);

                //start/end index
                int rippleStart = lineIndex - rippleWidth-1; // 1 outside the ripple
                rippleStart = Mathf.Max(0,rippleStart);
                int rippleEnd = lineIndex + rippleWidth;
                rippleEnd = Mathf.Min(rippleEnd, numLines);
                Vector3 firstPos = Vector3.zero;

                Vector3 thisPos = this.transform.position;
                Vector3 right = this.transform.right;
                Vector3 forward = this.transform.forward;
                Vector3 up = this.transform.up;

                for (int i = rippleStart; i < rippleEnd; i++)// for each line
                {
                    int dist = Mathf.Abs(lineIndex - i); // our distance from the lineIndex
                    int invDist = rippleWidth - dist;
                    float heightMultiplier = (dist > halfWidth) ? 0 : (1f - heightStep * dist);
                    float thisRadius = radiusStep * i; // the radius of this ring
                    //color the ring
                    if(i == (lineIndex - rippleWidth -1))
                    {
                        lines[i].SetColors(lineColors[lineIndex], lineColors[lineIndex]);
                    }
                    else{
                     lines[i].SetColors(rippleColors[invDist], rippleColors[invDist]);
                    }
                    for (int j = 0; j < lineAttributes.lineSegments - 1; j++) // for each line segment
                    {
                        float rad = Mathf.Deg2Rad * angle; // get angle in radians
                        //get x,y,z of this lineSegment using equation for a circle
                        float x = Mathf.Cos(rad) * thisRadius;
                        float y = rippleSamples[j] * heightDamp[lineIndex] * heightMultiplier; // y value based on audio info (rippleSamples) * heightMultiplier
                        float z = Mathf.Sin(rad) * thisRadius;
                        Vector3 pos = thisPos + right * x + up* y + forward * z;
                        lines[i].SetPosition(j, pos);
                        angle += angleStep; // increase angle by angleStep
                        if (j == 0)
                        {
                            firstPos = pos; // track the first lineSegment position
                        }
					}
				
				    lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
                }

                timer += Time.fixedDeltaTime;

                yield return null;
            }


        }

        /// <summary>
        /// Display the waveform in the first 'rippleLines' rings of the audioPad
        /// </summary>
        void Ripple()
		{

			float angleStep = 360f / lineAttributes.lineSegments;
			float[] samples = AudioSampler.instance.GetAudioSamples(audioIndex,lineAttributes.lineSegments, true,useAudioFile); //take 1 sample per vertex point

			//only draw a line in the middle,

			float angle = 0; // 
			float thisRadius = radius / (numLines-1); // the radius of this ring
			Vector3 firstPos = Vector3.zero; // the first position we use
			for(int j = 0; j < lineAttributes.lineSegments-1; j++)
			{
				//place each segment around the circle, wiht the given radius
				float rad = Mathf.Deg2Rad*angle;
				float x = Mathf.Cos(rad)*thisRadius;
				float y = samples[j]*maxHeight * AudioSampler.instance.globalSensitivity; // y value based on audio data
				float z = Mathf.Sin(rad)*thisRadius;
				Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
				lines[0].SetPosition(j,pos);
				angle += angleStep;
				if(j == 0)
				{
					firstPos = pos;
				}
			}
			lines[0].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			
			

		}

        /// <summary>
        /// Display the audio across all rings
        /// </summary>
        void Wave()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / lineAttributes.lineSegments;
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				float[] samples = AudioSampler.instance.GetAudioSamples(audioIndex,lineAttributes.lineSegments, true,useAudioFile); //take 1 sample per vertex point
				Vector3 firstPos = Vector3.zero; // the first position we use
				for(int j = 0; j < lineAttributes.lineSegments-1; j++)
				{
					
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = samples[j]*maxHeight * AudioSampler.instance.globalSensitivity;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
					if(j == 0)
					{
						firstPos = pos;
					}
				}
				lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			}
		}

        /// <summary>
        /// Display audio across all rings, damped by distance from the center
        /// </summary>
        void DampWave()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / lineAttributes.lineSegments;
			float[] samples = AudioSampler.instance.GetAudioSamples(audioIndex,lineAttributes.lineSegments, true,useAudioFile); //take 1 sample per vertex point

			for(int i = 0 ; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				Vector3 firstPos = Vector3.zero; // the first position we use
				float dampening = (1-(float)i/(numLines-1)) * AudioSampler.instance.globalSensitivity; // goes from 1 to 0. dampens the height
				for(int j = 0; j < lineAttributes.lineSegments-1; j++)
				{
					
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = samples[j]*maxHeight*dampening;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
					if(j == 0)
					{
						firstPos = pos;
					}
				}
				lines[i].SetPosition(lineAttributes.lineSegments-1,firstPos); // set the last pos = to the first pos.
			}

		}

        /// <summary>
        /// Move rings up and down, based on audio
        /// </summary>
        void Bounce()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / (lineAttributes.lineSegments-1);
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				float[] samples = AudioSampler.instance.GetAudioSamples(audioIndex,numLines, true,useAudioFile); //take 1 sample per vertex point
				float y = samples[i]*maxHeight * AudioSampler.instance.globalSensitivity;
				for(int j = 0; j < lineAttributes.lineSegments; j++)
				{
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float z = Mathf.Sin(rad)*thisRadius;
					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.up*y + this.transform.forward*z;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
				}
			}
		}

        /// <summary>
        /// Create "numlines" circles, from a radius of 0 to "radius".
        /// </summary>
        void CreatePad()
		{
			float radiusStep = radius / (numLines-1);
			float angleStep = 360f / (lineAttributes.lineSegments-1);
			for(int i = 0; i < numLines; i++)
			{
				float angle = 0; // 
				float thisRadius = radiusStep*i; // the radius of this ring
				for(int j = 0; j < lineAttributes.lineSegments; j++)
				{
					float rad = Mathf.Deg2Rad*angle;
					float x = Mathf.Cos(rad)*thisRadius;
					float y = Mathf.Sin(rad)*thisRadius;

					Vector3 pos = this.transform.position + this.transform.right*x + this.transform.forward*y;
					lines[i].SetPosition(j,pos);
					angle += angleStep;
				}
			}
		}

        /// <summary>
        /// Create a new lineRenderer, with the given color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        LineRenderer NewLine(Color c)
		{
			GameObject line = new GameObject ();
			line.transform.position = this.transform.position;
			line.transform.rotation = this.transform.rotation;
			line.hideFlags = HideFlags.HideInHierarchy;
			LineRenderer lr = line.AddComponent<LineRenderer> ();
			if(lineAttributes.lineMat == null)
			{
				lr.material = new Material(Shader.Find("Particles/Additive"));
			}
			else
			{
				lr.material = lineAttributes.lineMat;
			}
			lr.SetColors (c, c);
			lr.SetWidth (lineAttributes.startWidth,lineAttributes.endWidth);
			lr.SetVertexCount (lineAttributes.lineSegments);

			return lr;
		}
	}
}

