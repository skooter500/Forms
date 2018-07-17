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
    /// Transition Valentine.
    /// </summary>
    public sealed class CameraTransitionValentine : CameraTransitionBase
    {
      /// <summary>
      /// Border width [0.0 - 100.0]. Default 25.
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 25.0f)]
      public float Border
      {
        get { return border; }
        set { border = Mathf.Clamp(value, 0.0f, 100.0f); }
      }

      /// <summary>
      /// Border color.
      /// </summary>
      public Color Color
      {
        get { return color; }
        set { color = value; }
      }

      [SerializeField]
      private float border = 25.0f;

      [SerializeField]
      private Color color = Color.red;

      private const string variableValentineBorder = @"_ValentineBorder";
      private const string variableValentineColor = @"_ValentineColor";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        border = 25.0f;
        color = Color.red;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(Color))
        {
          Border = (float)parameters[0];
          Color = (Color)parameters[1];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Valentine' required parameters: border width (float), border color (Color).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableValentineBorder, border);
        material.SetColor(variableValentineColor, color);
      }
    }
  }
}