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

namespace CameraTransitions
{
  /// <summary>
  /// Doom transition.
  /// </summary>
  public sealed class CameraTransitionDoom : CameraTransitionBase
  {
    /// <summary>
    /// Bar width [1 - ...].
    /// </summary>
    public int BarWidth
    {
      get { return barWidth; }
      set { barWidth = value < 1 ? 1 : value; }
    }

    /// <summary>
    /// Height difference between bars [0.0 - 25.0].
    /// </summary>
    public float Amplitude
    {
      get { return amplitude; }
      set { amplitude = Mathf.Clamp(value, 0.0f, 25.0f); }
    }

    /// <summary>
    /// Height noise [0.0 - 1.0].
    /// </summary>
    public float Noise
    {
      get { return noise; }
      set { noise = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    /// <summary>
    /// Wave frequency [0.0 - 100.0].
    /// </summary>
    public float Frequency
    {
      get { return frequency; }
      set { frequency = Mathf.Clamp(value, 0.0f, 100.0f); }
    }

    [SerializeField, HideInInspector]
    private int barWidth = 10;

    [SerializeField, HideInInspector]
    private float amplitude = 2.0f;

    [SerializeField, HideInInspector]
    private float noise = 0.1f;

    [SerializeField, HideInInspector]
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
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      material.SetFloat(variableBarWidth, barWidth);
      material.SetFloat(variableAmplitude, amplitude);
      material.SetFloat(variableNoise, noise);
      material.SetFloat(variableFrequency, frequency);
    }
  }
}
