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
    /// Transition Swap.
    /// </summary>
    public sealed class CameraTransitionSwap : CameraTransitionBase
    {
      /// <summary>
      /// Perspective [0.0 - 1.0]. Default 0.2.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.2f)]
      public float Perspective
      {
        get { return perspective; }
        set { perspective = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      /// <summary>
      /// Depth [0.0 - 100.0]. Default 3.
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 3.0f)]
      public float Depth
      {
        get { return depth; }
        set { depth = Mathf.Clamp(value, 0.0f, 100.0f); }
      }

      /// <summary>
      /// Reflection [0.0 - 1.0]. Default 0.4.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.4f)]
      public float Reflection
      {
        get { return reflection; }
        set { reflection = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      [SerializeField]
      private float perspective = 0.2f;

      [SerializeField]
      private float depth = 3.0f;

      [SerializeField]
      private float reflection = 0.4f;

      private const string variableSwapPerspective = @"_SwapPerspective";
      private const string variableSwapDepth = @"_SwapDepth";
      private const string variablSwapReflection = @"_SwapReflection";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        perspective = 0.2f;
        depth = 3.0f;
        reflection = 0.4f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 3 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(float))
        {
          Perspective = (float)parameters[0];
          Depth = (float)parameters[1];
          Reflection = (float)parameters[2];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Swap' required parameters: perspective (float), depth (float), reflection (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableSwapPerspective, perspective);
        material.SetFloat(variableSwapDepth, depth);
        material.SetFloat(variablSwapReflection, reflection);
      }
    }
  }
}