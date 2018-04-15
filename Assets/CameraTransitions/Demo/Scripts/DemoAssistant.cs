///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;

using UnityEngine;

using Ibuprogames.CameraTransitionsAsset;

namespace CameraTransitionsDemo
{
  /// <summary>
  /// CameraTransitionsAssistant Demo source code. For a more complex example, load 'Demo/Demo.scene'.
  /// </summary>
  public sealed class DemoAssistant : MonoBehaviour
  {
    public CameraTransitionsAssistant[] assistants = new CameraTransitionsAssistant[10];

    private GUIStyle labelStyle;

    private CameraTransition cameraTransition;

    private void OnEnable()
    {
      cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
      if (cameraTransition == null)
      {
        Debug.LogError(@"No CameraTransition object found.");

        this.enabled = false;
      }

      if (assistants.Length == 0)
      {
        Debug.LogError(@"No CameraTransitionsAssistant found.");

        this.enabled = false;
      }
    }

    private void Update()
    {
      for (int i = 0; i < assistants.Length; ++i)
      {
        if (Input.GetKeyUp(KeyCode.Alpha0 + i) == true && assistants[i] != null)
          assistants[i].ExecuteTransition();
      }
    }

    private void OnGUI()
    {
      if (labelStyle == null)
      {
        labelStyle = new GUIStyle(GUI.skin.textArea);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 22;
      }

      string label = string.Empty;
      if (cameraTransition.IsRunning == true)
        label = cameraTransition.Transition.ToString();
      else
        label = @"Press any number";

      GUI.enabled = !cameraTransition.IsRunning;

      GUILayout.BeginArea(new Rect(Screen.width * 0.5f - 150.0f, 20.0f, 300.0f, 30.0f), label, labelStyle);
      GUILayout.EndArea();
    }
  }
}