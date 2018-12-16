using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace AudioVisualizer
{
    /// <summary>
    /// This script create an audio waveform on top of a UI panel.
    /// </summary>
    public class ArcWaveform : MonoBehaviour
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
        [Tooltip("The angle of the arc, max = 360")]
        [Range(0,360)]
        public float angle = 180;
        [Tooltip("The radius of the arc")]
        public float radius = 10f;
        [Tooltip("The height of the waveform \n effects the height of each sprite")]
        public float height = 5f;
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
        [Tooltip("How fast the waveform moves")]
        public float lerpSpeed = 20f; // how fast the panel is updated
        [Tooltip("Sample from a recorded AudioFile?")]
        public bool useAudioFile = false; // flag saying if we should use a pre-recorded audio file
        [Tooltip("Take the absolute value of samples?")]
        public bool abs = false; // use absolute value or not

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        GameObject[,] topCells; //midline and above
        GameObject[,] bottomCells; //below midline
        float[] samples;
        Gradient colorGradient;
        float widthPerImage;
        float heightPerImage;
        //track data from last update
        int lastCol;
        int lastRow;
        float lastRadius;
        float lastHeight;
        float lastAngle;
        bool lastAbs = false;



        /*________________Monobehaviour Methods________________*/

  
        void Start()
        {
            colorGradient = GetColorGradient(bottomColor, topColor); // create the color gradient
            
            CreateImages(); // instantiate sprites

            samples = new float[numColumns];
        }


        void FixedUpdate()
        {
            
            DrawWaveform(); 


            //if the user changed teh data during runtime, re-create our images.
            if (lastCol != numColumns || lastRow != numRows || lastAbs != abs)
            {
                Reset();
            }
            if(lastRadius != radius || lastAngle != angle || lastHeight != height)
            {
                PositionAndScale();
            }

            lastCol = numColumns;
            lastRow = numRows;
            lastRadius = radius;
            lastAngle = angle;
            lastHeight = height;
            lastAbs = abs;
        }

        /*________________Public Methods________________*/

        /// <summary>
        /// creating a color gradient, straight from the unity docs, with added alpha component
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
        /// re-create necessary images
        /// </summary>
        public void Reset()
        {
            samples = new float[numColumns];

            DestroyCells();
            CreateImages();
            
        }

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        /// <summary>
        /// Set the width and height that will be used for each sprite
        /// </summary>
        void CalculateWidthAndHeight()
        {
            float circumference = 2 * Mathf.PI * radius * (angle / 360);
            widthPerImage = (circumference / numColumns - spacingX) * .5f;
            heightPerImage = (height / numRows - spacingY);
        }

        /// <summary>
        /// create all the sprites
        /// </summary>
        void CreateImages()
        {
            topCells = new GameObject[numRows, numColumns];
            if (!abs)
            {
                bottomCells = new GameObject[numRows, numColumns];
            }

            for (int r = 0; r < numRows; r++)
            {
                Color color = colorGradient.Evaluate((float)r / numRows);

                for (int c = 0; c < numColumns; c++)
                {
                    //create top sprites
                    topCells[r, c] = CreateSprite(r, c, color);

                    //maybe create bottom sprites
                    if (!abs)
                    {
                        bottomCells[r, c] = CreateSprite(-r, c, color);
                    }

                }
            }

            PositionAndScale();
        }

        /// <summary>
        /// Create a sprite that will be used for 1 cell in the waveform.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        GameObject CreateSprite(int r, int c, Color color)
        {
            GameObject newObj = new GameObject();
            newObj.name = "Image_" + r + "x" + c;
            newObj.transform.SetParent(this.transform);

            SpriteRenderer newSpriteRenderer = newObj.AddComponent<SpriteRenderer>();
            newSpriteRenderer.sprite = sprite;
            newSpriteRenderer.color = color;
            return newObj;
        }

        /// <summary>
        /// If public variables like numColums or numRows have changed, destroy the old cells
        /// </summary>
        void DestroyCells()
        {
            int rowLength = topCells.GetLength(0);
            int columnLength = topCells.GetLength(1);
            for (int r = 0; r < rowLength; r++)
            {
                for (int c = 0; c < columnLength; c++)
                {
                    Destroy(topCells[r, c]);
                }
            }
            rowLength = bottomCells.GetLength(0);
            columnLength = bottomCells.GetLength(1);
            for (int r = 0; r < rowLength; r++)
            {
                for (int c = 0; c < columnLength; c++)
                {
                    Destroy(bottomCells[r, c]);
                }
            }
        }

        /// <summary>
        /// Enable or disable cells, based on the the audio waveform
        /// </summary>
        void DrawWaveform() 
        {
            float[] audioSamples;

            if (frequencyRange == FrequencyRange.Decibal)
            {
                audioSamples = AudioSampler.instance.GetAudioSamples(audioIndex, numColumns, abs, useAudioFile);
            }
            else
            {
                audioSamples = AudioSampler.instance.GetFrequencyData(audioIndex, frequencyRange, numColumns, abs, false);
            }

            float delta = Time.deltaTime*lerpSpeed;
            for(int i = 0; i < numColumns; i++)
            {
                samples[i] = Mathf.Lerp(samples[i], audioSamples[i], delta);
            }

            int index = 0;
            //render the correct objects


            for (int c = 0; c < numColumns; c++)
            {
                //top and bottom....
                float sample = samples[c];
                float sampleHeight = Mathf.Abs(sample) * sensitivity * AudioSampler.instance.globalSensitivity; //get an audio sample for each column

                for (int r = 0; r < numRows; r++) // for each row
                {
                    
                    bool bottomCell = !abs && sample < 0;
                    //GameObject cell = bottomCell ? bottomCells[r,c] : topCells[r,c];

                    float currentHeight = (float)r / numRows; // the % this row is out of numrows, i.e. how high we are
                    if (currentHeight <= sampleHeight) // if this height is < our sample height, enable the cell
                    {
                        //if negative value
                        if(bottomCell)
                        {
                            //turn bottom cell on
                            bottomCells[r, c].SetActive(true);
                            //turn top off
                            topCells[r, c].SetActive(false);
                        }
                        else // if positive value
                        {
                            //turn top on
                            topCells[r, c].SetActive(true);
                            //turn bottom off
                            if(!abs)
                            {
                                bottomCells[r, c].SetActive(false);
                            }
                            
                        }
                    }
                    else // otherwise we should turn this cell off
                    {
                        //turn top off
                        topCells[r, c].SetActive(false);
                        //turn bottom off
                        if (!abs)
                        {
                            bottomCells[r, c].SetActive(false);
                        }
                    }
                    index++;
                }
            }
        }

        


        /// <summary>
        /// Position each sprite and scale it to fit.
        /// </summary>
        void PositionAndScale()
        {
            CalculateWidthAndHeight();

            //every column, the angle increases this much.
            float angleStep = angle / numColumns;
            //the starting angle for our arc. If angle = 180, startAngle = -90 so our arc can go from -90 to 90
            float startAngle = -(angle * .5f);

            //width and height of each image
            for (int c = 0; c < numColumns; c++)
            {
                //the angle for this column
                float a = startAngle + angleStep * c;
                float rad = Mathf.Deg2Rad * a;

                float x = Mathf.Sin(rad) * radius;
                float z = Mathf.Cos(rad) * radius;

                Vector3 bottomRowPos = this.transform.position + this.transform.right * x  + this.transform.forward * z;
                Vector3 lookDir = (this.transform.position - bottomRowPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(lookDir, this.transform.up);

                float rowHeight = heightPerImage * 2;

                for (int r = 0; r < numRows; r++)
                {               
                    float y = r * rowHeight;

                    //position
                    GameObject cell = topCells[r, c];
                    cell.transform.position = bottomRowPos + this.transform.up * y;
                    //rotation, towards center
                    cell.transform.rotation = rotation;
                    //scale
                    cell.transform.localScale = new Vector3(widthPerImage, heightPerImage, 1);

                    //if we have bottom rows, position and scale them as well
                    if(!abs)
                    {
                        //position
                        cell = bottomCells[r, c];
                        cell.transform.position = bottomRowPos - this.transform.up * y;
                        //rotation, towards center
                        cell.transform.rotation = rotation;
                        //scale
                        cell.transform.localScale = new Vector3(widthPerImage, heightPerImage, 1);
                    }
                }
            }
        }






       

    }
}
