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
  /// Transition Cube.
  /// </summary>
  public sealed class CameraTransitionCube : CameraTransitionBase
  {
    /// <summary>
    /// Perspective [0.0 - 1.0].
    /// </summary>
    public float Perspective
    {
      get { return perspective; }
      set { perspective = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    /// <summary>
    /// Zoom [0.0 - 100.0].
    /// </summary>
    public float Zoom
    {
      get { return zoom; }
      set { zoom = Mathf.Clamp(value, 0.0f, 100.0f); }
    }

    /// <summary>
    /// Cube reflection [0.0 - 1.0].
    /// </summary>
    public float Reflection
    {
      get { return reflection; }
      set { reflection = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    /// <summary>
    /// Cube elevantion [0.0 - 100.0].
    /// </summary>
    public float Elevation
    {
      get { return elevantion; }
      set { elevantion = Mathf.Clamp(value, 0.0f, 100.0f); }
    }

    [SerializeField, HideInInspector]
    private float perspective = 0.7f;

    [SerializeField, HideInInspector]
    private float zoom = 0.3f;

    [SerializeField, HideInInspector]
    private float reflection = 0.4f;

    [SerializeField, HideInInspector]
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
