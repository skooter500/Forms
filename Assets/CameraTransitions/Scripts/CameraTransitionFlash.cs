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
    /// Transition Flash.
    /// </summary>
    public sealed class CameraTransitionFlash : CameraTransitionBase
    {
      /// <summary>
      /// The importance of the Flash in effect [0.0 - 1.0]. Default 0.3.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.3f)]
      public float Strength
      {
        get { return strength; }
        set { strength = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// The intensity of the flash [0.0 - 5.0]. Default 0.3.
      /// </summary>
      [RangeFloat(0.0f, 5.0f, 3.0f)]
      public float Intensity
      {
        get { return intensity; }
        set { intensity = value; }
      }

      /// <summary>
      /// If 0, there is no zoom effect, if 1 the zoom effect is intense [0.0 - 1.0]. Default 0.5.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.5f)]
      public float Zoom
      {
        get { return zoom; }
        set { zoom = value; }
      }

      /// <summary>
      /// The speed of the effect [0.1 - 10]. Default 3.
      /// </summary>
      [RangeFloat(0.1f, 10.0f, 3.0f)]
      public float Velocity
      {
        get { return velocity; }
        set { velocity = value > 0.0f ? value : 0.1f; }
      }

      /// <summary>
      /// The color.
      /// </summary>
      public Color Color
      {
        get { return color; }
        set { color = value; }
      }

      [SerializeField]
      private float strength = 0.3f;

      [SerializeField]
      private float intensity = 3.0f;

      [SerializeField]
      private float zoom = 0.5f;

      [SerializeField]
      private float velocity = 3.0f;

      [SerializeField]
      private Color color = new Color(1.0f, 0.8f, 0.3f);

      private const string variableFlashPhase = @"_FlashPhase";
      private const string variableFlashIntensity = @"_FlashIntensity";
      private const string variableFlashZoom = @"_FlashZoom";
      private const string variableFlashVelocity = @"_FlashVelocity";
      private const string variableFlashColor = @"_FlashColor";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        strength = 0.3f;
        intensity = 3.0f;
        zoom = 0.5f;
        velocity = 3.0f;
        color = new Color(1.0f, 0.8f, 0.3f);
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 5 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(float) &&
                                      parameters[3].GetType() == typeof(float) &&
                                      parameters[4].GetType() == typeof(Color))
        {
          Strength = (float)parameters[0];
          Intensity = (float)parameters[1];
          Zoom = (float)parameters[2];
          Velocity = (float)parameters[3];
          Color = (Color)parameters[4];
        }
        else
          Debug.LogWarning("@[Ibuprogames.CameraTransitions] Effect 'Flash' required parameters: strength (float), intensity (float), zoom (float), velocity (float), color (Color).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableFlashPhase, strength);
        material.SetFloat(variableFlashIntensity, intensity);
        material.SetFloat(variableFlashZoom, zoom);
        material.SetFloat(variableFlashVelocity, velocity);
        material.SetColor(variableFlashColor, color);
      }
    }
  }
}