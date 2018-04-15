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
    /// CameraTransition Editor.
    /// </summary>
    [CustomEditor(typeof(CameraTransition))]
    public class CameraTransitionEditor : Editor
    {
      private string warnings;

      private string errors;

      private CameraTransition baseTarget;

      private bool foldoutAdvancedSettings = false;

      private void OnEnable()
      {
        foldoutAdvancedSettings = EditorPrefs.GetBool(@"CameraTransitions.AdvancedSettingsFoldout", false);
      }

      private void OnDestroy()
      {
        EditorPrefs.SetBool(@"CameraTransitions.AdvancedSettingsFoldout", foldoutAdvancedSettings);
      }

      /// <summary>
      /// OnInspectorGUI.
      /// </summary>
      public override void OnInspectorGUI()
      {
        if (baseTarget == null)
          baseTarget = this.target as CameraTransition;

        EditorHelper.IndentLevel = 0;

        EditorHelper.FieldWidth = 0.0f;
        EditorHelper.LabelWidth = 125.0f;

        EditorHelper.BeginVertical();
        {
          EditorHelper.Separator();

          foldoutAdvancedSettings = EditorHelper.Foldout(foldoutAdvancedSettings, @"Advanced settings");
          if (foldoutAdvancedSettings == true)
          {
            EditorHelper.BeginVertical();
            {
              EditorHelper.Label(@"Behavior");

              EditorHelper.IndentLevel++;

              baseTarget.ProgressMode = (CameraTransition.ProgressModes)EditorHelper.EnumPopup(@"Progress", @"How the effect progresses.",
                                                                                                baseTarget.ProgressMode, CameraTransition.ProgressModes.Automatic);

              if (baseTarget.ProgressMode == CameraTransition.ProgressModes.Manual)
              {
                EditorHelper.IndentLevel++;

                EditorHelper.Enabled = baseTarget.IsRunning;

                baseTarget.Progress = EditorHelper.Slider(string.Empty, baseTarget.Progress, 0.0f, 1.0f, 0.0f);

                EditorHelper.Enabled = true;

                EditorHelper.IndentLevel--;
              }

              baseTarget.UseScaledTime = EditorGUILayout.Toggle(@"Use scaled time", baseTarget.UseScaledTime);

              baseTarget.CustomTimeScale = EditorHelper.Slider(@"Time scale", "Custom time scale factor.", baseTarget.CustomTimeScale, 0.0f, 10.0f, 1.0f);

              EditorHelper.IndentLevel--;
            }
            EditorHelper.EndVertical();

            EditorHelper.Separator();

            EditorHelper.BeginVertical();
            {
              EditorHelper.Label(@"RenderTexture");
              
              EditorHelper.IndentLevel++;

              baseTarget.RenderTextureMode = (CameraTransition.RenderTextureModes)EditorHelper.EnumPopup(@"Mode", "How handle the RenderTexture.",
                                                                                                          baseTarget.RenderTextureMode, CameraTransition.RenderTextureModes.Automatic);

              if (baseTarget.RenderTextureMode != CameraTransition.RenderTextureModes.Automatic)
              {
                warnings += @"In manual mode, the target camera must handle its own RenderTexture.";

                EditorHelper.Enabled = false;
              }

              baseTarget.RenderTextureUpdateMode = (CameraTransition.RenderTextureUpdateModes)EditorHelper.EnumPopup(@"Update", "The destination camera is updated during\nthe entire transition or only the first frame.",
                                                                                                                      baseTarget.RenderTextureUpdateMode, CameraTransition.RenderTextureUpdateModes.AllFrames);

              baseTarget.RenderTextureSize = (CameraTransition.RenderTextureSizes)EditorHelper.EnumPopup(@"Size", "The size of the render texture.\nThe smaller, the worse the quality.",
                                                                                                          baseTarget.RenderTextureSize, CameraTransition.RenderTextureSizes.SameAsScreen);

              baseTarget.InvertRenderTexture = EditorHelper.Toggle(@"Invert", baseTarget.InvertRenderTexture, false);

              EditorHelper.Enabled = true;

              EditorHelper.IndentLevel--;
            }
            EditorHelper.EndVertical();
          }

          if (string.IsNullOrEmpty(warnings) == false)
          {
            EditorHelper.Separator();

            EditorGUILayout.HelpBox(warnings, MessageType.Warning);
          }

          if (string.IsNullOrEmpty(errors) == false)
          {
            EditorHelper.Separator();

            EditorGUILayout.HelpBox(errors, MessageType.Error);
          }

          EditorGUILayout.HelpBox(@"CameraTransition is a collection of transition effects between two cameras.", MessageType.Info);

          EditorHelper.BeginHorizontal();
          {
            EditorHelper.FlexibleSpace();
            
            if (EditorHelper.Button("[doc]", "Online documentation", GUI.skin.label) == true)
              Application.OpenURL(@"http://www.ibuprogames.com/2015/11/10/camera-transitions/");
          }
          EditorHelper.EndHorizontal();
        }
        EditorHelper.EndVertical();

        warnings = errors = string.Empty;

        if (EditorHelper.Changed == true)
          EditorHelper.SetDirty(target);

        EditorHelper.LabelWidth = EditorHelper.FieldWidth = 0.0f;
        EditorHelper.IndentLevel = 0;
      }
    }
  }
}