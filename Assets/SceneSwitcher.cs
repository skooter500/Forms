using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string[] scenes; 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.U))
        {
            string sceneName = SceneManager.GetActiveScene().name;

            int i = 0;
            for (i = 0; i < scenes.Length; i++)
            {
                if (sceneName == scenes[i])
                {
                    break;
                }
            }
            i = (i + 1) % scenes.Length;
            SceneManager.LoadScene(scenes[i]);            
        }
    }
}
