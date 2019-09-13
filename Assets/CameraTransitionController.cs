using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ibuprogames.CameraTransitionsAsset;
using BGE.Forms;

public class CameraTransitionController : MonoBehaviour
{

    public GameObject audioThing;

    CameraTransition cameraTransition;
    public List<GameObject> leftEffects = new List<GameObject>();
    public List<GameObject> rightEffects = new List<GameObject>();

    public TestVideoPlayer videoPlayer;

    public int left = -1;
    public int right = -1;
    public int video = -1;

    public enum State { Hidden, Waiting, FadeIn, Showing, FadeOut };

    public State state = State.Hidden;

    public void ShowEffect(GameObject effect)
    {
        int leftIndex = leftEffects.IndexOf(effect);
        if (leftIndex != -1)
        {
            left = leftIndex;
            ShowLeftEffect();
            return;
        }
        int rightIndex = rightEffects.IndexOf(effect);
        if (rightIndex != -1)
        {
            right = rightIndex;
            ShowRightEffect();
            return;
        }


    }



    // Use this for initialization
    void Start()
    {
        cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
        cameraTransition.ProgressMode = CameraTransition.ProgressModes.Manual;
        if (cameraTransition == null)
            Debug.LogWarning(@"CameraTransition not found.");
        cameraTransition.DoTransition(CameraTransitionEffects.Simple, cameraA, cameraB, 2.0f, false, null);

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject effect = transform.GetChild(i).gameObject;
            effect.SetActive(false);
        }

        left = -1;
        right = -1;
        video = -1;
        state = State.Hidden;
        ellapsed = 0;
    }

    public Camera cameraA;
    public Camera cameraB;

    [Range(0, 0.99f)]
    public float progress = 0;

    public float overlayOpacity = 0.4f;
    public float fadeTime = 1.0f;

    float t = 0;

    public void RandomiseEffects()
    {
        left = Random.Range(0, leftEffects.Count);
        right = Random.Range(0, rightEffects.Count);
        video = Random.Range(0, videoPlayer.videos.Count);
    }

    public void ShowLeftEffect()
    {
        Debug.Log("Showing Left Effect");
        state = State.FadeIn;
        leftEffects[left].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null) StopCoroutine(hideCr);
    }

    public void ShowRightEffect()
    {
        Debug.Log("Showing RightEffect");
        state = State.FadeIn;
        rightEffects[right].SetActive(true);
        audioThing.SetActive(true);
        if (hideCr != null) StopCoroutine(hideCr);

    }

    public void ShowVideo()
    {
        Debug.Log("Showing video");
        state = State.Waiting;
        waitTime = 0;
        videoPlayer.PlayVideo(video);
        videoPlayer.gameObject.SetActive(true);
        audioThing.SetActive(false);
        if (hideCr != null) StopCoroutine(hideCr);
    }

    public void HideEffect()
    {
        Debug.Log("Hiding effect");
        state = State.FadeOut;
        if (left != -1)
        {
            StartCoroutine(DisableEffectAfter(leftEffects[left], fadeTime));
        }

        if (right != -1)
        {
            StartCoroutine(DisableEffectAfter(rightEffects[right], fadeTime));
        }
        if (video != -1)
        {
            StartCoroutine(DisableEffectAfter(videoPlayer.gameObject, fadeTime));
        }
        hideCr = StartCoroutine(DisableEffectAfter(audioThing, fadeTime));
        left = -1;
        right = -1;
        video = -1;
    }

    Coroutine hideCr = null;

    public float ellapsed = 0;
    public float toPass = 0.3f;

    public float waitTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            if (state == State.Hidden)
            {
                ellapsed = 0;
                video = (video + 1) % videoPlayer.videos.Count;
            }
            else
            {
                HideEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            if (state == State.Hidden)
            {
                ellapsed = 0;
                left = (left + 1) % leftEffects.Count;
            }
            else
            {
                HideEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            if (state == State.Hidden)
            {
                ellapsed = 0;
                right = (right + 1) % rightEffects.Count;
            }
            else
            {
                HideEffect();
            }
        }


        switch (state)
        {
            case State.Waiting:
                t = 0;
                waitTime += Time.deltaTime;
                if (waitTime > 0.2f)
                {
                    state = State.FadeIn;
                }
                break;
            case State.FadeIn:
                t += (Time.deltaTime * (1.0f / fadeTime));
                if (t >= 1.0f)
                {
                    t = 1.0f;
                    state = State.Showing;
                }
                break;
            case State.FadeOut:
                t -= (Time.deltaTime * (1.0f / fadeTime));
                if (t <= 0.0f)
                {
                    t = 0;
                    state = State.Hidden;
                }
                break;
        }
        cameraTransition.Progress = Utilities.Map(t, 0, 1, 0.0f, overlayOpacity);

        ellapsed += Time.deltaTime;
        if (state == State.Hidden && ellapsed > toPass)
        {
            if (left > -1)
            {
                ShowLeftEffect();
            }
            if (right > -1)
            {
                ShowRightEffect();
            }
            if (video > -1)
            {
                ShowVideo();
            }
        }
    }
    System.Collections.IEnumerator DisableEffectAfter(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        SchoolGenerator sg = effect.GetComponent<SchoolGenerator>();
        if (sg != null)
        {
            sg.Suspend();
        }
        effect.SetActive(false);
    }
}


