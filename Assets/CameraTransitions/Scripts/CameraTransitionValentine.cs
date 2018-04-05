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
  /// Transition Valentine.
  /// </summary>
  public sealed class CameraTransitionValentine : CameraTransitionBase
  {
    /// <summary>
    /// Border width [0.0 - 100.0].
    /// </summary>
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

    [SerializeField, HideInInspector]
    private float border = 25.0f;

    [SerializeField, HideInInspector]
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
