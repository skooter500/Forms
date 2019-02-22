using UnityEngine;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class CreatureManager : MonoBehaviour {

        public int threadCount = 0;

        GUIStyle style = new GUIStyle();

        [HideInInspector]
        public static float threadTimeDelta;

        Thread thread;

        [HideInInspector]
        public List<Boid> boids = new List<Boid>();

        StringBuilder message = new StringBuilder();

        static CreatureManager instance;

        [Header("Debugging")]
        public bool showMessages;

        void DisablePrefabs()
        {
            GameObject[] prefabs = GameObject.FindGameObjectsWithTag("prefab");
            foreach (GameObject go in prefabs)
            {
                go.SetActive(false);
            }
        }

        public static void Log(string message)
        {
            if (instance != null)
            {
                Instance.message.Append(message + "\n");
            }
        }

        public static void PrintFloat(string message, float f)
        {
            if (instance != null)
            {
                Instance.message.Append(message + ": " + f + "\n");
            }
        }

        public static void PrintVector(string message, Vector3 v)
        {
            if (instance != null)
            {
                Instance.message.Append(message + ": (" + v.x + ", " + v.y + ", " + v.z + ")\n");
            }
        }

        CreatureManager()
        {
            instance = this;
        }

        void OnGUI()
        {
            if (showMessages)
            {
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "" + message, style);                
            }
            if (Event.current.type == EventType.Repaint)
            {
                message.Length = 0;
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.F4)
                {
                    showMessages = !showMessages;
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Application.Quit();
                }
            }
        }

        float minFPS = float.MaxValue;
        float maxFPS = float.MinValue;
        float avgFPS = 0;
        float sumFPS = 0;
        int frameCount = 0;

        public static CreatureManager Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {

            instance = this;
            style.fontSize = 18;
            style.normal.textColor = Color.white;
            Cursor.visible = false;

            DisablePrefabs();
        }

        // Use this for initialization
        void Start ()
        {
            /*
            boids = new List<Boid>(FindObjectsOfType<Boid>()); // Find all the boids
            foreach (Boid boid in boids)
            {
                boid.multiThreaded = true;
            }
            */
            StartUpdateThreads();
        }

        void Update()
        {
            frameCount++;
            //threadTimeDelta = Time.deltaTime;
            float fps = (1.0f / Time.deltaTime);
            if (fps < minFPS)
            {
                minFPS = fps;
            }
            if (fps > maxFPS)
            {
                maxFPS = fps;
            }
            sumFPS += fps;
            avgFPS = sumFPS / frameCount;
            PrintFloat("FPS: ", (int)fps);
            PrintFloat("Avg FPS: ", (int)avgFPS);
            PrintFloat("Min FPS: ", (int)minFPS);
            PrintFloat("Max FPS: ", (int)maxFPS);

            if (!thread.IsAlive)
            {
                StartUpdateThreads();
            }
            PrintFloat("Boid FPS: ", threadFPS);
            Log("Num boids:" + boids.Count);
            Log("Suspended boids:" + suspended);

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                showMessages = !showMessages;
            }
        }

        long lastThreadCount = 0;
        float threadFPS;

        int suspended = 0;

        void UpdateThread()
        {
            float maxFPS = 100.0f;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            
            while (running)
            {
                stopwatch.Reset();
                stopwatch.Start();
                suspended = 0;
                // Update all the boids
                for (int i = 0; i < boids.Count; i++)
                {
                    Boid boid = boids[i];
                    if (boid == null)
                    {
                        continue;
                    }
                    if (boid.suspended)
                    {
                        suspended++;
                    }
                    else
                    {
                        boid.force = boid.CalculateForce();
                    }
                }
                stopwatch.Stop();
                
                threadTimeDelta = (float) ((double) stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency);
                threadCount++;
                if (threadTimeDelta < 0.01f)
                {
                    Thread.Sleep(10);
                    threadTimeDelta = 0.01f;
                }
                threadFPS = 1.0f / threadTimeDelta;
            }
        }

        bool running = false;

        void OnApplicationQuit()
        {
            running = false;
        }



        void StartUpdateThreads()
        {
            running = true;
            Debug.Log("Starting thread...");

            for (int i = 0; i < boids.Count; i++)
            {
                Boid boid = boids[i];
                boid.multiThreaded = true;
                boid.UpdateLocalFromTransform();
            }
            thread = new Thread(UpdateThread);
            thread.Start();
        }
    }
}