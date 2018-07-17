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
    /// Transition Cube.
    /// </summary>
    public sealed class CameraTransitionCube : CameraTransitionBase
    {
      /// <summary>
      /// Perspective [0.0 - 1.0]. Default 0.7.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.7f)]
      public float Perspective
      {
        get { return perspective; }
        set { perspective = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      /// <summary>
      /// Zoom [0.0 - 100.0]. Default 0.3.
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 0.3f)]
      public float Zoom
      {
        get { return zoom; }
        set { zoom = Mathf.Clamp(value, 0.0f, 100.0f); }
      }

      /// <summary>
      /// Cube reflection [0.0 - 1.0]. Default 0.4.
      /// </summary>
      [RangeFloat(0.0f, 1.0f, 0.4f)]
      public float Reflection
      {
        get { return reflection; }
        set { reflection = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      /// <summary>
      /// Cube elevantion [0.0 - 100.0]. Default 3.
      /// </summary>
      [RangeFloat(0.0f, 100.0f, 3.0f)]
      public float Elevation
      {
        get { return elevantion; }
        set { elevantion = Mathf.Clamp(value, 0.0f, 100.0f); }
      }

      [SerializeField]
      private float perspective = 0.7f;

      [SerializeField]
      private float zoom = 0.3f;

      [SerializeField]
      private float reflection = 0.4f;

      [SerializeField]
      private float elevantion = 3.0f;

      private const string variableCubePerspective = @"_CubePerspective";
      private const string variableCubeZoom = @"_CubeZoom";
      private const string variableCubeReflection = @"_CubeReflection";
      private const string variableCubeElevantion = @"_CubeElevantion";

      /// <summary>
      /// Set the default values of the shader.
      /// </summary>
      public override void ResetDefaultValues()
      {
        base.ResetDefaultValues();

        perspective = 0.7f;
        zoom = 0.3f;
        reflection = 0.4f;
        elevantion = 3.0f;
      }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public override void SetParameters(object[] parameters)
      {
        if (parameters.Length == 4 && parameters[0].GetType() == typeof(float) &&
                                      parameters[1].GetType() == typeof(float) &&
                                      parameters[2].GetType() == typeof(float) &&
                                      parameters[3].GetType() == typeof(float))
        {
          Perspective = (float)parameters[0];
          Zoom = (float)parameters[1];
          Reflection = (float)parameters[2];
          Elevation = (float)parameters[3];
        }
        else
          Debug.LogWarning(@"[Ibuprogames.CameraTransitions] Effect 'Cubes' required parameters: perspective (float), zoom (float), reflection (float), elevation (float).");
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected override void SendValuesToShader()
      {
        base.SendValuesToShader();

        material.SetFloat(variableCubePerspective, perspective);
        material.SetFloat(variableCubeZoom, zoom);
        material.SetFloat(variableCubeReflection, reflection);
        material.SetFloat(variableCubeElevantion, -elevantion / 100.0f);
      }
    }
  }
}