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
    /// Linear blur transition.
    /// </summary>
    public sealed class CameraTransitionLinearBlur : CameraTransitionBase
    {
      /// <summary>
      /// Blur intensity [0.0 - 1.0]. Default 0.1.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.1f)]
      public float Intensity
      {
        get { return intensity; }
        set { intensity = value; }
      }

      /// <summary>
      /// Blur quality [1 - 8]. Default 8.
      /// </summary>
      [RangeInt(1, 8, 8)]
      public int Passes
      {
        get { return passes; }
        set { passes = value; }
      }

      [SerializeField]
      private float intensity = 0.1f;

      [SerializeField]
      private int passes = 8;

      private const string variableIntensity = @"_Intensity";
      private const string variablePasses = @"_Passes";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        intensity = 0.1f;
        passes = 8;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 2 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(int))
        {
          Intensity = (float)parameters[0];
          Passes = (int)parameters[1];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Linear Blur' required parameters: intensity (float), passes (int).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableIntensity, intensity);
        material.SetInt(variablePasses, passes);
      }
    }
  }
  }