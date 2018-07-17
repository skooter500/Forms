///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

namespace CameraTransitionsDemo
{
  /// <summary>
  /// Auto translation.
  /// </summary>
  public sealed class AutoTranslate : MonoBehaviour
  {
    /// <summary>
    /// Translation velocity.
    /// </summary>
    public Vector3 moveSpeed = Vector3.zero;

    private Vector3 translationOriginal = Vector3.zero;

    private void OnEnable()
    {
      translationOriginal = gameObject.transform.position;
    }

    private void Update()
    {
      gameObject.transform.position = translationOriginal + new Vector3(moveSpeed.x * Mathf.Sin(Time.realtimeSinceStartup), moveSpeed.y * Mathf.Sin(Time.realtimeSinceStartup), moveSpeed.z * Mathf.Cos(Time.realtimeSinceStartup));
    }
  }
}