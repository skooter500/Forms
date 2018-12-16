using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioVisualizer
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleFrequencyEmitter : MonoBehaviour
    {
        //________ Public Variables ________

        //________ Private Variables ________
        ParticleSystem particleSystem;

        //________ Monobehaviour Methods ________

        private void Awake()
        {
            particleSystem = this.GetComponent<ParticleSystem>();
        }

        //________ Public Methods ________

        public void Emit(float val)
        {
            particleSystem.Emit((int)val);
        }

        //________ Private Methods________
    }
}
