///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// CameraTransitionsAssistant Editor.
    /// </summary>
    [CustomEditor(typeof(CameraTransitionsAssistant))]
    public class CameraTransitionsAssistantEditor : Editor
    {
      private CameraTransitionsAssistant baseTarget;

      /// <summary>
      /// OnInspectorGUI.
      /// </summary>
      public override void OnInspectorGUI()
      {
        if (baseTarget == null)
          baseTarget = this.target as CameraTransitionsAssistant;

        EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0.0f;

        EditorGUI.indentLevel = 0;

        EditorGUIUtility.labelWidth = 125.0f;

        EditorGUILayout.BeginVertical();
        {
          EditorGUILayout.Separator();

          baseTarget.transitionEffect = (CameraTransitionEffects)EditorGUILayout.EnumPopup(@"Transition", baseTarget.transitionEffect);

          baseTarget.reverse = EditorGUILayout.Toggle(@"Reverse", baseTarget.reverse);

          // Custom parameters.

          EditorGUI.indentLevel++;

          switch (baseTarget.transitionEffect)
          {
            case CameraTransitionEffects.CrossZoom:
              baseTarget.crossZoomParams.strength = EditorGUILayout.Slider("Strength", baseTarget.crossZoomParams.strength, 0.0f, 1.0f);
              baseTarget.crossZoomParams.quality = EditorGUILayout.Slider("Quality", Mathf.Abs(baseTarget.crossZoomParams.quality), 1.0f, 40.0f);
              break;

            case CameraTransitionEffects.Cube:
              baseTarget.cubeParams.perspective = EditorGUILayout.Slider("Perspective", baseTarget.cubeParams.perspective, 0.0f, 1.0f);
              baseTarget.cubeParams.zoom = EditorGUILayout.Slider("Zoom", baseTarget.cubeParams.zoom, 0.0f, 100.0f);
              baseTarget.cubeParams.reflection = EditorGUILayout.Slider("Reflection", baseTarget.cubeParams.reflection, 0.0f, 1.0f);
              baseTarget.cubeParams.elevantion = EditorGUILayout.Slider("Elevantion", baseTarget.cubeParams.elevantion, 0.0f, 100.0f);
              break;

            case CameraTransitionEffects.Doom:
              baseTarget.doomParams.barWidth = EditorGUILayout.IntSlider("Bar width", baseTarget.doomParams.barWidth, 0, 1024);
              baseTarget.doomParams.amplitude = EditorGUILayout.Slider("Amplitude", baseTarget.doomParams.amplitude, 0.0f, 25.0f);
              baseTarget.doomParams.noise = EditorGUILayout.Slider("Noise", baseTarget.doomParams.noise, 0.0f, 1.0f);
              baseTarget.doomParams.frequency = EditorGUILayout.Slider("Frequency", baseTarget.doomParams.frequency, 0.0f, 100.0f);
              break;

            case CameraTransitionEffects.FadeToColor:
              baseTarget.fadeToColorParams.strength = EditorGUILayout.Slider("Strength", baseTarget.fadeToColorParams.strength, 0.0f, 1.0f);
              baseTarget.fadeToColorParams.color = EditorGUILayout.ColorField("Color", baseTarget.fadeToColorParams.color);
              break;

            case CameraTransitionEffects.FadeToGrayscale:
              baseTarget.fadeToGrayscaleParams.strength = EditorGUILayout.Slider("Strength", baseTarget.fadeToGrayscaleParams.strength, 0.0f, 1.0f);
              break;

            case CameraTransitionEffects.Flash:
              baseTarget.flashParams.strength = EditorGUILayout.Slider("Strength", baseTarget.flashParams.strength, 0.0f, 1.0f);
              baseTarget.flashParams.intensity = EditorGUILayout.Slider("Intensity", baseTarget.flashParams.intensity, 0.0f, 5.0f);
              baseTarget.flashParams.zoom = EditorGUILayout.Slider("Zoom", baseTarget.flashParams.zoom, 0.0f, 1.0f);
              baseTarget.flashParams.velocity = EditorGUILayout.Slider("Velocity", baseTarget.flashParams.velocity, 0.1f, 10.0f);
              baseTarget.flashParams.color = EditorGUILayout.ColorField("Color", baseTarget.flashParams.color);
              break;

            case CameraTransitionEffects.Flip:
              baseTarget.flipParams.mode = (CameraTransitionFlip.Modes)EditorGUILayout.EnumPopup("Mode", baseTarget.flipParams.mode);
              break;

            case CameraTransitionEffects.Fold:
              baseTarget.foldParams.mode = (CameraTransitionFold.Modes)EditorGUILayout.EnumPopup("Mode", baseTarget.foldParams.mode);
              break;

            case CameraTransitionEffects.Gate:
              baseTarget.gateParams.perspective = EditorGUILayout.Slider("Perspective", baseTarget.gateParams.perspective, 0.0f, 1.0f);
              baseTarget.gateParams.depth = EditorGUILayout.Slider("Depth", baseTarget.gateParams.depth, 0.0f, 100.0f);
              baseTarget.gateParams.reflection = EditorGUILayout.Slider("Reflection", baseTarget.gateParams.reflection, 0.0f, 1.0f);
              break;

            case CameraTransitionEffects.Glitch:
              baseTarget.glitchParams.strength = EditorGUILayout.Slider("Strength", baseTarget.glitchParams.strength, 0.0f, 10.0f);
              break;

            case CameraTransitionEffects.Gradient:
              baseTarget.gradientParams.texture = EditorGUILayout.ObjectField("Gradient", baseTarget.gradientParams.texture, typeof(Texture), false) as Texture;
              break;

            case CameraTransitionEffects.LinearBlur:
              baseTarget.linearBlurParams.intensity = EditorGUILayout.Slider("Intensity", baseTarget.linearBlurParams.intensity, 0.0f, 1.0f);
              baseTarget.linearBlurParams.passes = EditorGUILayout.IntSlider("Passes", baseTarget.linearBlurParams.passes, 1, 8);
              break;

            case CameraTransitionEffects.Mosaic:
              EditorGUILayout.LabelField("Steps");
              EditorGUI.indentLevel++;
              baseTarget.mosaicParams.steps.x = EditorGUILayout.IntSlider("Horizontal", (int)baseTarget.mosaicParams.steps.x, -10, 10);
              baseTarget.mosaicParams.steps.y = EditorGUILayout.IntSlider("Vertical", (int)baseTarget.mosaicParams.steps.y, -10, 10);
              EditorGUI.indentLevel--;
              baseTarget.mosaicParams.rotate = EditorGUILayout.Toggle("Rotation", baseTarget.mosaicParams.rotate);
              break;

            case CameraTransitionEffects.PageCurl:
              baseTarget.pageCurlParams.angle = EditorGUILayout.Slider("Angle", baseTarget.pageCurlParams.angle, 0.0f, 180.0f);
              baseTarget.pageCurlParams.radius = EditorGUILayout.Slider("Radius", baseTarget.pageCurlParams.radius, 0.0f, 1.0f);
              baseTarget.pageCurlParams.shadows = EditorGUILayout.Toggle("Shadows", baseTarget.pageCurlParams.shadows);
              break;

            case CameraTransitionEffects.PageCurlAdvanced:
              baseTarget.pageCurlAdvancedParams.angle = EditorGUILayout.Slider("Angle", baseTarget.pageCurlAdvancedParams.angle, 0.0f, 180.0f);
              baseTarget.pageCurlAdvancedParams.radius = EditorGUILayout.Slider("Radius", baseTarget.pageCurlAdvancedParams.radius, 0.0f, 1.0f);
              baseTarget.pageCurlAdvancedParams.backTransparency = EditorGUILayout.Slider("Back page trans.", baseTarget.pageCurlAdvancedParams.backTransparency, 0.0f, 1.0f);
              EditorGUILayout.LabelField("Shadows");
              EditorGUI.indentLevel++;
              baseTarget.pageCurlAdvancedParams.frontShadow = EditorGUILayout.Toggle("Front", baseTarget.pageCurlAdvancedParams.frontShadow);
              baseTarget.pageCurlAdvancedParams.backShadow = EditorGUILayout.Toggle("Back", baseTarget.pageCurlAdvancedParams.backShadow);
              baseTarget.pageCurlAdvancedParams.innerShadow = EditorGUILayout.Toggle("Inner", baseTarget.pageCurlAdvancedParams.innerShadow);
              EditorGUI.indentLevel--;
              break;

            case CameraTransitionEffects.Pixelate:
              baseTarget.pixelateParams.size = EditorGUILayout.Slider("Size", baseTarget.pixelateParams.size, 0.0f, 1000.0f);
              break;

            case CameraTransitionEffects.Radial:
              baseTarget.radialParams.clockWise = EditorGUILayout.Toggle("Clockwise", baseTarget.radialParams.clockWise);
              break;

            case CameraTransitionEffects.RandomGrid:
              baseTarget.randomGridParams.rows = EditorGUILayout.IntSlider("Rows", baseTarget.randomGridParams.rows, 0, 100);
              baseTarget.randomGridParams.columns = EditorGUILayout.IntSlider("Columns", baseTarget.randomGridParams.columns, 0, 100);
              baseTarget.randomGridParams.smoothness = EditorGUILayout.Slider("Smoothness", baseTarget.randomGridParams.smoothness, 0.0f, 1.0f);
              break;

            case CameraTransitionEffects.Simple:
              break;

            case CameraTransitionEffects.SmoothCircle:
              baseTarget.smoothCircleParams.smoothness = EditorGUILayout.Slider("Smoothness", baseTarget.smoothCircleParams.smoothness, 0.0f, 1.0f);
              baseTarget.smoothCircleParams.invert = EditorGUILayout.Toggle("Invert", baseTarget.smoothCircleParams.invert);
              baseTarget.smoothCircleParams.center = EditorGUILayout.Vector2Field("Center", baseTarget.smoothCircleParams.center);
              break;

            case CameraTransitionEffects.SmoothLine:
              baseTarget.smoothLineParams.angle = EditorGUILayout.Slider("Angle", baseTarget.smoothLineParams.angle, 0.0f, 360.0f);
              baseTarget.smoothLineParams.smoothness = EditorGUILayout.Slider("Smoothness", baseTarget.smoothLineParams.smoothness, 0.0f, 1.0f);
              break;

            case CameraTransitionEffects.Swap:
              baseTarget.swapParams.perspective = EditorGUILayout.Slider("Perspective", baseTarget.swapParams.perspective, 0.0f, 1.0f);
              baseTarget.swapParams.depth = EditorGUILayout.Slider("Depth", baseTarget.swapParams.depth, 0.0f, 100.0f);
              baseTarget.swapParams.reflection = EditorGUILayout.Slider("Reflection", baseTarget.swapParams.reflection, 0.0f, 1.0f);
              break;
            
            case CameraTransitionEffects.Valentine:
              baseTarget.valentineParams.border = EditorGUILayout.Slider("Border", baseTarget.valentineParams.border, 0.0f, 100.0f);
              baseTarget.valentineParams.color = EditorGUILayout.ColorField("Color", baseTarget.valentineParams.color);
              break;

            case CameraTransitionEffects.WarpWave:
              baseTarget.warpWaveParams.mode = (CameraTransitionWarpWave.Modes)EditorGUILayout.EnumPopup("Mode", baseTarget.warpWaveParams.mode);
              baseTarget.warpWaveParams.curvature = EditorGUILayout.Slider("Curvature", baseTarget.warpWaveParams.curvature, 0.0f, 5.0f);
              break;
          }

          EditorGUI.indentLevel--;

          EditorGUILayout.Separator();

          baseTarget.transitionTime = EditorGUILayout.Slider(@"Transition time", baseTarget.transitionTime, 0.0f, 10.0f);

          baseTarget.executeMethod = (CameraTransitionsAssistant.ExecutionMethod)EditorGUILayout.EnumPopup(@"Execution", baseTarget.executeMethod);
          if (baseTarget.executeMethod == CameraTransitionsAssistant.ExecutionMethod.OnEnable)
          {
            EditorGUI.indentLevel++;

            baseTarget.delayTime = EditorGUILayout.Slider(@"Delay", baseTarget.delayTime, 0.0f, 60.0f);

            EditorGUI.indentLevel--;
          }

          if (baseTarget.cameraTransition != null && baseTarget.cameraTransition.ProgressMode == CameraTransition.ProgressModes.Manual)
          {
            EditorGUILayout.Separator();

            GUI.enabled = baseTarget.cameraTransition.IsRunning;

            baseTarget.cameraTransition.Progress = EditorGUILayout.Slider(@"Progress", baseTarget.cameraTransition.Progress, 0.0f, 1.0f);

            GUI.enabled = true;
          }

          EditorGUILayout.Separator();

          baseTarget.cameraA = EditorGUILayout.ObjectField(new GUIContent(@"Camera A", "The source camera"), baseTarget.cameraA, typeof(Camera), true) as Camera;

          baseTarget.cameraB = EditorGUILayout.ObjectField(new GUIContent(@"Camera B", "The target camera"), baseTarget.cameraB, typeof(Camera), true) as Camera;

          EditorGUILayout.Separator();

          EditorGUILayout.BeginHorizontal();
          {
            baseTarget.easeType = (EaseType)EditorGUILayout.EnumPopup("Easing", baseTarget.easeType);

            if (baseTarget.easeType != EaseType.Linear)
              baseTarget.easeMode = (EaseMode)EditorGUILayout.EnumPopup(baseTarget.easeMode);
          }
          EditorGUILayout.EndHorizontal();

          EditorGUILayout.Separator();

          baseTarget.cameraTransition = EditorGUILayout.ObjectField(new GUIContent(@"Camera Transition", "The CameraTransition component"), baseTarget.cameraTransition, typeof(CameraTransition), true) as CameraTransition;

          EditorGUILayout.Separator();

          GUI.enabled = (EditorApplication.isPlaying == true);

          if (GUILayout.Button(AddSpacesToName(string.Format("Execute '{0}'", baseTarget.transitionEffect.ToString()))) == true)
            baseTarget.ExecuteTransition();

          GUI.enabled = true;

          EditorGUILayout.Separator();

          if (baseTarget.cameraTransition == null)
            EditorGUILayout.HelpBox(@"Assign a valid CameraTransition component.", MessageType.Warning);
          else if (baseTarget.cameraA == null || baseTarget.cameraB == null)
            EditorGUILayout.HelpBox(@"Assign two valid cameras.", MessageType.Warning);
          else if (baseTarget.cameraA == baseTarget.cameraB)
            EditorGUILayout.HelpBox(@"The cameras must be different.", MessageType.Error);

          EditorGUILayout.BeginHorizontal();
          {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("[doc]", "Online documentation"), GUI.skin.label) == true)
              Application.OpenURL(@"http://www.ibuprogames.com/2015/11/10/camera-transitions/");
          }
          EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(target);

        EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0.0f;

        EditorGUI.indentLevel = 0;
      }

      private string AddSpacesToName(string name)
      {
        return System.Text.RegularExpressions.Regex.Replace(name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
      }
    }
  }
}