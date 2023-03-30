/*
 * AudioPlayButton.cs
 * Help class to generate sounds from a 4.6 Unity UI system.
 * (Current UI OnClick() function doesn't allow functions with multiple parameters).
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamCarballo/Unity-AudioController
 */

using UnityEngine;

namespace F10dev.Audio {
	public class AudioPlayButton : MonoBehaviour {

		public AudioController audioController;
		public bool loop = false;
		public bool interrupt = false;


		public void Play(string group) {
			audioController.Play(group, loop, interrupt);
		}
	}
}