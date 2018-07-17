///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

using UnityEngine;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// Camera transitions assistant.
    /// </summary>
    [AddComponentMenu("Camera Transitions/Camera Transitions Assistant")]
    public sealed class CameraTransitionsAssistant : MonoBehaviour
    {
      #region Transition Effects Parameters.
      interface CameraTransitionParams
      {
        object[] ToParams();
      }

      [Serializable]
      public class FadeToColorParams : CameraTransitionParams
      {
        public float strength = 0.3f;
        public Color color = Color.black;

        public object[] ToParams() { return new object[] { strength, color }; }
      }

      [Serializable]
      public class FadeToGrayscaleParams : CameraTransitionParams
      {
        public float strength = 0.3f;

        public object[] ToParams() { return new object[] { strength }; }
      }

      [Serializable]
      public class FlashParams : CameraTransitionParams
      {
        public float strength = 0.3f;
        public float intensity = 3.0f;
        public float zoom = 0.5f;
        public float velocity = 3.0f;
        public Color color = new Color(1.0f, 0.8f, 0.3f);

        public object[] ToParams() { return new object[] { strength, intensity, zoom, velocity, color }; }
      }

      [Serializable]
      public class FoldParams : CameraTransitionParams
      {
        public CameraTransitionFold.Modes mode = CameraTransitionFold.Modes.Vertical;

        public object[] ToParams() { return new object[] { mode }; }
      }

      [Serializable]
      public class FlipParams : CameraTransitionParams
      {
        public CameraTransitionFlip.Modes mode = CameraTransitionFlip.Modes.Vertical;

        public object[] ToParams() { return new object[] { mode }; }
      }

      [Serializable]
      public class NoParams : CameraTransitionParams
      {
        // No params... :(
        public object[] ToParams() { return null; }
      }

      [Serializable]
      public class GateParams : CameraTransitionParams
      {
        public float perspective = 0.4f;
        public float depth = 3.0f;
        public float reflection = 0.4f;

        public object[] ToParams() { return new object[] { perspective, depth, reflection }; }
      }

      [Serializable]
      public class GlitchParams : CameraTransitionParams
      {
        public float strength = 0.4f;

        public object[] ToParams() { return new object[] { strength }; }
      }

      [Serializable]
      public class GradientParams : CameraTransitionParams
      {
        public Texture texture;

        public object[] ToParams() { return new object[] { texture }; }
      }

      [Serializable]
      public class LinearBlurParams : CameraTransitionParams
      {
        public float intensity = 0.1f;
        public int passes = 8;

        public object[] ToParams() { return new object[] { intensity, passes }; }
      }

      [Serializable]
      public class PageCurlParams : CameraTransitionParams
      {
        public float angle = 45.0f;
        public float radius = 0.1f;
        public bool shadows = true;

        public object[] ToParams() { return new object[] { angle, radius, shadows }; }
      }

      [Serializable]
      public class PageCurlAdvancedParams : CameraTransitionParams
      {
        public float angle = 45.0f;
        public float radius = 0.1f;
        public float backTransparency = 0.25f;
        public bool frontShadow = true;
        public bool backShadow = true;
        public bool innerShadow = true;

        public object[] ToParams() { return new object[] { angle, radius, backTransparency, frontShadow, backShadow, innerShadow }; }
      }

      [Serializable]
      public class PixelateParams : CameraTransitionParams
      {
        public float size = 50.0f;

        public object[] ToParams() { return new object[] { size }; }
      }

      [Serializable]
      public class RadialParams : CameraTransitionParams
      {
        public bool clockWise = true;

        public object[] ToParams() { return new object[] { clockWise }; }
      }

      [Serializable]
      public class RandomGridParams : CameraTransitionParams
      {
        public int rows = 10;
        public int columns = 10;
        public float smoothness = 0.5f;

        public object[] ToParams() { return new object[] { rows, columns, smoothness }; }
      }

      [Serializable]
      public class SmoothCircleParams : CameraTransitionParams
      {
        public bool invert = false;
        public float smoothness = 0.3f;
        public Vector2 center = Vector2.one * 0.5f;

        public object[] ToParams() { return new object[] { invert, smoothness, center }; }
      }

      [Serializable]
      public class SmoothLineParams : CameraTransitionParams
      {
        public float angle = 45.0f;
        public float smoothness = 0.5f;

        public object[] ToParams() { return new object[] { angle, smoothness }; }
      }

      [Serializable]
      public class ValentineParams : CameraTransitionParams
      {
        public float border = 25.0f;
        public Color color = Color.red;

        public object[] ToParams() { return new object[] { border, color }; }
      }

      [Serializable]
      public class WarpWaveParams : CameraTransitionParams
      {
        public CameraTransitionWarpWave.Modes mode = CameraTransitionWarpWave.Modes.Horizontal;
        public float curvature = 0.5f;

        public object[] ToParams() { return new object[] { mode, curvature }; }
      }

      [Serializable]
      public class CrossZoomParams : CameraTransitionParams
      {
        public float strength = 0.4f;
        public float quality = 20.0f;

        public object[] ToParams() { return new object[] { strength, quality }; }
      }

      [Serializable]
      public class CubeParams : CameraTransitionParams
      {
        public float perspective = 0.7f;
        public float zoom = 0.3f;
        public float reflection = 0.4f;
        public float elevantion = 3.0f;

        public object[] ToParams() { return new object[] { perspective, zoom, reflection, elevantion }; }
      }

      [Serializable]
      public class DoomParams : CameraTransitionParams
      {
        public int barWidth = 10;
        public float amplitude = 2.0f;
        public float noise = 0.1f;
        public float frequency = 1.0f;

        public object[] ToParams() { return new object[] { barWidth, amplitude, noise, frequency }; }
      }

      [Serializable]
      public class MosaicParams : CameraTransitionParams
      {
        public Vector2 steps = Vector2.one;
        public bool rotate = true;

        public object[] ToParams() { return new object[] { steps, rotate }; }
      }

      [Serializable]
      public class SwapParams : CameraTransitionParams
      {
        public float perspective = 0.2f;
        public float depth = 3.0f;
        public float reflection = 0.4f;

        public object[] ToParams() { return new object[] { perspective, depth, reflection }; }
      }

      public bool reverse = false;

      public FadeToColorParams fadeToColorParams = new FadeToColorParams();

      public FadeToGrayscaleParams fadeToGrayscaleParams = new FadeToGrayscaleParams();

      public FlashParams flashParams = new FlashParams();

      public FoldParams foldParams = new FoldParams();

      public FlipParams flipParams = new FlipParams();

      public NoParams noParams = new NoParams();

      public GateParams gateParams = new GateParams();

      public GlitchParams glitchParams = new GlitchParams();

      public GradientParams gradientParams = new GradientParams();

      public LinearBlurParams linearBlurParams = new LinearBlurParams();

      public PageCurlParams pageCurlParams = new PageCurlParams();

      public PageCurlAdvancedParams pageCurlAdvancedParams = new PageCurlAdvancedParams();

      public PixelateParams pixelateParams = new PixelateParams();

      public RadialParams radialParams = new RadialParams();

      public RandomGridParams randomGridParams = new RandomGridParams();

      public SmoothCircleParams smoothCircleParams = new SmoothCircleParams();

      public SmoothLineParams smoothLineParams = new SmoothLineParams();

      public ValentineParams valentineParams = new ValentineParams();

      public WarpWaveParams warpWaveParams = new WarpWaveParams();

      public CrossZoomParams crossZoomParams = new CrossZoomParams();

      public CubeParams cubeParams = new CubeParams();

      public DoomParams doomParams = new DoomParams();

      public MosaicParams mosaicParams = new MosaicParams();

      public SwapParams swapParams = new SwapParams();
      #endregion
      
      /// <summary>
      /// Execution methods.
      /// </summary>
      public enum ExecutionMethod
      {
        /// <summary>
        /// You must call ExecuteTransition() manually.
        /// </summary>
        Manual,

        /// <summary>
        /// It is executed on Awake() the GameObject.
        /// You can set a delay time with delayTime.
        /// </summary>
        OnEnable,
      }

      /// <summary>
      /// Execution method.
      /// </summary>
      public ExecutionMethod executeMethod = ExecutionMethod.Manual;

      /// <summary>
      /// Delay time in Awake mode (>0).
      /// </summary>
      public float delayTime;

      /// <summary>
      /// Transition effect.
      /// </summary>
      public CameraTransitionEffects transitionEffect;

      /// <summary>
      /// Source camera.
      /// </summary>
      public Camera cameraA;

      /// <summary>
      /// Destiny camera.
      /// </summary>
      public Camera cameraB;

      /// <summary>
      /// Easing equation.
      /// </summary>
      public EaseType easeType = EaseType.Linear;

      /// <summary>
      /// Easing mode.
      /// </summary>
      public EaseMode easeMode;

      /// <summary>
      /// Transition time (>0).
      /// </summary>
      public float transitionTime = 1.0f;

      /// <summary>
      /// CameraTransition object.
      /// </summary>
      public CameraTransition cameraTransition;

      /// <summary>
      /// Is running some transition?
      /// </summary>
      public bool IsRunning
      {
        get { return cameraTransition != null ? cameraTransition.IsRunning : false; }
      }

      private float timeToExecute = -1.0f;
      
      /// <summary>
      /// Execute transition.
      /// </summary>
      public void ExecuteTransition()
      {
        if (cameraTransition != null)
        {
          timeToExecute = -1.0f;

          object[] parameters = null;
          switch (transitionEffect)
          {
            case CameraTransitionEffects.CrossZoom:         parameters = crossZoomParams.ToParams(); break;
            case CameraTransitionEffects.Cube:              parameters = cubeParams.ToParams(); break;
            case CameraTransitionEffects.Doom:              parameters = doomParams.ToParams(); break;
            case CameraTransitionEffects.FadeToColor:       parameters = fadeToColorParams.ToParams(); break;
            case CameraTransitionEffects.FadeToGrayscale:   parameters = fadeToGrayscaleParams.ToParams(); break;
            case CameraTransitionEffects.Flash:             parameters = flashParams.ToParams(); break;
            case CameraTransitionEffects.Flip:              parameters = flipParams.ToParams(); break;
            case CameraTransitionEffects.Fold:              parameters = foldParams.ToParams(); break;
            case CameraTransitionEffects.Gate:              parameters = gateParams.ToParams(); break;
            case CameraTransitionEffects.Glitch:            parameters = glitchParams.ToParams(); break;
            case CameraTransitionEffects.Gradient:          parameters = gradientParams.ToParams(); break;
            case CameraTransitionEffects.LinearBlur:        parameters = linearBlurParams.ToParams(); break;
            case CameraTransitionEffects.Mosaic:            parameters = mosaicParams.ToParams(); break;
            case CameraTransitionEffects.PageCurl:          parameters = pageCurlParams.ToParams(); break;
            case CameraTransitionEffects.PageCurlAdvanced:  parameters = pageCurlAdvancedParams.ToParams(); break;
            case CameraTransitionEffects.Pixelate:          parameters = pixelateParams.ToParams(); break;
            case CameraTransitionEffects.Radial:            parameters = radialParams.ToParams(); break;
            case CameraTransitionEffects.RandomGrid:        parameters = randomGridParams.ToParams(); break;
            case CameraTransitionEffects.SmoothCircle:      parameters = smoothCircleParams.ToParams(); break;
            case CameraTransitionEffects.SmoothLine:        parameters = smoothLineParams.ToParams(); break;
            case CameraTransitionEffects.Swap:              parameters = swapParams.ToParams(); break;
            case CameraTransitionEffects.Valentine:         parameters = valentineParams.ToParams(); break;
            case CameraTransitionEffects.WarpWave:          parameters = warpWaveParams.ToParams(); break;
          }

          cameraTransition.DoTransition(transitionEffect, cameraA, cameraB, transitionTime, reverse, easeType, easeMode, parameters);
        }
        else
          Debug.LogWarning(@"CameraTransition component not found.");
      }

      private void OnEnable()
      {
        if (executeMethod == ExecutionMethod.OnEnable)
        {
          if (delayTime <= 0.0f)
            ExecuteTransition();
          else
            timeToExecute = delayTime;
        }
      }

      private void Update()
      {
        if (executeMethod == ExecutionMethod.OnEnable && timeToExecute >= 0.0f)
        {
          timeToExecute -= Time.deltaTime;
          if (timeToExecute <= 0)
            ExecuteTransition();
        }
      }
    }
  }
}