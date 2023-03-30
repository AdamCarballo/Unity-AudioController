/*
 * AudioTemporalSource.cs
 * Automatically generated when a new sound spawns. Handles timers and destroy methods.
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamCarballo/Unity-AudioController
 */

using UnityEngine;

namespace F10dev.Audio {
	public class AudioTemporalSource : MonoBehaviour {

		[HideInInspector]
		public string groupName;


		public void StartInvoke(float time) {
			Invoke("Destroy", time);
		}

		private void Destroy() {
			Destroy(gameObject);
		}
	}
}