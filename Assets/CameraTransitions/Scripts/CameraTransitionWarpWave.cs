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
  /// Warp Wave transition.
  /// </summary>
  public sealed class CameraTransitionWarpWave : CameraTransitionBase
  {
    /// <summary>
    /// 
    /// </summary>
    public enum Modes
    {
      /// <summary>
      ///
      /// </summary>
      Horizontal = 0,

      /// <summary>
      ///
      /// </summary>
      Vertical = 1,
    }

    /// <summary>
    /// Mode [Horizontal / Vertical].
    /// </summary>
    public Modes Mode
    {
      get { return mode; }
      set { mode = value; }
    }

    /// <summary>
    /// Curvature of the wave [0.0 - 5.0].
    /// </summary>
    public float Curvature
    {
      get { return curvature; }
      set { curvature = Mathf.Clamp(value, 0.0f, 5.0f); }
    }

    [SerializeField, HideInInspector]
    private Modes mode = Modes.Horizontal;

    [SerializeField, HideInInspector]
    private float curvature = 0.5f;

    private const string variableWarpWaveCurvature = @"_WarpWaveCurvature";

    private const string keywordMode = @"MODE_HORIZONTAL";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      mode = Modes.Horizontal;
      curvature = 0.5f;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      if (mode == Modes.Horizontal)
        material.EnableKeyword(keywordMode);
      else
        material.DisableKeyword(keywordMode);

      material.SetFloat(variableWarpWaveCurvature, curvature);
    }
  }
}
