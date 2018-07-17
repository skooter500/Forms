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
    /// Transition Fade To Color.
    /// </summary>
    public sealed class CameraTransitionFadeToColor : CameraTransitionBase
    {
      /// <summary>
      /// If 0 color has little influence on the effect, if 1, the color color influences and lasts more. [0 - 1]. Default 0.3.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.3f)]
      public float Strength
      {
        get { return strength; }
        set { strength = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// The color.
      /// </summary>
      public Color Color
      {
        get { return color; }
        set { color = value; }
      }

      [SerializeField]
      private float strength = 0.3f;

      [SerializeField]
      private Color color = Color.black;

      private const string variableFadeColor = @"_FadeColor";
      private const string variableFadePhase = @"_FadePhase";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        color = Color.black;
        strength = 0.3f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(Color))
        {
          Strength = (float)parameters[0];
          Color = (Color)parameters[1];
        }
        else
          Debug.LogWarning("@[Ibuprogames.CameraTransitions] Effect 'Fade To Color' required parameters: strength (float), color (Color).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetColor(variableFadeColor, color);
        material.SetFloat(variableFadePhase, strength);
      }
    }
  }
}