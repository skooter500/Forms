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
    /// Transition Smooth circle.
    /// </summary>
    public sealed class CameraTransitionSmoothCircle : CameraTransitionBase
    {
      /// <summary>
      /// If 0 the edge of the circle is not smooth, if 1 is very smooth. [0 - 1]. Default 0.3.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.3f)]
      public float Smoothness
      {
        get { return smoothness; }
        set { smoothness = value; }
      }

      /// <summary>
      /// Center [0 - 1]. Default (0.5, 0.5).
      /// </summary>
      [RangeVector2(0.0f, 1.0f, 0.5f)]
      public Vector2 Center
      {
        get { return center; }
        set { center = value; }
      }

      /// <summary>
      /// Opening or closing. Default false.
      /// </summary>
      public bool Invert
      {
        get { return invert; }
        set { invert = value; }
      }

      [SerializeField]
      private float smoothness = 0.3f;

      [SerializeField]
      private Vector2 center = Vector2.one * 0.5f;

      [SerializeField]
      private bool invert = false;

      private const string variableSmoothness = @"_Smoothness";
      private const string variableCenter = @"_Center";
      private const string variableInvert = @"_Invert";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        smoothness = 0.3f;
        center = Vector2.one * 0.5f;
        invert = false;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 3 && parameters[0].GetType() == typeof(bool) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(Vector2))
        {
          Invert = (bool)parameters[0];
          Smoothness = (float)parameters[1];
          Center = (Vector2)parameters[2];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Smooth Circle' required parameters: invert (bool), smoothness (float), center (Vector2).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableSmoothness, smoothness);
        material.SetInt(variableInvert, invert == true ? 1 : 0);
        material.SetVector(variableCenter, center);
      }
    }
  }
}