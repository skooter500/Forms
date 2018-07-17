///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

namespace Ibuprogames
{
  namespace CameraTransitionsAsset
  {
    /// <summary>
    /// Page Curl transition.
    /// </summary>
    public sealed class CameraTransitionPageCurl : CameraTransitionBase
    {
      /// <summary>
      /// Angle [0.0 - 180.0]. Default 45.
      /// </summary>
      [RangeFloat(0.0f, 180.0f, 45.0f)]
      public float Angle
      {
        get { return angle; }
        set { angle = Mathf.Clamp(value, 0.0f, 180.0f); }
      }

      /// <summary>
      /// Page radius [0.0 - 1.0]. Default 0.1.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.1f)]
      public float Radius
      {
        get { return radius; }
        set { radius = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// Enable shadows [true - false]. Default true.
      /// </summary>
      public bool Shadows
      {
        get { return shadows; }
        set
        {
          if (value != shadows)
          {
            shadows = value;

            LoadShader();
          }
        }
      }

      [SerializeField]
      private float angle = 45.0f;

      [SerializeField]
      private float radius = 0.1f;

      [SerializeField]
      private bool shadows = true;

      /// <summary>
      /// Shader path.
      /// </summary>
      protected override string ShaderPath { get { return string.Format("Shaders/TransitionPageCurl{0}", shadows == true ? string.Empty : "NoShadows"); } }

      private const string variableAngle = @"_Angle";
      private const string variableRadius = @"_Radius";
      private const string variableObtuse = @"_Obtuse";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        angle = 45.0f;
        radius = 0.1f;
        shadows = true;

        LoadShader();
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 3 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(bool))
        {
          Angle = (float)parameters[0];
          Radius = (float)parameters[1];
          Shadows = (bool)parameters[2];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Page Curl' required parameters: angle (float), radius (float), shadows (bool).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        float rad = (angle < 90.0f ? angle : angle - 90.0f) * Mathf.Deg2Rad;

        Vector2 angleSinCos = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        angleSinCos.Normalize();

        material.SetInt(variableObtuse, angle > 90.0f ? 1 : 0);
        material.SetVector(variableAngle, angleSinCos);
        material.SetFloat(variableRadius, radius);
      }
    }
  }
}
