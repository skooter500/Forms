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
    /// Fold transition.
    /// </summary>
    public sealed class CameraTransitionFold : CameraTransitionBase
    {
      /// <summary>
      /// Fold transition.
      /// </summary>
      public enum Modes
      {
        /// <summary>
        /// Horizontal.
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Vertical.
        /// </summary>
        Vertical = 1,
      }

      /// <summary>
      /// Mode [Horizontal / Vertical]. Default Vertical.
      /// </summary>
      [Enum(typeof(Modes))]
      public Modes Mode
      {
        get { return mode; }
        set { mode = value; }
      }

      [SerializeField]
      private Modes mode = Modes.Vertical;

      private const string keywordMode = @"MODE_HORIZONTAL";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        mode = Modes.Vertical;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 1 && parameters[0].GetType() == typeof(CameraTransitionFold.Modes))
          Mode = (CameraTransitionFold.Modes)parameters[0];
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Fold' required parameters: mode (CameraTransitionFlip.Modes).");
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
      }
    }
  }
}