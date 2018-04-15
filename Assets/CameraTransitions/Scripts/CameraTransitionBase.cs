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
    /// Transition base.
    /// </summary>
    public abstract class CameraTransitionBase : MonoBehaviour
    {
      /// <summary>
      /// Progress effect [0.0 - 1.0].
      /// </summary>
      [RangeFloat(0.0f, 1.0f)]
      public float Progress
      {
        get { return progress; }
        set { progress = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// Reverse progress.
      /// </summary>
      public bool Reverse
      {
        get { return reverse; }
        set { reverse = value; }
      }

      /// <summary>
      /// Internal use only.
      /// </summary>
      internal Texture RenderTexture
      {
        get { return secondTexture; }
        set { secondTexture = value; }
      }

      /// <summary>
      /// Internal use only.
      /// </summary>
      internal bool InvertRenderTexture
      {
        set { invertRenderTexture = value; }
      }

      /// <summary>
      /// Shader path.
      /// </summary>
      protected virtual string ShaderPath { get { return string.Format("Shaders/{0}", this.GetType().ToString().Replace(@"Ibuprogames.CameraTransitionsAsset.Camera", string.Empty)); } }

      [SerializeField]
      private float progress;

      private bool reverse = false;

      protected Material material;

      protected Texture secondTexture;

      private bool invertRenderTexture = false;

      private const string variableProgress = @"_T";
      private const string variableSecondTex = @"_SecondTex";

      private const string keywordReverse = @"MODE_REVERSE";
      private const string keywordInvertRenderTexture = @"INVERT_RENDERTEXTURE";

      /// <summary>
      /// Set the default values of the effect.
      /// </summary>
      public virtual void ResetDefaultValues() { }

      /// <summary>
      /// Set parameters.
      /// </summary>
      public virtual void SetParameters(object[] parameters) { }

      /// <summary>
      /// Check hardware requirements.
      /// </summary>
      protected virtual bool CheckHardwareRequirements()
      {
        bool isSupported = true;

        if (SystemInfo.supportsImageEffects == false)
        {
          Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] Hardware not support Image Effects. '{0}' disabled.", this.GetType().ToString());

          isSupported = false;
        }

        return isSupported;
      }

      /// <summary>
      /// Load the shader.
      /// </summary>
      protected void LoadShader()
      {
        Shader shader = Resources.Load<Shader>(ShaderPath);
        if (shader != null)
        {
          if (shader.isSupported == true)
          {
            if (material != null)
              DestroyImmediate(material);

            material = new Material(shader);
            if (material == null)
            {
              Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] '{0}' material null.", this.GetType().ToString());

              this.enabled = false;
            }
          }
          else
          {
            Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] Shader '{0}' not supported.", this.GetType().ToString());

            this.enabled = false;
          }
        }
        else
        {
          Debug.LogErrorFormat("[Ibuprogames.CameraTransitions] '{0}' shader not found. Please contact to 'hello@ibuprogames.com'.", ShaderPath);

          this.enabled = false;
        }
      }

      /// <summary>
      /// Set the values to shader.
      /// </summary>
      protected virtual void SendValuesToShader()
      {
        if (reverse == true)
        {
          material.EnableKeyword(keywordReverse);

          material.SetFloat(variableProgress, 1.0f - progress);
        }
        else
          material.SetFloat(variableProgress, progress);

        material.SetTexture(variableSecondTex, secondTexture);
      }

      private void Awake()
      {
        if (CheckHardwareRequirements() == true)
          LoadShader();
        else
          this.enabled = false;
      }

      /// <summary>
      /// Destroy resources.
      /// </summary>
      protected virtual void OnDestroy()
      {
        if (material != null)
          DestroyImmediate(material);

        secondTexture = null;
      }

      private void OnRenderImage(RenderTexture source, RenderTexture destination)
      {
        if (material != null)
        {
          if (invertRenderTexture == true)
            material.EnableKeyword(keywordInvertRenderTexture);
          else
            material.DisableKeyword(keywordInvertRenderTexture);

          SendValuesToShader();

          Graphics.Blit(source, destination, material);
        }
      }
    }
  }
}