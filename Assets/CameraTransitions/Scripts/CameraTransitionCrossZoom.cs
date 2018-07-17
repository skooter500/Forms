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
    /// Transition Cross Zoom.
    /// </summary>
    public sealed class CameraTransitionCrossZoom : CameraTransitionBase
    {
      /// <summary>
      /// The amount of effect. [0.0 - 1.0]. Default 0.4.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.4f)]
      public float Strength
      {
        get { return strength; }
        set { strength = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// Quality effect. [0.0 - 40.0]. Default 20.
      /// </summary>
      [RangeFloat(0.0f, 40.0f, 20.0f)]
      public float Quality
      {
        get { return quality; }
        set { quality = Mathf.Abs(Mathf.Clamp(value, 1.0f, 40.0f)); }
      }

      [SerializeField]
      private float strength = 0.4f;

      [SerializeField]
      private float quality = 20.0f;

      private const string variableStrength = @"_Strength";
      private const string variableQuality = @"_Quality";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        strength = 0.4f;
        quality = 20.0f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 2 && parameters[0].GetType() == typeof(float)
                                  && parameters[0].GetType() == typeof(float))
        {
          Strength = (float)parameters[0];
          Quality = (float)parameters[1];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Cross Zoom' required parameters: strength (float), quality (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableStrength, strength);
        material.SetFloat(variableQuality, quality);
      }
    }
  }
}