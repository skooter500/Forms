///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Uncomment to activate logs.
//#define ENABLE_LOGS

using System;
using System.Collections;

using UnityEngine;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// Camera transition manager.
    /// </summary>
    [AddComponentMenu("Camera Transitions/Camera Transition")]
    public sealed class CameraTransition : MonoBehaviour
    {
      /// <summary>
      /// The current transition type.
      /// </summary>
      public CameraTransitionEffects Transition
      {
        get { return transition; }
        set
        {
          if (value != transition)
          {
            transition = value;

            if (fromCamera != null)
              UpdateEffect(transition);
          }
        }
      }

      /// <summary>
      /// Transition progress [0 - 1].
      /// </summary>
      public float Progress
      {
        get { return (currentEffect != null) ? currentEffect.Progress : 0.0f; }
        set
        {
          if (currentEffect != null && currentEffect.Progress != value)
          {
            if (progressMode == ProgressModes.Manual)
              currentEffect.Progress = value;
            else
              Debug.LogWarning(@"[Ibuprogames.CameraTransitions] You can not change progress in automatic mode. Change to manual mode.");
          }
        }
      }

      /// <summary>
      /// From camera.
      /// </summary>
      public Camera FromCamera
      {
        get { return fromCamera; }
        set
        {
          if (currentEffect != null)
            Destroy(currentEffect);

          if (value != null)
          {
            if (value != toCamera)
            {
              fromCamera = value;
                
              UpdateEffect(transition);
            }
            else
              Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Cameras must be different.");
          }
          else
            fromCamera = null;
        }
      }

      /// <summary>
      /// To camera.
      /// </summary>
      public Camera ToCamera
      {
        get { return toCamera; }
        set
        {
          if (toCamera != null && toCamera.targetTexture != null && renderTextureMode == RenderTextureModes.Automatic)
          {
#if ENABLE_LOGS
            Debug.LogFormat(@"[Ibuprogames.CameraTransitions] ToCamera: '{0}' TargetTexture set to null.", toCamera.gameObject.name);
#endif
            toCamera.targetTexture = null;
          }

          if (value != null)
          {
            if (value != fromCamera)
            {
              toCamera = value;
              
              if (renderTexture != null && renderTextureMode == RenderTextureModes.Automatic)
              {
#if ENABLE_LOGS
                Debug.LogFormat(@"[Ibuprogames.CameraTransitions] ToCamera: '{0}' TargetTexture set to '{1}'.", toCamera.gameObject.name, renderTexture.name);
#endif
                toCamera.targetTexture = renderTexture;
              }
            }
            else
              Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Cameras must be different.");
          }
          else
            toCamera = null;
        }
      }

      /// <summary>
      /// Duration of the transition in seconds [>0].
      /// </summary>
      public float Time
      {
        get { return transitionTime; }
        set
        {
          if (value > 0.0f)
            transitionTime = value;
          else
            Debug.LogWarning(@"[Ibuprogames.CameraTransitions] The duration of the effect must be greater than zero.");
        }
      }

      /// <summary>
      /// Is running some transition?
      /// </summary>
      public bool IsRunning
      {
        get { return isRunning; }
      }

      #region Advanced settings
      /// <summary>
      /// How the effect progresses.
      /// </summary>
      public enum ProgressModes
      {
        /// <summary>
        /// Move automatically (default).
        /// </summary>
        Automatic,

        /// <summary>
        /// Manual progress. You can use 'Progress' to set the progress.
        /// At the begin of the transition, progress is always 0 and not finalized until reach 1.
        /// </summary>
        Manual,
      }

      /// <summary>
      /// How handle the RenderTexture.
      /// </summary>
      public enum RenderTextureModes
      {
        /// <summary>
        /// Everything is done automatically (default).
        /// </summary>
        Automatic,
        
        /// <summary>
        /// The target camera must handle their own RenderTexture.
        /// </summary>
        Manual,
      }
      
      /// <summary>
      /// The size of the render texture. The smaller, the worse the quality.
      /// </summary>
      public enum RenderTextureSizes
      {
        /// <summary>
        /// The same as screen (default).
        /// </summary>
        SameAsScreen = 1,
        
        /// <summary>
        /// Half video size.
        /// </summary>
        HalfVideo = 2,
        
        /// <summary>
        /// Quarter video size.
        /// </summary>
        QuarterVideo = 4,
      }

      /// <summary>
      /// The destination camera is updated during the entire transition or only the first frame.
      /// </summary>
      public enum RenderTextureUpdateModes
      {
        /// <summary>
        /// All frames, awesome but more expensive (default).
        /// </summary>
        AllFrames = 1,

        /// <summary>
        /// Only the first frame, faster.
        /// Useful if the destination camera is a static frame.
        /// </summary>
        FirstFrameOnly = 2,
      }

      /// <summary>
      /// How the effect progresses.
      /// </summary>
      public ProgressModes ProgressMode
      {
        get { return progressMode; }
        set { progressMode = value; }
      }

      /// <summary>
      /// Use scaled time.
      /// </summary>
      public bool UseScaledTime
      {
        get { return useScaledTime; }
        set { useScaledTime = value; }
      }

      /// <summary>
      /// Custom time scale.
      /// </summary>
      public float CustomTimeScale
      {
        get { return timeScale; }
        set { if (value > 0.0f) timeScale = value; }
      }

      /// <summary>
      /// How handle the RenderTexture.
      /// </summary>
      public RenderTextureModes RenderTextureMode
      {
        get { return renderTextureMode; }
        set
        {
          if (value != renderTextureMode)
          {
            if (isRunning == false)
            {
              renderTextureMode = value;

#if ENABLE_LOGS
              Debug.LogFormat("[Ibuprogames.CameraTransitions] RenderTextureMode changed to '{0}'", renderTextureMode.ToString());
#endif
              if (renderTextureMode == RenderTextureModes.Automatic)
                createRenderTexture = true;
              else if (renderTexture != null)
              {
                renderTexture.DiscardContents();
                
                Destroy(renderTexture);
              }
            }
            else
              Debug.LogError("[Ibuprogames.CameraTransitions] A transition is active, try again when finished.");
          }
        }
      }

      /// <summary>
      /// Changes the RenderTexture quality.
      /// </summary>
      public RenderTextureSizes RenderTextureSize
      {
        get { return renderTextureSize; }
        set
        {
          if (value != renderTextureSize)
          {
            if (isRunning == false)
            {
              renderTextureSize = value;
              
              createRenderTexture = true;
            }
            else
              Debug.LogError(@"[Ibuprogames.CameraTransitions] A transition is active, try again when finished.");
          }
        }
      }

      /// <summary>
      /// Changes the RenderTexture quality.
      /// </summary>
      public RenderTextureUpdateModes RenderTextureUpdateMode
      {
        get { return renderTextureUpdateMode; }
        set
        {
          if (value != renderTextureUpdateMode)
          {
            if (isRunning == false)
              renderTextureUpdateMode = value;
            else
              Debug.LogError(@"[Ibuprogames.CameraTransitions] A transition is active, try again when finished.");
          }
        }
      }

      /// <summary>
      /// Inverts RenderTexture.
      /// </summary>
      public bool InvertRenderTexture
      {
        get { return invertRenderTexture; }
        set { invertRenderTexture = value; }
      }

      [SerializeField]
      private ProgressModes progressMode = ProgressModes.Automatic;

      [SerializeField]
      private bool useScaledTime = true;

      [SerializeField]
      private float timeScale = 1.0f;

      [SerializeField]
      private RenderTextureModes renderTextureMode = RenderTextureModes.Automatic;
      
      [SerializeField]
      private RenderTextureSizes renderTextureSize = RenderTextureSizes.SameAsScreen;

      [SerializeField]
      private RenderTextureUpdateModes renderTextureUpdateMode = RenderTextureUpdateModes.AllFrames;
      #endregion

      /// <summary>
      /// Transition start event.
      /// <param name="CameraTransitionEffects">Transition effect.</param>
      /// </summary>
      public event Action<CameraTransitionEffects> transitionStartEvent;

      /// <summary>
      /// Transition progress event.
      /// <param name="CameraTransitionEffects">Transition effect.</param>
      /// <param name="float">Transition progress.</param>
      /// </summary>
      public event Action<CameraTransitionEffects, float> transitionProgressEvent;

      /// <summary>
      /// Transition end event.
      /// <param name="CameraTransitionEffects">Transition effect.</param>
      /// </summary>
      public event Action<CameraTransitionEffects> transitionEndEvent;

      [SerializeField]
      private CameraTransitionEffects transition;

      [SerializeField]
      private Camera fromCamera;

      [SerializeField]
      private Camera toCamera;

      [SerializeField]
      private float transitionTime = 1.0f;

      [SerializeField]
      private bool invertRenderTexture = false;

      private float transitionLife = 0.0f;

      private RenderTexture renderTexture;

      private CameraTransitionBase currentEffect;

      private bool isRunning = false;

      private IEnumerator transitionCorutine = null;

      private bool createRenderTexture = true;

      private EaseType easeType = EaseType.Linear;

      private EaseMode easeMode = EaseMode.In;

      /// <summary>
      /// Create a transition effect between cameras.
      /// 
      /// At the end of the transition, the source camera (from) will be deactivated
      /// and the camera target (to) will be activated.
      /// 
      /// No AudioListener is modified.
      /// </summary>
      /// <param name="transition">The transition effect.</param>
      /// <param name="from">Source camera.</param>
      /// <param name="to">Target camera.</param>
      /// <param name="time">Duration in seconds (>0).</param>
      /// <param name="reverse">Invert the effect.</param>
      /// <param name="parameters">Parameters to configure the effects (optional).</param>
      /// <remarks>
      /// Transitions parameters:
      /// <list type="bullet">
      /// <item>CrossZoom: strength (float), quality (float).</item>
      /// <item>Cube: perspective (float), zoom (float), reflection (float), elevation (float).</item>
      /// <item>Doom: bar width (int), amplitude (float), noise (float), frequency (float).</item>
      /// <item>FadeToColor: strength (float), color (Color).</item>
      /// <item>FadeToGrayscale: strength (float).</item>
      /// <item>Flash: strength (float), intensity (float), zoom (float), velocity (float), color (Color).</item>
      /// <item>Fold: mode (CameraTransitionFold.Modes).</item>
      /// <item>Flip: mode (CameraTransitionFlip.Modes).</item>
      /// <item>Gate: perspective (float), depth (float), reflection (float).</item>
      /// <item>Glitch: strength (float).</item>
      /// <item>LinearBlur: intensity (float), passes (int).</item>
      /// <item>Mosaic: steps (Vector2), rotate (bool).</item>
      /// <item>PageCurl: angle (float), radius (float), shadows (bool).</item>
      /// <item>PageCurlAdvanced: angle (float), radius (float).</item>
      /// <item>Pixelate: size (float).</item>
      /// <item>Radial: clockwise (bool).</item>
      /// <item>RandomGrid: rows (int), columns (int), smoothness (float).</item>
      /// <item>Simple: strength (float).</item>
      /// <item>SmoothCircle: invert (bool), smoothness (float), center (Vector2).</item>
      /// <item>SmoothLine: angle (float), smoothness (float).</item>
      /// <item>Swap: perspective (float), depth (float), reflection (float).</item>
      /// <item>Valentine: border (float), color (Color).</item>
      /// <item>WarpWave: mode (CameraTransitionWarpWave.Modes), curvature (float).</item>
      /// </list>
      /// <example>
      /// cameraTransition.DoTransition(CameraTransitionEffects.FadeToColor, cameraA, cameraB, 1.0f, new object[] { 0.3f, Color.black });
      /// </example>
      /// </remarks>
      public void DoTransition(CameraTransitionEffects transition, Camera from, Camera to, float time, bool reverse, params object[] parameters)
      {
        if (from != null && to != null)
        {
          if (from != to)
          {
            if (time > 0.0f)
            {
              isRunning = true;

              CameraTransitionEffects oldTransition = Transition;

              //Progress = 0.0f;
              Transition = transition;
              FromCamera = from;
              ToCamera = to;
              transitionTime = time;
              transitionLife = 0.0f;

              from.gameObject.SetActive(false);
              to.gameObject.SetActive(true);
              from.gameObject.SetActive(true);

              currentEffect.Reverse = reverse;
              currentEffect.InvertRenderTexture = invertRenderTexture;

#if ENABLE_LOGS
              Debug.LogFormat("[Ibuprogames.CameraTransitions] Camera transition '{0}' from '{1}' to '{2}' using '{3}' in {4} seconds.", transition.ToString(), from.gameObject.name, to.gameObject.name, easeType.ToString(), time);
#endif
              if (parameters != null && parameters.Length > 0)
                currentEffect.SetParameters(parameters);

              if (IsRunning == true && transitionCorutine != null)
              {
                if (transitionEndEvent != null)
                  transitionEndEvent(oldTransition);

                StopCoroutine(transitionCorutine);
              }

              transitionCorutine = TransitionCoroutine();

              StartCoroutine(transitionCorutine);
            }
            else
              Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Duration must be greater than zero.");
          }
          else
            Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Cameras must be differents.");
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Invalid cameras.");
      }

      /// <summary>
      /// Same as previus funtion but with easing in/out equations.
      /// </summary>
      public void DoTransition(CameraTransitionEffects transition, Camera from, Camera to, float time, bool reverse, EaseType easeType, EaseMode easeMode, params object[] parameters)
      {
        this.easeType = easeType;
        this.easeMode = easeMode;

        DoTransition(transition, from, to, time, reverse, parameters);
      }

      /// <summary>
      /// Checks if a transition effect is supported by the current hardware.
      /// </summary>
      /// <param name="transition">Transition effect</param>
      /// <returns>Is supported?</returns>
      public bool CheckTransition(CameraTransitionEffects transition)
      {
        bool isSupported = true;

        string transitionName = transition.ToString();
        string shaderPath = string.Format("Shaders/Transition{0}", transitionName);

        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader != null)
        {
          if (shader.isSupported == true)
          {
            Material material = new Material(shader);
            if (material == null)
            {
              Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] '{0}' not supported, material null.", transitionName);

              isSupported = false;
            }
            else
              DestroyImmediate(material);
#if ENABLE_LOGS
            Debug.LogFormat("[Ibuprogames.CameraTransitions] '{0}' supported.", transitionName);
#endif
          }
          else
          {
            Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] '{0}' not supported, shader not supported.", transitionName);

            isSupported = false;
          }
        }
        else
        {
          Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] '{0}' not supported, shader null or not found. Please contact to 'hello@ibuprogames.com'.", transitionName);

          isSupported = false;
        }

        return isSupported;
      }

      private bool CheckHardwareRequirements()
      {
        bool isSupported = true;

        if (SystemInfo.supportsImageEffects == false)
        {
          Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] Hardware not support Image Effects, '{0}' disabled.", this.GetType().ToString());

          isSupported = false;
        }

        return isSupported;
      }

      private void UpdateEffect(CameraTransitionEffects newEffect)
      {
        transition = newEffect;

        if (currentEffect != null)
          Destroy(currentEffect);

        switch (transition)
        {
          case CameraTransitionEffects.CrossZoom:         currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionCrossZoom>(); break;
          case CameraTransitionEffects.Cube:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionCube>(); break;
          case CameraTransitionEffects.Doom:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionDoom>(); break;
          case CameraTransitionEffects.FadeToColor:       currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionFadeToColor>(); break;
          case CameraTransitionEffects.FadeToGrayscale:   currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionFadeToGrayscale>(); break;
          case CameraTransitionEffects.Flash:             currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionFlash>(); break;
          case CameraTransitionEffects.Flip:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionFlip>(); break;
          case CameraTransitionEffects.Fold:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionFold>(); break;
          case CameraTransitionEffects.Gate:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionGate>(); break;
          case CameraTransitionEffects.Glitch:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionGlitch>(); break;
          case CameraTransitionEffects.Gradient:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionGradient>(); break;
          case CameraTransitionEffects.LinearBlur:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionLinearBlur>(); break;
          case CameraTransitionEffects.Mosaic:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionMosaic>(); break;
          case CameraTransitionEffects.PageCurl:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPageCurl>(); break;
          case CameraTransitionEffects.PageCurlAdvanced:  currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPageCurlAdvanced>(); break;
          case CameraTransitionEffects.Pixelate:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPixelate>(); break;
          case CameraTransitionEffects.Radial:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionRadial>(); break;
          case CameraTransitionEffects.RandomGrid:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionRandomGrid>(); break;
          case CameraTransitionEffects.Simple:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSimple>(); break;
          case CameraTransitionEffects.SmoothCircle:      currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSmoothCircle>(); break;
          case CameraTransitionEffects.SmoothLine:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSmoothLine>(); break;
          case CameraTransitionEffects.Swap:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSwap>(); break;
          case CameraTransitionEffects.Valentine:         currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionValentine>(); break;
          case CameraTransitionEffects.WarpWave:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionWarpWave>(); break;
        }

        if (currentEffect == null)
          Debug.LogWarningFormat("[Ibuprogames.CameraTransitions] No camera effect found in '{0}'.", fromCamera.name);
      }

      private void CreateRenderTexture()
      {
#if ENABLE_LOGS
        Debug.LogFormat("[Ibuprogames.CameraTransitions] Creating a RenderTexture {0}x{1}.",
                        Screen.width / (int)renderTextureSize,
                        Screen.height / (int)renderTextureSize);
#endif
        if (Screen.width > 0 && Screen.height > 0)
        {
          renderTexture = new RenderTexture(Screen.width / (int)renderTextureSize,
                                            Screen.height / (int)renderTextureSize,
                                            24,
                                            Application.isMobilePlatform == true ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);

          if (renderTexture != null)
          {
            renderTexture.isPowerOfTwo = false;
            renderTexture.antiAliasing = 1;
            renderTexture.name = @"RenderTexture from CameraTransition";
            if (renderTexture.Create() != true)
            {
              Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] Hardware not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

              this.enabled = false;
            }
            else
              createRenderTexture = false;
          }
          else
          {
            Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] RenderTexture null, hardware may not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

            this.enabled = false;
          }
        }
        else
        {
          Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] Wrong screen resolution '{0}x{1}', '{2}' disabled.", Screen.width, Screen.height, this.GetType().ToString());

          this.enabled = false;
        }
      }

      private IEnumerator TransitionCoroutine()
      {
        if (transitionStartEvent != null)
          transitionStartEvent(transition);

        while (currentEffect.Progress < 1.0f)
        {
          if (progressMode == ProgressModes.Automatic)
          {
            transitionLife += (useScaledTime == true ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) * timeScale;

            currentEffect.Progress = Easing.Ease(easeType, easeMode, 0.0f, 1.0f, (transitionLife / transitionTime));
          }

          if (transitionProgressEvent != null)
            transitionProgressEvent(transition, currentEffect.Progress);

          yield return null;
        }

        transitionLife = 0.0f;

        fromCamera.gameObject.SetActive(false);
        toCamera.gameObject.SetActive(true);

        FromCamera = null;
        ToCamera = null;
        currentEffect.Progress = 0.0f;
        currentEffect = null;

        isRunning = false;

        if (transitionEndEvent != null)
          transitionEndEvent(transition);
      }

      private bool IsRenderTextureSizeObsolete()
      {
        if (renderTexture == null)
          return true;

        return (Screen.width / (int)renderTextureSize != renderTexture.width) || (Screen.height / (int)renderTextureSize != renderTexture.height);
      }

      private void Awake()
      {
        this.enabled = CheckHardwareRequirements();
      }

      private void OnEnable()
      {
        if (fromCamera != null)
        {
          currentEffect = fromCamera.GetComponentInChildren<CameraTransitionBase>();
          if (currentEffect == null)
            UpdateEffect(transition);
        }

        if (renderTextureMode == RenderTextureModes.Automatic)
          CreateRenderTexture();
      }

      private void OnDisable()
      {
        if (currentEffect != null)
          Destroy(currentEffect);

        if (toCamera != null && renderTextureMode == RenderTextureModes.Automatic)
        {
#if ENABLE_LOGS
          Debug.LogFormat("[Ibuprogames.CameraTransitions] OnDisable(): '{0}' TargetTexture set to null.", toCamera.gameObject.name);
#endif
          toCamera.targetTexture = null;
        }

        if (renderTexture != null)
        {
          renderTexture.DiscardContents();

          Destroy(renderTexture);
        }
      }

      private void Update()
      {
        if (renderTextureMode == RenderTextureModes.Automatic)
        {
          if (renderTexture == null || IsRenderTextureSizeObsolete() == true || renderTexture.IsCreated() == false || createRenderTexture == true)
            CreateRenderTexture();

          if (renderTexture != null && toCamera != null && currentEffect != null && toCamera.gameObject.activeSelf == true)
          {
            toCamera.Render();

            currentEffect.RenderTexture = renderTexture;

            if (renderTextureUpdateMode == RenderTextureUpdateModes.FirstFrameOnly)
              toCamera.gameObject.SetActive(false);
          }
        }
        else if (toCamera != null && currentEffect != null && toCamera.gameObject.activeSelf == true)
        {
          if (toCamera.targetTexture != null)
          {
            toCamera.Render();

            currentEffect.RenderTexture = toCamera.targetTexture;

            if (renderTextureUpdateMode == RenderTextureUpdateModes.FirstFrameOnly)
              toCamera.gameObject.SetActive(false);
          }
          else
            Debug.LogError(@"[Ibuprogames.CameraTransitions] In manual mode, the target camera must handle its own RenderTexture.");
        }
      }      
    }
  }
}