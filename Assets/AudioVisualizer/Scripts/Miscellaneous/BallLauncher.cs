using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AudioVisualizer
{
    public class BallLauncher : MonoBehaviour
    {
        public AudioFileEventListener eventListener;
        public List<Transform> spawnPoints;
        public List<Transform> targets;
        int spawnIndex = 0;
        float lastVolume = 0;

        private void OnEnable()
        {
            //subscribe to AudioFile beat events
            AudioFileEventListener.OnBeatRecognized += LaunchBall;
        }

        private void OnDisable()
        {
            //subscribe to AudioFile beat events
            AudioFileEventListener.OnBeatRecognized -= LaunchBall;
        }

        public void LaunchBall(Beat beat)
        {
            //chose which point to spawn from
            if (beat.volume > lastVolume)
            {
                spawnIndex++;
            }
            else
            {
                spawnIndex--;
            }
            if (spawnIndex >= spawnPoints.Count)
            {
                spawnIndex = spawnPoints.Count - 1;
            }
            if (spawnIndex < 0)
            {
                spawnIndex = 0;
            }

            //record the volume for the next time
            lastVolume = beat.volume;

            //grab a ball from the objectpooler
            GameObject ball = ObjectPooler.instance.GetObject("Ball");
            //place it at the correct spawnPoint
            ball.transform.position = spawnPoints[spawnIndex].position;
            ball.SetActive(true);
            //launch the ball towards the correct target
            Ball ballScript = ball.GetComponent<Ball>();
            ballScript.LaunchBall(targets[spawnIndex], eventListener.preBeatOffset);
        }
    }
}
