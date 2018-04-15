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
    /// Transition Glitch.
    /// </summary>
    public sealed class CameraTransitionGlitch : CameraTransitionBase
    {
      /// <summary>
      /// Displacement strength [0.0 - 10.0]. Default 0.4.
      /// </summary>
      [RangeFloat(0.0f, 10.0f, 0.4f)]
      public float Strength
      {
        get { return strength; }
        set { strength = Mathf.Clamp(value, 0.0f, 10.0f); }
      }

      [SerializeField]
      private float strength = 0.4f;

      private const string variableGlitchStrength = @"_GlitchStrength";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        strength = 0.4f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 1)
          Strength = (float)parameters[0];
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Glitch' required parameters: strength (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableGlitchStrength, strength);
      }
    }
  }
}