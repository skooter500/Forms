///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Camera Transitions.
//
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Uncomment to activate logs.
//#define ENABLE_LOGS

using System;
using System.Collections;

using UnityEngine;

namespace CameraTransitions
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
            Debug.LogWarning(@"You can not change progress in automatic mode. Change to manual mode.");
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
            Debug.LogWarning(@"Cameras must be different.");
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
          Debug.LogFormat(@"ToCamera: '{0}' TargetTexture set to null.", toCamera.gameObject.name);
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
              Debug.LogFormat(@"ToCamera: '{0}' TargetTexture set to '{1}'.", toCamera.gameObject.name, renderTexture.name);
#endif
              toCamera.targetTexture = renderTexture;
            }
          }
          else
            Debug.LogWarning(@"Cameras must be different.");
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
          Debug.LogWarning(@"The duration of the effect must be greater than zero.");
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
    /// The precision of the render texture's depth buffer in bits.
    /// </summary>
    public enum RenderTextureDepths
    {
      /// <summary>
      /// 16 bits.
      /// </summary>
      _16Bits = 16,

      /// <summary>
      /// 24 bits (default).
      /// </summary>
      _24Bits = 24,
    }

    /// <summary>
    /// HDR mode.
    /// </summary>
    public enum RenderTextureHDRModes
    {
      /// <summary>
      /// Use HDR.
      /// </summary>
      Enable,

      /// <summary>
      /// Not use HDR.
      /// </summary>
      Disable,

      /// <summary>
      /// Disable on mobile platforms (default).
      /// </summary>
      Automatic,
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
            Debug.LogFormat("RenderTextureMode changed to '{0}'", renderTextureMode.ToString());
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
              Debug.LogError("A transition is active, try again when finished.");
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
            Debug.LogError(@"A transition is active, try again when finished.");
        }
      }
    }

    /// <summary>
    /// RenderTexture depth.
    /// </summary>
    public RenderTextureDepths RenderTextureDepth
    {
      get { return renderTextureDepth; }
      set { renderTextureDepth = value; }
    }

    /// <summary>
    /// RenderTexture depth.
    /// </summary>
    public RenderTextureHDRModes RenderTextureHDR
    {
      get { return renderTextureHDR; }
      set { renderTextureHDR = value; }
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
            Debug.LogError(@"A transition is active, try again when finished.");
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

    public void ResetAdvancedOptions()
    {
      // Behaviors
      progressMode = ProgressModes.Automatic;

      // RenderTexture.
      renderTextureMode = RenderTextureModes.Automatic;
      renderTextureUpdateMode = RenderTextureUpdateModes.AllFrames;
      renderTextureSize = RenderTextureSizes.SameAsScreen;
      renderTextureDepth = RenderTextureDepths._24Bits;
      renderTextureHDR = RenderTextureHDRModes.Automatic;
      invertRenderTexture = false;
    }

    [SerializeField]
    private ProgressModes progressMode = ProgressModes.Automatic;

    [SerializeField]
    private RenderTextureModes renderTextureMode = RenderTextureModes.Automatic;

    [SerializeField]
    private RenderTextureUpdateModes renderTextureUpdateMode = RenderTextureUpdateModes.AllFrames;

    [SerializeField]
    private RenderTextureSizes renderTextureSize = RenderTextureSizes.SameAsScreen;

    [SerializeField]
    private RenderTextureDepths renderTextureDepth = RenderTextureDepths._24Bits;

    [SerializeField]
    private RenderTextureHDRModes renderTextureHDR = RenderTextureHDRModes.Automatic;

    [SerializeField]
    private bool invertRenderTexture = false;
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
    public void DoTransition(CameraTransitionEffects transition, Camera from, Camera to, float time, params object[] parameters)
    {
      if (from != null && to != null)
      {
        if (from != to)
        {
          if (time > 0.0f)
          {
            isRunning = true;

            CameraTransitionEffects oldTransition = Transition;

            Transition = transition;
            FromCamera = from;
            ToCamera = to;
            transitionTime = time;
            transitionLife = 0.0f;

            //from.gameObject.SetActive(false);
            to.gameObject.SetActive(true);
            from.gameObject.SetActive(true);

            currentEffect.InvertRenderTexture = invertRenderTexture;

#if ENABLE_LOGS
            Debug.LogFormat("Camera transition '{0}' from '{1}' to '{2}' using '{3}' in {4} seconds.", transition.ToString(), from.gameObject.name, to.gameObject.name, easeType.ToString(), time);
#endif
            if (parameters != null && parameters.Length > 0)
              SetParametersToCurrentEffect(parameters);

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
            Debug.LogWarning(@"Duration must be greater than zero.");
        }
        else
          Debug.LogWarning(@"Cameras must be differents.");
      }
      else
        Debug.LogWarning(@"Invalid cameras.");
    }

    /// <summary>
    /// Same as previus funtion but with easing in/out equations.
    /// </summary>
    public void DoTransition(CameraTransitionEffects transition, Camera from, Camera to, float time, EaseType easeType, EaseMode easeMode, params object[] parameters)
    {
      this.easeType = easeType;
      this.easeMode = easeMode;

      DoTransition(transition, from, to, time, parameters);
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
            Debug.LogErrorFormat("'{0}' not supported, material null.", transitionName);

            isSupported = false;
          }
          else
            DestroyImmediate(material);
#if ENABLE_LOGS
          Debug.LogFormat("'{0}' supported.", transitionName);
#endif
        }
        else
        {
          Debug.LogErrorFormat("'{0}' not supported, shader not supported.", transitionName);

          isSupported = false;
        }
      }
      else
      {
        Debug.LogErrorFormat("'{0}' not supported, shader null or not found. Please contact to 'hello@ibuprogames.com'.", transitionName);

        isSupported = false;
      }

      return isSupported;
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
        Debug.LogFormat(@"OnDisable: '{0}' TargetTexture set to null.", toCamera.gameObject.name);
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
          Debug.LogError(@"In manual mode, the target camera must handle its own RenderTexture.");
      }
    }

    private bool CheckHardwareRequirements()
    {
      bool isSupported = true;

      if (SystemInfo.supportsImageEffects == false)
      {
        Debug.LogErrorFormat("Hardware not support Image Effects, '{0}' disabled.", this.GetType().ToString());

        isSupported = false;
      }

      if (SystemInfo.supportsRenderTextures == false)
      {
        Debug.LogErrorFormat("Hardware not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

        isSupported = false;
      }

      return isSupported;
    }

    private void SetParametersToCurrentEffect(object[] parameters)
    {
      switch (Transition)
      {
        case CameraTransitionEffects.CrossZoom:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(float)
                                     && parameters[0].GetType() == typeof(float))
          {
            CameraTransitionCrossZoom crossZoom = currentEffect as CameraTransitionCrossZoom;
            crossZoom.Strength = (float)parameters[0];
            crossZoom.Quality = (float)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: strength (float), quality (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Cube:
          if (parameters.Length == 4 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float) &&
                                        parameters[3].GetType() == typeof(float))
          {
            CameraTransitionCube cube = currentEffect as CameraTransitionCube;
            cube.Perspective = (float)parameters[0];
            cube.Zoom = (float)parameters[1];
            cube.Reflection = (float)parameters[2];
            cube.Elevation = (float)parameters[3];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: perspective (float), zoom (float), reflection (float), elevation (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Doom:
          if (parameters.Length == 4 && parameters[0].GetType() == typeof(int) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float) &&
                                        parameters[3].GetType() == typeof(float))
          {
            CameraTransitionDoom doom = currentEffect as CameraTransitionDoom;
            doom.BarWidth = (int)parameters[0];
            doom.Amplitude = (float)parameters[1];
            doom.Noise = (float)parameters[2];
            doom.Frequency = (float)parameters[3];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: bar width (int), amplitude (float), noise (float), frequency (float).", transition.ToString());
          break;

        case CameraTransitionEffects.FadeToColor:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(Color))
          {
            CameraTransitionFadeToColor fadeToColor = currentEffect as CameraTransitionFadeToColor;
            fadeToColor.Strength = (float)parameters[0];
            fadeToColor.Color = (Color)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: strength (float), color (Color).", transition.ToString());
          break;

        case CameraTransitionEffects.FadeToGrayscale:
          if (parameters.Length == 1 && parameters[0].GetType() == typeof(float))
          {
            CameraTransitionFadeToGrayscale fadeToGrayscale = currentEffect as CameraTransitionFadeToGrayscale;
            fadeToGrayscale.Strength = (float)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: strength (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Flash:
          if (parameters.Length == 5 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float) &&
                                        parameters[3].GetType() == typeof(float) &&
                                        parameters[4].GetType() == typeof(Color))
          {
            CameraTransitionFlash flash = currentEffect as CameraTransitionFlash;
            flash.Strength = (float)parameters[0];
            flash.Intensity = (float)parameters[1];
            flash.Zoom = (float)parameters[2];
            flash.Velocity = (float)parameters[3];
            flash.Color = (Color)parameters[4];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: strength (float), intensity (float), zoom (float), velocity (float), color (Color)", transition.ToString());
          break;

        case CameraTransitionEffects.Flip:
          if (parameters.Length == 1 && parameters[0].GetType() == typeof(CameraTransitionFlip.Modes))
          {
            CameraTransitionFlip flip = currentEffect as CameraTransitionFlip;
            flip.Mode = (CameraTransitionFlip.Modes)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: mode (CameraTransitionFlip.Modes).", transition.ToString());
          break;

        case CameraTransitionEffects.Fold:
          if (parameters.Length == 1 && parameters[0].GetType() == typeof(CameraTransitionFold.Modes))
          {
            CameraTransitionFold fold = currentEffect as CameraTransitionFold;
            fold.Mode = (CameraTransitionFold.Modes)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: mode (CameraTransitionFlip.Modes).", transition.ToString());
          break;

        case CameraTransitionEffects.Gate:
          if (parameters.Length == 3 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float))
          {
            CameraTransitionGate swap = currentEffect as CameraTransitionGate;
            swap.Perspective = (float)parameters[0];
            swap.Depth = (float)parameters[1];
            swap.Reflection = (float)parameters[2];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: perspective (float), depth (float), reflection (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Glitch:
          if (parameters.Length == 1)
          {
            CameraTransitionGlitch glitch = currentEffect as CameraTransitionGlitch;
            glitch.Strength = (float)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' does not require any parameters.", transition.ToString());
          break;

        case CameraTransitionEffects.LinearBlur:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(int))
          {
            CameraTransitionLinearBlur linearBlur = currentEffect as CameraTransitionLinearBlur;
            linearBlur.Intensity = (float)parameters[0];
            linearBlur.Passes = (int)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: intensity (float), passes (int).", transition.ToString());
          break;

        case CameraTransitionEffects.Mosaic:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(Vector2) &&
                                        parameters[1].GetType() == typeof(bool))
          {
            CameraTransitionMosaic mosaic = currentEffect as CameraTransitionMosaic;
            mosaic.Steps = (Vector2)parameters[0];
            mosaic.Rotate = (bool)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: steps (Vector2), rotate (bool).", transition.ToString());
          break;

        case CameraTransitionEffects.PageCurl:
          if (parameters.Length == 3 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(bool))
          {
            CameraTransitionPageCurl pageCurl = currentEffect as CameraTransitionPageCurl;
            pageCurl.Angle = (float)parameters[0];
            pageCurl.Radius = (float)parameters[1];
            pageCurl.Shadows = (bool)parameters[2];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: angle (float), radius (float), shadows (bool).", transition.ToString());
          break;

        case CameraTransitionEffects.PageCurlAdvanced:
          if (parameters.Length == 6 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float) &&
                                        parameters[3].GetType() == typeof(bool) &&
                                        parameters[4].GetType() == typeof(bool) &&
                                        parameters[5].GetType() == typeof(bool))
          {
            CameraTransitionPageCurlAdvanced pageCurlAdvanced = currentEffect as CameraTransitionPageCurlAdvanced;
            pageCurlAdvanced.Angle = (float)parameters[0];
            pageCurlAdvanced.Radius = (float)parameters[1];
            pageCurlAdvanced.BackTransparency = (float)parameters[2];
            pageCurlAdvanced.FrontShadow = (bool)parameters[3];
            pageCurlAdvanced.BackShadow = (bool)parameters[4];
            pageCurlAdvanced.InnerShadow = (bool)parameters[5];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: angle (float), radius (float), back transparency (float), front shadow (bool), back shadow (bool), inner shadow (bool).", transition.ToString());
          break;

        case CameraTransitionEffects.Pixelate:
          if (parameters.Length == 1 && parameters[0].GetType() == typeof(float))
          {
            CameraTransitionPixelate pixelate = currentEffect as CameraTransitionPixelate;
            pixelate.Size = (float)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: size (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Radial:
          if (parameters.Length == 1 && parameters[0].GetType() == typeof(bool))
          {
            CameraTransitionRadial radial = currentEffect as CameraTransitionRadial;
            radial.Clockwise = (bool)parameters[0];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: clockwise (bool).", transition.ToString());
          break;

        case CameraTransitionEffects.RandomGrid:
          if (parameters.Length == 3 && parameters[0].GetType() == typeof(int) &&
                                        parameters[1].GetType() == typeof(int) &&
                                        parameters[2].GetType() == typeof(float))
          {
            CameraTransitionRandomGrid randomGrid = currentEffect as CameraTransitionRandomGrid;
            randomGrid.Rows = (int)parameters[0];
            randomGrid.Columns = (int)parameters[1];
            randomGrid.Smoothness = (float)parameters[2];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: rows (int), columns (int), smoothness (float).", transition.ToString());
          break;

        case CameraTransitionEffects.SmoothCircle:
          if (parameters.Length == 3 && parameters[0].GetType() == typeof(bool) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(Vector2))
          {
            CameraTransitionSmoothCircle smoothCircle = currentEffect as CameraTransitionSmoothCircle;
            smoothCircle.Invert = (bool)parameters[0];
            smoothCircle.Smoothness = (float)parameters[1];
            smoothCircle.Center = (Vector2)parameters[2];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: invert (bool), smoothness (float), center (Vector2).", transition.ToString());
          break;

        case CameraTransitionEffects.SmoothLine:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float))
          {
            CameraTransitionSmoothLine smoothLine = currentEffect as CameraTransitionSmoothLine;
            smoothLine.Angle = (float)parameters[0];
            smoothLine.Smoothness = (float)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: angle (float), smoothness (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Swap:
          if (parameters.Length == 3 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(float) &&
                                        parameters[2].GetType() == typeof(float))
          {
            CameraTransitionSwap swap = currentEffect as CameraTransitionSwap;
            swap.Perspective = (float)parameters[0];
            swap.Depth = (float)parameters[1];
            swap.Reflection = (float)parameters[2];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: perspective (float), depth (float), reflection (float).", transition.ToString());
          break;

        case CameraTransitionEffects.Valentine:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                        parameters[1].GetType() == typeof(Color))
          {
            CameraTransitionValentine valentine = currentEffect as CameraTransitionValentine;
            valentine.Border = (float)parameters[0];
            valentine.Color = (Color)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: border width (float), border color (Color).", transition.ToString());
          break;

        case CameraTransitionEffects.WarpWave:
          if (parameters.Length == 2 && parameters[0].GetType() == typeof(CameraTransitionWarpWave.Modes) &&
                                        parameters[1].GetType() == typeof(float))
          {
            CameraTransitionWarpWave warpWave = currentEffect as CameraTransitionWarpWave;
            warpWave.Mode = (CameraTransitionWarpWave.Modes)parameters[0];
            warpWave.Curvature = (float)parameters[1];
          }
          else
            Debug.LogWarningFormat("Effect '{0}' required parameters: mode (CameraTransitionWarpWave.Modes), curvature (float).", transition.ToString());
          break;
      }
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
        case CameraTransitionEffects.LinearBlur:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionLinearBlur>(); break;
        case CameraTransitionEffects.Mosaic:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionMosaic>(); break;
        case CameraTransitionEffects.PageCurl:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPageCurl>(); break;
        case CameraTransitionEffects.PageCurlAdvanced:  currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPageCurlAdvanced>(); break;
        case CameraTransitionEffects.Pixelate:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionPixelate>(); break;
        case CameraTransitionEffects.Radial:            currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionRadial>(); break;
        case CameraTransitionEffects.RandomGrid:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionRandomGrid>(); break;
        case CameraTransitionEffects.SmoothCircle:      currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSmoothCircle>(); break;
        case CameraTransitionEffects.SmoothLine:        currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSmoothLine>(); break;
        case CameraTransitionEffects.Swap:              currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionSwap>(); break;
        case CameraTransitionEffects.Valentine:         currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionValentine>(); break;
        case CameraTransitionEffects.WarpWave:          currentEffect = fromCamera.gameObject.AddComponent<CameraTransitionWarpWave>(); break;
      }

      if (currentEffect == null)
        Debug.LogWarningFormat("No camera effect found in '{0}'", fromCamera.name);
    }

    private void CreateRenderTexture()
    {
#if ENABLE_LOGS
      Debug.LogFormat("Creating a RenderTexture {0}x{1}.",
                      Screen.width / (int)renderTextureSize,
                      Screen.height / (int)renderTextureSize);
#endif
      if (Screen.width > 0 && Screen.height > 0)
      {
        RenderTextureFormat renderTextureFormat = (renderTextureHDR == RenderTextureHDRModes.Disable ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);
        if (renderTextureHDR == RenderTextureHDRModes.Automatic)
          renderTextureFormat = (Application.isMobilePlatform == true ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);

        renderTexture = new RenderTexture(Screen.width / (int)renderTextureSize,
                                          Screen.height / (int)renderTextureSize,
                                          (int)renderTextureDepth,
                                          renderTextureFormat);

        if (renderTexture != null)
        {
          renderTexture.isPowerOfTwo = false;
          renderTexture.antiAliasing = 1;
          renderTexture.anisoLevel = 0;
          renderTexture.name = @"RenderTexture from CameraTransition";

          if (renderTexture.Create() != true)
          {
            Debug.LogErrorFormat("Hardware not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

            this.enabled = false;
          }
          else
            createRenderTexture = false;
        }
        else
        {
          Debug.LogErrorFormat("RenderTexture null, hardware may not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

          this.enabled = false;
        }
      }
      else
      {
        Debug.LogErrorFormat("Wrong screen resolution '{0}x{1}', '{2}' disabled.", Screen.width, Screen.height, this.GetType().ToString());

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
          transitionLife += UnityEngine.Time.deltaTime;

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
  }
}