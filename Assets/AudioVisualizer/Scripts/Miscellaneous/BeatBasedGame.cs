using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AudioVisualizer
{
    /// <summary>
    /// See the BeatBasedGame scene
    /// </summary>
    public class BeatBasedGame : MonoBehaviour
    {
        
        //public AudioFileEventListener eventListener;
        public float ballTravelTime = 2f; // should be the same delay as the AUdioEventListener
        public static BeatBasedGame Instance; // a singleton instance
        public List<string> spawnObjects; // the objects we're going to spawn
        public List<Transform> rings; // the rings, destination for our orbs
        public float spawnHeight = 10f; // how far up we spawn stuff
        public int audioIndex = 0;
        public bool useAudioFile = true;


        int spawnIndex = 0;
        float lastVolume = 0;

        private void OnEnable()
        {
            Instance = this;
            //subscribe to audiofile beat events
            if (useAudioFile)
            {
                AudioFileEventListener.OnBeatRecognized += SpawnOrb;
            }
            else
            {
                AudioEventListener.OnBeatRecognized += SpawnOrb;
            }
        }

        private void OnDisable()
        {
            Instance = null;
            //unsubscribe from audiofile beat events
            if (useAudioFile)
            {
                AudioFileEventListener.OnBeatRecognized -= SpawnOrb;
            }
            else
            {
                AudioEventListener.OnBeatRecognized -= SpawnOrb;
            }
        }

        //called in an AuidioFileEventListener.OnBeat event, 'preBeatOffset'seconds before the beat happens
        //spawn an orb at one of the spawnPoints
        //travel time is the time allowed for the orb to reach it's destination
        public void SpawnOrb(Beat beat)
        {

            //chose which point to spawn from
            if(beat.volume > lastVolume)
            {
                spawnIndex++;
            }
            else
            {
                spawnIndex--;
            }
            if(spawnIndex >= rings.Count)
            {
                spawnIndex = rings.Count -1;
            }
            if(spawnIndex < 0)
            {
                spawnIndex = 0;
            }

            //Debug.Log("Index: " + spawnIndex);
            //grab an object to spawn from our objectpooler
            GameObject spawnObj = ObjectPooler.instance.GetObject(spawnObjects[spawnIndex]);
            //place it "spawnHeight" above the appropriate ring
            spawnObj.transform.position = rings[spawnIndex].position + Vector3.up*spawnHeight;
            //set the speed so it reaches the ring on time
            Orb orbScript = spawnObj.GetComponent<Orb>();
            orbScript.speed = spawnHeight / ballTravelTime;
            spawnObj.SetActive(true); // enable the object

            lastVolume = beat.volume;
        }

        //called from Objectpooler.OnLoadComplete
        public void StartGame()
        {
            //AudioSampler.instance.PlayWithPreBeatOffset(ballTravelTime);

            //if using an AudioFile for beat detection, we can start playing right away.
            if (useAudioFile)
            {
                AudioSampler.instance.Play();
            }
            else // if we're doing runtime beat detection
            {
                //start playing audio in "ballTravelTime" seconds
                AudioSampler.instance.PlayWithPreBeatOffset(ballTravelTime);
            }
        }
    }
}
