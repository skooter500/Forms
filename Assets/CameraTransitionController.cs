using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ibuprogames.CameraTransitionsAsset;
using BGE.Forms;

public class CameraTransitionController : MonoBehaviour {

    public GameObject audioThing;

    CameraTransition cameraTransition;
    public List<GameObject> leftEffects = new List<GameObject>();
    public List<GameObject> rightEffects = new List<GameObject>();
    public int nextLeft = 0;
    public int nextRight = 0;

    public enum State {FadeIn, FadeOut, Normal };

    public State state = State.Normal;
    // Use this for initialization
    void Start () {
        cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
        cameraTransition.ProgressMode = CameraTransition.ProgressModes.Manual;
        if (cameraTransition == null)
            Debug.LogWarning(@"CameraTransition not found.");
        cameraTransition.DoTransition(CameraTransitionEffects.Simple, cameraA, cameraB, 2.0f, false, null);

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
    
    public float overlayOpacity = 0.4f;
    public float fadeTime = 1.0f;

    float t = 0;

    //public enum State {S

    public void ShowLeftEffect()
    {
        state = State.FadeIn;
        leftEffects[nextLeft].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null ) StopCoroutine(hideCr);
    }

    public void ShowRightEffect()
    {
        state = State.FadeIn;
        rightEffects[nextRight].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null) StopCoroutine(hideCr);

    }

    public void HideEffect()
    {
        Debug.Log("Hiding effect");
        state = State.FadeOut;
        StartCoroutine(DisableEffectAfter(leftEffects[nextLeft], fadeTime));
        StartCoroutine(DisableEffectAfter(rightEffects[nextRight], fadeTime));
        hideCr = StartCoroutine(DisableEffectAfter(audioThing, fadeTime));
        nextLeft = (nextLeft + 1) % leftEffects.Count;
        nextRight = (nextRight + 1) % rightEffects.Count;
    }

    Coroutine hideCr = null;

    void Update () {
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            if (cameraTransition.Progress  < overlayOpacity - 0.05f)
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
            if (cameraTransition.Progress  < overlayOpacity - 0.05f)
            {
                ShowRightEffect();
            }
            else
            {
                HideEffect();
            }
        }
        switch (state)
        {
            case State.FadeIn:
                t += (Time.deltaTime * (1.0f / fadeTime));
                if (t >= 1.0f)
                {
                    t = 1.0f;
                    state = State.Normal;
                }
                break;
            case State.FadeOut:
                t -= (Time.deltaTime * (1.0f / fadeTime));
                if (t <= 0.0f)
                {
                    t = 0;
                    state = State.Normal;
                }
                break;
        }
        cameraTransition.Progress = Utilities.Map(t, 0, 1, 0.0f, overlayOpacity);

    }

    System.Collections.IEnumerator DisableEffectAfter(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        effect.SetActive(false);
    }    
}
