using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    /// <summary>
    /// The white cube in the beat anticipation scene.
    /// Flashes to a color and emits a particle effect when a ball collides with it.
    /// </summary>
    public class CubeTarget : MonoBehaviour
    {

        public ParticleSystem particle;
        public Color hitColor;
        public float flashTime = 1f;

        Material mat;
        Color startColor;

        private void Start()
        {
            mat = this.GetComponent<MeshRenderer>().material;
            startColor = mat.color;
        }

        //play a flash effect when hit
        private void OnCollisionEnter(Collision collision)
        {
            PlayFlashEffect();
            collision.gameObject.SetActive(false);

        }

        void ResetColor()
        {
            mat.color = startColor;
        }

        public void PlayFlashEffect()
        {
            particle.Emit(30);
            mat.color = hitColor;
            Invoke("ResetColor", flashTime);
        }
    }
}
