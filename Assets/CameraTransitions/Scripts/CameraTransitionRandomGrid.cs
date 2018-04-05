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
  /// Random grid transition.
  /// </summary>
  public sealed class CameraTransitionRandomGrid : CameraTransitionBase
  {
    /// <summary>
    /// Rows [0 - ...].
    /// </summary>
    public int Rows
    {
      get { return (int)gridSize.y; }
      set { gridSize.y = (value > 0.0 ? value : 0.0f); }
    }

    /// <summary>
    /// Columns [0 - ...].
    /// </summary>
    public int Columns
    {
      get { return (int)gridSize.x; }
      set { gridSize.x = (value > 0.0 ? value : 0.0f); }
    }

    /// <summary>
    /// Smoothness of the transition between rectangles [0 - 1].
    /// </summary>
    public float Smoothness
    {
      get { return smoothness; }
      set { smoothness = value; }
    }

    [SerializeField, HideInInspector]
    private Vector2 gridSize = new Vector2(10.0f, 10.0f);

    [SerializeField, HideInInspector]
    private float smoothness = 0.5f;

    private const string variableGridSize = @"_GridSize";
    private const string variableSmoothness = @"_Smoothness";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      gridSize = new Vector2(10.0f, 10.0f); 
      smoothness = 0.5f;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      material.SetVector(variableGridSize, gridSize);
      material.SetFloat(variableSmoothness, smoothness);
    }
  }
}
