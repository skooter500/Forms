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
  /// Transition Gate.
  /// </summary>
  public sealed class CameraTransitionGate : CameraTransitionBase
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
    /// Depth [0.0 - 100.0].
    /// </summary>
    public float Depth
    {
      get { return depth; }
      set { depth = Mathf.Clamp(value, 0.0f, 100.0f); }
    }

    /// <summary>
    /// Cube reflection [0.0 - 1.0].
    /// </summary>
    public float Reflection
    {
      get { return reflection; }
      set { reflection = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    [SerializeField, HideInInspector]
    private float perspective = 0.4f;

    [SerializeField, HideInInspector]
    private float depth = 3.0f;

    [SerializeField, HideInInspector]
    private float reflection = 0.4f;

    private const string variableGatePerspective = @"_GatePerspective";
    private const string variableGateDepth = @"_GateDepth";
    private const string variablGateReflection = @"_GateReflection";

    /// <summary>
    /// Set the default values of the shader.
    /// </summary>
    public override void ResetDefaultValues()
    {
      base.ResetDefaultValues();

      perspective = 0.4f;
      depth = 3.0f;
      reflection = 0.4f;
    }

    /// <summary>
    /// Set the values to shader.
    /// </summary>
    protected override void SendValuesToShader()
    {
      base.SendValuesToShader();

      material.SetFloat(variableGatePerspective, perspective);
      material.SetFloat(variableGateDepth, depth);
      material.SetFloat(variablGateReflection, reflection);
    }
  }
}
