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
  /// Transition Smooth circle.
  /// </summary>
  public sealed class CameraTransitionSmoothCircle : CameraTransitionBase
  {
    /// <summary>
    /// If 0 the edge of the circle is not smooth, if 1 is very smooth. [0 - 1].
    /// </summary>
    public float Smoothness
    {
      get { return smoothness; }
      set { smoothness = value; }
    }

    /// <summary>
    /// .
    /// </summary>
    public Vector2 Center
    {
      get { return center; }
      set { center = value; }
    }

    /// <summary>
    /// Opening or closing.
    /// </summary>
    public bool Invert
    {
      get { return invert; }
      set { invert = value; }
    }

    [SerializeField, HideInInspector]
    private float smoothness = 0.3f;

    [SerializeField, HideInInspector]
    private Vector2 center = Vector2.one * 0.5f;

    [SerializeField, HideInInspector]
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