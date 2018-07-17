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
    /// Random grid transition.
    /// </summary>
    public sealed class CameraTransitionRandomGrid : CameraTransitionBase
    {
      /// <summary>
      /// Rows [0 - ...]. Default 10.
      /// </summary>
      [RangeInt(0, 100, 10)]
      public int Rows
      {
        get { return (int)gridSize.y; }
        set { gridSize.y = (value > 0.0 ? value : 0.0f); }
      }

      /// <summary>
      /// Columns [0 - ...]. Default 10.
      /// </summary>
      [RangeInt(0, 100, 10)]
      public int Columns
      {
        get { return (int)gridSize.x; }
        set { gridSize.x = (value > 0.0 ? value : 0.0f); }
      }

      /// <summary>
      /// Smoothness of the transition between rectangles [0.0 - 1.0]. Default 0.5.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.5f)]
      public float Smoothness
      {
        get { return smoothness; }
        set { smoothness = value; }
      }

      [SerializeField]
      private Vector2 gridSize = new Vector2(10.0f, 10.0f);

      [SerializeField]
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
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 3 && parameters[0].GetType() == typeof(int) &&
                                      parameters[1].GetType() == typeof(int) &&
                                      parameters[2].GetType() == typeof(float))
        {
          Rows = (int)parameters[0];
          Columns = (int)parameters[1];
          Smoothness = (float)parameters[2];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Random Grid' required parameters: rows (int), columns (int), smoothness (float).");
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
}