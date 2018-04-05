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
  /// Linear blur transition.
  /// </summary>
  public sealed class CameraTransitionLinearBlur : CameraTransitionBase
  {
    /// <summary>
    /// Blur intensity [0 - 1].
    /// </summary>
    public float Intensity
    {
      get { return intensity; }
      set { intensity = value; }
    }

    /// <summary>
    /// Blur quality [1 - 8].
    /// </summary>
    public int Passes
    {
      get { return passes; }
      set { passes = value; }
    }

    [SerializeField, HideInInspector]
    private float intensity = 0.1f;

    [SerializeField, HideInInspector]
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
