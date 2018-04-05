using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraTransitions;

public class CameraTransitionController : MonoBehaviour {

    CameraTransition cameraTransition;
    public List<GameObject> effects = new List<GameObject>();
    public int next = 0;
    public enum Button { left, right };
    public Button button = Button.left;
    // Use this for initialization
    void Start () {
        cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
        cameraTransition.ProgressMode = CameraTransition.ProgressModes.Manual;
        if (cameraTransition == null)
            Debug.LogWarning(@"CameraTransition not found.");
        cameraTransition.DoTransition(CameraTransitionEffects.FadeToColor, cameraA, cameraB, 2.0f);


        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject effect = transform.GetChild(i).gameObject;
            effects.Add(effect);
            effect.SetActive(false);
        }
    }

    public Camera cameraA;
    public Camera cameraB;

    [Range (0, 0.99f)]
    public float progress = 0;

    float targetProgress = 0;
    // Update is called once per frame
    void Update () {
        cameraTransition.Progress = progress;
        UnityEngine.KeyCode key = (button == Button.left) ? KeyCode.Joystick1Button8 : KeyCode.Joystick1Button9;
        if (Input.GetKeyDown(key))
        {
            if (progress < 0.5)
            {
                targetProgress = 0.55f;
                effects[next].SetActive(true);
            }
            else
            {
                targetProgress = 0;
                StartCoroutine(DisableEffectAfter(effects[next], 2));
                next = (next + 1) % effects.Count;
                
            }            
        }
        progress = Mathf.Lerp(progress, targetProgress, Time.deltaTime * .5f);
 
    }

    System.Collections.IEnumerator DisableEffectAfter(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        effect.SetActive(false);
    }


}
