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
    /// Pixelate transition.
    /// </summary>
    public sealed class CameraTransitionPixelate : CameraTransitionBase
    {
      /// <summary>
      /// Max size of the pixels [0 - ...].
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 50.0f)]
      public float Size
      {
        get { return pixelSize; }
        set { pixelSize = (value > 0.0 ? value : 0.0f); }
      }

      [SerializeField]
      private float pixelSize = 50.0f;

      private const string variableSize = @"_PixelSize";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        pixelSize = 50.0f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 1 && parameters[0].GetType() == typeof(float))
          Size = (float)parameters[0];
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Pixelate' required parameters: size (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableSize, pixelSize);
      }
    }
  }
}