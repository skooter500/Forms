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
using UnityEngine;
using UnityEditor;

namespace CameraTransitions
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

      EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0.0f;

      EditorGUI.indentLevel = 0;

      EditorGUIUtility.labelWidth = 125.0f;

      EditorGUILayout.BeginVertical();
      {
        EditorGUILayout.Separator();

        EditorGUILayout.HelpBox(@"CameraTransition is a collection of transition effects between two cameras in the same scene.", MessageType.Info);

        EditorGUILayout.Separator();

        foldoutAdvancedSettings = CameraTransitionEditorHelper.Foldout(foldoutAdvancedSettings, @"Advanced settings");
        if (foldoutAdvancedSettings == true)
        {
          EditorGUILayout.BeginVertical(@"Box");
          {
            EditorGUILayout.LabelField(@"Behaviors");

            EditorGUI.indentLevel++;

            baseTarget.ProgressMode = (CameraTransition.ProgressModes)EditorGUILayout.EnumPopup(new GUIContent(@"Progress", @"How the effect progresses."), baseTarget.ProgressMode);

            if (baseTarget.ProgressMode == CameraTransition.ProgressModes.Manual)
            {
              EditorGUI.indentLevel++;

              GUI.enabled = baseTarget.IsRunning;

              baseTarget.Progress = EditorGUILayout.Slider(baseTarget.Progress, 0.0f, 1.0f);

              GUI.enabled = true;

              EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(@"RenderTexture");
            
            EditorGUI.indentLevel++;

            baseTarget.RenderTextureMode = (CameraTransition.RenderTextureModes)EditorGUILayout.EnumPopup(new GUIContent(@"Mode", "How handle the RenderTexture."), baseTarget.RenderTextureMode);

            if (baseTarget.RenderTextureMode != CameraTransition.RenderTextureModes.Automatic)
            {
              warnings += @"In manual mode, the target camera must handle its own RenderTexture.";

              GUI.enabled = false;
            }

            baseTarget.RenderTextureUpdateMode = (CameraTransition.RenderTextureUpdateModes)EditorGUILayout.EnumPopup(new GUIContent(@"Update", "The destination camera is updated during\nthe entire transition or only the first frame."), baseTarget.RenderTextureUpdateMode);

            baseTarget.RenderTextureSize = (CameraTransition.RenderTextureSizes)EditorGUILayout.EnumPopup(new GUIContent(@"Size", "The size of the render texture.\nThe smaller, the worse the quality."), baseTarget.RenderTextureSize);

            baseTarget.RenderTextureDepth = (CameraTransition.RenderTextureDepths)EditorGUILayout.EnumPopup(new GUIContent(@"Depth", "The precision of the render texture's depth buffer in bits."), baseTarget.RenderTextureDepth);

            baseTarget.RenderTextureHDR = (CameraTransition.RenderTextureHDRModes)EditorGUILayout.EnumPopup(new GUIContent(@"HDR mode", "HDR format."), baseTarget.RenderTextureHDR);

            baseTarget.InvertRenderTexture = EditorGUILayout.Toggle(@"Invert", baseTarget.InvertRenderTexture);

            GUI.enabled = true;

            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            if (GUILayout.Button(@"Reset advanced options") == true)
              baseTarget.ResetAdvancedOptions();

            EditorGUILayout.Separator();
          }
          EditorGUILayout.EndVertical();
        }

        if (string.IsNullOrEmpty(warnings) == false)
        {
          EditorGUILayout.Separator();

          EditorGUILayout.HelpBox(warnings, MessageType.Warning);
        }

        if (string.IsNullOrEmpty(errors) == false)
        {
          EditorGUILayout.Separator();

          EditorGUILayout.HelpBox(errors, MessageType.Error);
        }

        EditorGUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          
          if (GUILayout.Button(new GUIContent("[doc]", "Online documentation"), GUI.skin.label) == true)
            Application.OpenURL(CameraTransitionEditorHelper.DocumentationURL);
        }
        EditorGUILayout.EndHorizontal();
      }
      EditorGUILayout.EndVertical();

      warnings = errors = string.Empty;

      if (GUI.changed == true)
        EditorUtility.SetDirty(target);

      EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0.0f;

      EditorGUI.indentLevel = 0;
    }
  }
}