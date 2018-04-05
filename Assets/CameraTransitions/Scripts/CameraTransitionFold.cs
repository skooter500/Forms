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
  /// Fold transition.
  /// </summary>
  public sealed class CameraTransitionFold : CameraTransitionBase
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

    [SerializeField, HideInInspector]
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
