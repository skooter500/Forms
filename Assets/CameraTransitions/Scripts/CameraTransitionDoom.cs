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
    /// Doom transition.
    /// </summary>
    public sealed class CameraTransitionDoom : CameraTransitionBase
    {
      /// <summary>
      /// Bar width [0 - ...]. Default 10.
      /// </summary>
      [RangeInt(0, 200, 10)]
      public int BarWidth
      {
        get { return barWidth; }
        set { barWidth = value < 0 ? 0 : value; }
      }

      /// <summary>
      /// Height difference between bars [0.0 - 25.0]. Default 2.
      /// </summary>
      [RangeFloat(0.0f, 25.0f, 2.0f)]
      public float Amplitude
      {
        get { return amplitude; }
        set { amplitude = Mathf.Clamp(value, 0.0f, 25.0f); }
      }

      /// <summary>
      /// Height noise [0.0 - 1.0]. Default 0.1.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.1f)]
      public float Noise
      {
        get { return noise; }
        set { noise = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      /// <summary>
      /// Wave frequency [0.0 - 100.0]. Default 1.
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 1.0f)]
      public float Frequency
      {
        get { return frequency; }
        set { frequency = Mathf.Clamp(value, 0.0f, 100.0f); }
      }

      [SerializeField]
      private int barWidth = 10;

      [SerializeField]
      private float amplitude = 2.0f;

      [SerializeField]
      private float noise = 0.1f;

      [SerializeField]
      private float frequency = 1.0f;

      private const string variableBarWidth = @"_BarWidth";
      private const string variableAmplitude = @"_Amplitude";
      private const string variableNoise = @"_Noise";
      private const string variableFrequency = @"_Frequency";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        barWidth = 10;
        amplitude = 2.0f;
        noise = 0.1f;
        frequency = 1.0f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 4 && parameters[0].GetType() == typeof(int) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(float) &&
                                      parameters[3].GetType() == typeof(float))
        {
          BarWidth = (int)parameters[0];
          Amplitude = (float)parameters[1];
          Noise = (float)parameters[2];
          Frequency = (float)parameters[3];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Doom' required parameters: bar width (int), amplitude (float), noise (float), frequency (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetInt(variableBarWidth, barWidth);
        material.SetFloat(variableAmplitude, amplitude);
        material.SetFloat(variableNoise, noise);
        material.SetFloat(variableFrequency, frequency);
      }
    }
  }
}