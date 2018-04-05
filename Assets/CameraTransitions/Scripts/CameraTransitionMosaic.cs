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
  /// Mosaic transition.
  /// </summary>
  public sealed class CameraTransitionMosaic : CameraTransitionBase
  {
    /// <summary>
    /// Cell to jump to [-10 - 10].
    /// Avoid jump to the same cell (0 - 0).
    /// </summary>
    public Vector2 Steps
    {
      get { return steps; }
      set
      {
        Vector2 jumpTo = new Vector2(Mathf.Floor(value.x), Mathf.Floor(value.y));
        if (jumpTo == Vector2.zero)
          Debug.LogWarning(@"Avoid jump to the same cell.");

        steps = jumpTo;
      }
    }

    /// <summary>
    /// Random rotate cells? [true - false].
    /// </summary>
    public bool Rotate
    {
      get { return rotate; }
      set { rotate = value; }
    }

    [SerializeField, HideInInspector]
    private Vector2 steps = Vector2.one;

    [SerializeField, HideInInspector]
    private bool rotate = false;

    private const string variableStepX = @"_StepX";
    private const string variableStepY = @"_StepY";
    private const string variableRotate = @"_Rotate";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      steps = Vector2.one;
      rotate = false;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      material.SetInt(variableStepX, (int)steps.x);
      material.SetInt(variableStepY, (int)steps.y);
      material.SetInt(variableRotate, rotate == true ? 1 : 0);
    }
  }
}
