﻿///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
  /// Smooth line transition.
  /// </summary>
  public sealed class CameraTransitionSmoothLine : CameraTransitionBase
  {
    /// <summary>
    /// Angle [0 - 360].
    /// </summary>
    public float Angle
    {
      get { return Mathf.Atan2(angle.y, angle.x); }
      set { angle = new Vector2(Mathf.Sin(value * Mathf.Deg2Rad), Mathf.Cos(value * Mathf.Deg2Rad)); angle.Normalize(); }
    }

    /// <summary>
    /// If 0 color has little influence on the effect, if 1, the color color influences and lasts more. [0 - 1].
    /// </summary>
    public float Smoothness
    {
      get { return smoothness; }
      set { smoothness = value; }
    }

    [SerializeField, HideInInspector]
    private Vector2 angle = new Vector2(1.0f, 1.0f);

    [SerializeField, HideInInspector]
    private float smoothness = 0.5f;

    private const string variableAngle = @"_Angle";
    private const string variableSmoothness = @"_Smoothness";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      angle = new Vector2(1.0f, 1.0f);
      smoothness = 0.5f;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      material.SetVector(variableAngle, angle);
      material.SetFloat(variableSmoothness, smoothness);
    }
  }
}
