using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AudioVisualizer
{
    /// <summary>
    /// Similar to AudioEventListener, but uses pre-recorded tracks instead of live ones.
    /// </summary>
    public class AudioFileEventListener : MonoBehaviour
    {


        //____________Public Variables
        [Tooltip("Index into the AudioSampler audioFiles list")]
        public int audioIndex = 0; // index for the AudioSampler.audioFiles list
        [Tooltip("OnBeat events will trigger x seconds before the beat (Requires SilentAudio or AudioFiles)")]
        public float preBeatOffset = 0; // OnBeat events will trigger "preBeatOffset" seconds before the beat occurs.
        [Tooltip("The frequency range you want to listen to")]
        public FrequencyRange frequencyRange = FrequencyRange.Decibal; // what frequency will we listen to? 


        //____________Delegates/Actions

        [Tooltip("Calls public events when a beat is detected")]
        public UnityEvent OnBeat; // call public events in the inspector when a beat is detected
        [Tooltip("Adjusts public float values between min and max")]
        public OnPublicFrequencyChanged onFrequencyChanged;
        
        //a public event and class can subscribe too, see "BallLauncher.cs" for an example
        public delegate void BeatEvent(Beat b);
        public static BeatEvent OnBeatRecognized;

        //____________Protected Variables

        //____________Private Variables

        float timer = 0;
        float nextBeatTime = 0;
        float lastLooptime = 0;
        int beatIndex = 0;
        AudioData audioData;
        

        /*________________Monobehaviour Methods________________*/


        void OnEnable()
        {
            //subscribe to events
            AudioSampler.OnStop += ResetBeats;
            AudioSampler.AudioUpdate += AudioUpdate;
        }

        void OnDisable()
        {
            //un-subscribe to events
            AudioSampler.OnStop -= ResetBeats;
            AudioSampler.AudioUpdate -= AudioUpdate;
        }

        void Start()
        {
            audioData = AudioSampler.instance.GetAudioData(audioIndex);
            if(audioData.beats.Count == 0)
            {
                Debug.LogError("No beats were recorded for this Audio File");
            }
            ResetBeats();
        }

        /*________________Public Methods________________*/

        /// <summary>
        /// Reset the beats when a song is re-started. Start from the first valid beat.
        /// </summary>
        public void ResetBeats()
        {
            int i = 0;
            nextBeatTime = -1;
            //find the first beat, that's > preBeatOffset
            while (nextBeatTime < preBeatOffset && i < audioData.beats.Count)
            {
                nextBeatTime = audioData.beats[i].time - preBeatOffset;
                i++;
            }
            beatIndex = i;

            //Debug.Log("-------------------------------First beat at: " + nextBeatTime);
        }

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/

        /// <summary>
        /// The AudioSampler update loop for staying in sync with the audio
        /// </summary>
        /// <param name="audioTime"></param>
        /// <param name="deltaTime"></param>
        void AudioUpdate(float audioTime, float deltaTime)
        {
            //wait until we've initialized
            if (audioData == null)
                return;

            float loopTime = audioTime % audioData.clipLength;
            //Debug.Log(loopTime + "/" + audioData.clipLength + "> " + (nextBeatTime - preBeatOffset));

            //check to see if we've looped
            if(loopTime < lastLooptime)
            {
                //Debug.Log("LOOP ---------------------");
                ResetBeats();
            }
            lastLooptime = loopTime;

            //if we're looping, reset the beat counter
            if (beatIndex >= audioData.beats.Count)
                return;
            
            
            if (loopTime >= nextBeatTime - preBeatOffset)
            {
                //Debug.Log("Play File Beat: " + nextBeatTime + " audioTime: " + audioTime);
                Beat beat = audioData.beats[beatIndex];
                nextBeatTime = beat.time;
                beatIndex++;
                OnBeat.Invoke();

                if (OnBeatRecognized != null)
                {
                    OnBeatRecognized.Invoke(beat);
                }
            }
           
            HandleFrequencyEvents();
            
        }

        /// <summary>
        /// Adjust float values in the inspector using the public OnFrequencyChange UnityEvent<float>
        /// </summary>
        void HandleFrequencyEvents()
        {
            //get a value between min-max, using the frequency.
            if (onFrequencyChanged.onChange != null)
            {
                float delta = onFrequencyChanged.maxValue - onFrequencyChanged.minValue; //delta = max-min
                float frequency = AudioSampler.instance.GetFrequencyVol(audioIndex, frequencyRange,true);
                float[] samples = AudioSampler.instance.GetAudioSamples(audioIndex,true);
                float scaledValue = onFrequencyChanged.minValue + delta * AudioEventListener.GetNormalizedFrequency(frequency, samples); //min + delta*frequency
                onFrequencyChanged.onChange.Invoke(scaledValue);
            }
        }

       
    }
}
