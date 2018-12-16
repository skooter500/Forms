using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// This script create an audio waveform on top of a UI panel.
    /// </summary>
	[RequireComponent(typeof(RectTransform))]
    public class PanelWaveform : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Index into the AudioSampler audioSources or audioFiles list")]
        public int audioIndex = 0; // index into audioSampler audioSources or audioFIles list. Determines which audio source we want to sample
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 
        [Tooltip("How sensitive to the audio are we?")]
        public float sensitivity = 2; // how sensitive is this script to the audio. This value is multiplied by the audio sample data.
        [Tooltip("The sprite to be used in each cell")]
        public Sprite sprite; // the sprite used for each cell of the waveform
        [Tooltip("Number of columns in the waveform")]
        public int numColumns = 10; //number of columns in our waveform
        [Tooltip("Number of rows in the waveform")]
        public int numRows = 10; //number of rows in our waveform
        [Tooltip("Space between each column")]
        public int spacingX = 0; //space between each column
        [Tooltip("Space between each row")]
        public int spacingY = 0; //space between each row
		//we'll use a gradient of colors, bottom to top
        [Tooltip("Color at the bottom of the waveform")]
		public Color bottomColor = Color.white;
        [Tooltip("Color at the top of the waveform")]
        public Color topColor = Color.white;
        [Tooltip("How fast the panel is updated")]
        public float lerpSpeed = 20f; // how fast the panel is updated
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        float[] samples;
		List<GameObject> cells = new List<GameObject> (); //private list of object pooled sprites.
		Gradient colorGradient;
		float widthPerImage;
		float heightPerImage;
		//track data from last update
		private int lastCol;
		private int lastRow;

        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void Awake () 
		{
		
			//set RectTransform parameters
			RectTransform rectTransform = this.GetComponent<RectTransform> ();
			rectTransform.anchorMin = new Vector2(0,0);
			rectTransform.anchorMax = new Vector2(1,1);
			rectTransform.anchoredPosition = new Vector2(1,1);

			colorGradient = GetColorGradient (bottomColor, topColor); // create the color gradient
            samples = new float[numColumns];


        }

		void Start()
		{
			CreateImages (); // instantiate sprites
			SetWidthAndHeight (); //set width and height vals
		}
		
		
		void FixedUpdate () 
		{

			DrawWaveform (); // place and color sprites on the panel


			//if the user changed teh data during runtime, re-create our images.
			if(lastCol != numColumns || lastRow != numRows)
			{
               
				Reset();
			}

			lastCol = numColumns;
			lastRow = numRows;
		}

        /*________________Public Methods________________*/

        /// <summary>
        /// Creating a color gradient, straight from the unity docs, with added alpha component
        /// </summary>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        /// <returns></returns>
        public static Gradient GetColorGradient(Color startColor, Color endColor)
        {
            Gradient g = new Gradient();

            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            GradientColorKey[] gck = new GradientColorKey[2];
            gck[0].color = startColor;
            gck[0].time = 0.0f;
            gck[1].color = endColor;
            gck[1].time = 1.0f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            GradientAlphaKey[] gak = new GradientAlphaKey[2];
            gak[0].alpha = startColor.a;
            gak[0].time = 0.0f;
            gak[1].alpha = endColor.a;
            gak[1].time = 1.0f;

            g.SetKeys(gck, gak);
            return g;
        }

        /// <summary>
        /// Reset the panel, destroy cells & re-create them.
        /// </summary>
        public void Reset()
		{
            samples = new float[numColumns];
            DestroyCells();
			CreateImages ();
			SetWidthAndHeight ();
		}

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        /// <summary>
        /// enable or disable cells, based on the the audio waveform
        /// </summary>
        void DrawWaveform() 
		{
			float[] audioSamples;

            if (frequencyRange == FrequencyRange.Decibal)
            {
                audioSamples = AudioSampler.instance.GetAudioSamples(audioIndex, numColumns, true,useAudioFile);
            }
            else
            {
                audioSamples = AudioSampler.instance.GetFrequencyData(audioIndex, frequencyRange, numColumns, true,false);
            }

            float delta = Time.deltaTime * lerpSpeed;
            for(int i = 0; i < numColumns; i++)
            {
                samples[i] = Mathf.Lerp(samples[i], audioSamples[i], delta);
            }

			int index = 0;
            //render the correct objects
            float s = sensitivity * AudioSampler.instance.globalSensitivity;

			for(int r = 0; r < numRows; r++) // for each row
			{
                float currentHeight = (float)r / numRows; // the % this row is out of numrows, i.e. how high we are

                for (int c = 0; c < numColumns; c++)
				{
					float sampleHeight = Mathf.Abs(samples[c])*s; //get an audio sample for each column

					
					if(currentHeight <= sampleHeight) // if this height is < our sample height, enable the cell
					{
                        cells[index].SetActive(true);
					}
					else // otherwise we should turn this cell off
					{
                        cells[index].SetActive(false);
					}
					index++;
				}
			}
		}

        /// <summary>
        /// Set the width and height of each sprite
        /// </summary>
        void SetWidthAndHeight()
		{
			widthPerImage = this.GetComponent<RectTransform> ().rect.width / numColumns;
			heightPerImage = this.GetComponent<RectTransform> ().rect.height / numRows;

			//Debug.Log ("ChildCount = " + this.transform.parent.childCount);

			//handle layoutGroups!
			if(this.transform.parent.GetComponent<VerticalLayoutGroup>())
			{
				heightPerImage = heightPerImage/this.transform.parent.childCount; //(float)this.transform.parent.childCount / numRows;
			}
			if(this.transform.parent.GetComponent<HorizontalLayoutGroup>())
			{
				widthPerImage = widthPerImage/this.transform.parent.childCount; //(float)this.transform.parent.childCount / numColumns;
			}
			if(this.transform.parent.GetComponent<GridLayoutGroup>())
			{
				GridLayoutGroup glg = this.transform.parent.GetComponent<GridLayoutGroup>();
				widthPerImage = glg.cellSize.x / numColumns;
				heightPerImage = glg.cellSize.y / numRows;
			}
		}

        /// <summary>
        /// Create all the sprites
        /// </summary>
        void CreateImages()
		{
			//Debug.Log ("Rect for : " + this.gameObject.name + " = " + this.GetComponent<RectTransform>().rect);
			cells = new List<GameObject> ();
			for(int r = 0; r < numRows; r++)
			{
				for(int c = 0; c < numColumns; c++)
				{
                    GameObject newObj = new GameObject();
					newObj.transform.position = this.transform.position;
					newObj.transform.rotation = this.transform.rotation;

					newObj.SetActive(true);
					newObj.name = "Image_" + r + "x" + c;
					newObj.transform.SetParent(this.transform);

					Image newImage = newObj.AddComponent<Image>();
					newImage.sprite = sprite;

					//set the rectTransform values
					newImage.rectTransform.pivot = new Vector2(0,0); // pivot to bottom left of image
					newImage.rectTransform.anchorMin = new Vector2(0,0); // anchor to bottom left of parent panel
					newImage.rectTransform.anchorMax = new Vector2(0,0);
					newImage.rectTransform.localScale = Vector3.one;
					//scale the image
					newImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,(widthPerImage-spacingX*2));// = new Vector2(thisRect.rect.width,panelHeight);
					newImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,(heightPerImage-spacingY*2));//panelHeight/thisRect.rect.height);
					
					//position the image
					float xPos = c*widthPerImage;// + widthPerImage*.5f;
					float yPos = r*heightPerImage;//+ heightPerImage*.5f;
					newImage.rectTransform.anchoredPosition = new Vector3(xPos,yPos,0);
					newImage.color = colorGradient.Evaluate((float)r/numRows);

					cells.Add(newObj);

				}
			}
		}


        /// <summary>
        /// If public variables like numColums or numRows have changed, destroy the old cells
        /// </summary>
        void DestroyCells()
		{
			foreach(GameObject cell in cells)
			{
				cell.SetActive(false);
				//Destroy(cell);
			}
		}
	}
}
