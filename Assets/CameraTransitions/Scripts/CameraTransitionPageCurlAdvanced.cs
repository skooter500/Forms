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
  /// A more configurable version of 'Page Curl', more mobile friendly.
  /// </summary>
  public sealed class CameraTransitionPageCurlAdvanced : CameraTransitionBase
  {
    /// <summary>
    /// Angle [0.0 - 180.0].
    /// </summary>
    public float Angle
    {
      get { return angle; }
      set { angle = Mathf.Clamp(value, 0.0f, 180.0f); }
    }

    /// <summary>
    /// Page radius [0 - 1].
    /// </summary>
    public float Radius
    {
      get { return radius; }
      set { radius = Mathf.Clamp01(value); }
    }

    /// <summary>
    /// Back page transparency [0 - 1].
    /// </summary>
    public float BackTransparency
    {
      get { return backTransparency; }
      set { backTransparency = Mathf.Clamp01(value); }
    }

    /// <summary>
    /// Front shadow [true - false].
    /// </summary>
    public bool FrontShadow
    {
      get { return frontShadow; }
      set { frontShadow = value; }
    }

    /// <summary>
    /// Back shadow [true - false].
    /// </summary>
    public bool BackShadow
    {
      get { return backShadow; }
      set { backShadow = value; }
    }

    /// <summary>
    /// Inner shadow [true - false].
    /// </summary>
    public bool InnerShadow
    {
      get { return innerShadow; }
      set { innerShadow = value; }
    }

    [SerializeField, HideInInspector]
    private float angle = 45.0f;

    [SerializeField, HideInInspector]
    private float radius = 0.1f;

    [SerializeField, HideInInspector]
    private float backTransparency = 0.25f;

    [SerializeField, HideInInspector]
    private bool frontShadow = true;

    [SerializeField, HideInInspector]
    private bool backShadow = true;

    [SerializeField, HideInInspector]
    private bool innerShadow = true;

    private const string variableAngle = @"_Angle";
    private const string variableRadius = @"_Radius";
    private const string variableObtuse = @"_Obtuse";
    private const string variableBackTransparency = @"_BackTransparency";
    private const string variableFrontShadow = @"_FrontShadow";
    private const string variableBackShadow = @"_BackShadow";
    private const string variableInnerShadow = @"_InnerShadow";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      angle = 45.0f;
      radius = 0.1f;
      backTransparency = 0.25f;
      frontShadow = true;
      backShadow = true;
      innerShadow = true;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      float rad = (angle < 90.0f ? angle : angle - 90.0f) * Mathf.Deg2Rad;

      Vector2 angleSinCos = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
      angleSinCos.Normalize();

      material.SetInt(variableObtuse, angle > 90.0f ? 1 : 0);
      material.SetVector(variableAngle, angleSinCos);
      material.SetFloat(variableRadius, radius);

      material.SetInt(variableFrontShadow, frontShadow == true ? 1 : 0);
      material.SetInt(variableBackShadow, backShadow == true ? 1 : 0);
      material.SetInt(variableInnerShadow, innerShadow == true ? 1 : 0);
      material.SetFloat(variableBackTransparency, backTransparency);
    }
  }
}
