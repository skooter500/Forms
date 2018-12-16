using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AudioVisualizer
{
	public class ParticleController : MonoBehaviour {

		public List<ParticleSystem> particleSystems;
		public int updateRate; // play all particle systems every 'burstRate" seconds
		private int updateCounter = 0; // count update frames

		// Update is called once per frame
		void FixedUpdate () 
		{
			updateCounter ++; // count update frames
			if(updateCounter > updateRate) // if > updateRate
			{
				//play all the particles
				foreach(ParticleSystem ps in particleSystems)
				{
					ps.Play();
				}
				updateCounter = 0;
			}
		}
	}
}
