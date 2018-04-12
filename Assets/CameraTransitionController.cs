using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraTransitions;

public class CameraTransitionController : MonoBehaviour {

    public GameObject audioThing;
    CameraTransition cameraTransition;
    public List<GameObject> leftEffects = new List<GameObject>();
    public List<GameObject> rightEffects = new List<GameObject>();
    public int nextLeft = 0;
    public int nextRight = 0;
    // Use this for initialization
    void Start () {
        cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
        cameraTransition.ProgressMode = CameraTransition.ProgressModes.Manual;
        if (cameraTransition == null)
            Debug.LogWarning(@"CameraTransition not found.");
        cameraTransition.DoTransition(CameraTransitionEffects.FadeToColor, cameraA, cameraB, 2.0f, new object[] { 0.0f, Color.clear});


        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject effect = transform.GetChild(i).gameObject;
            //leftEffects.Add(effect);
            effect.SetActive(false);
        }
    }

    public Camera cameraA;
    public Camera cameraB;

    [Range (0, 0.99f)]
    public float progress = 0;

    float targetProgress = 0;

    public void ShowLeftEffect()
    {
        targetProgress = 0.55f;
        leftEffects[nextLeft].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null ) StopCoroutine(hideCr);
    }

    public void ShowRightEffect()
    {
        targetProgress = 0.55f;
        rightEffects[nextRight].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null) StopCoroutine(hideCr);

    }

    public void HideEffect()
    {
        Debug.Log("Hiding effect");
        targetProgress = 0;
        StartCoroutine(DisableEffectAfter(leftEffects[nextLeft], 3));
        StartCoroutine(DisableEffectAfter(rightEffects[nextRight], 3));
        hideCr = StartCoroutine(DisableEffectAfter(audioThing, 3));
        nextLeft = (nextLeft + 1) % leftEffects.Count;
        nextRight = (nextRight + 1) % rightEffects.Count;
    }

    Coroutine hideCr = null;

    void Update () {
        cameraTransition.Progress = progress;
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            if (progress < 0.4)
            {
                ShowLeftEffect();
            }
            else
            {
                HideEffect();
            }            
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            if (progress < 0.4)
            {
                ShowRightEffect();
            }
            else
            {
                HideEffect();
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
