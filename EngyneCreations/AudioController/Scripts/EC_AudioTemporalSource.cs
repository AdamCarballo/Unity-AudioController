/*
 * EC_AudioTemporalSource.cs
 * Automatically generated when a new sound spawns. Handles timers and destroy methods.
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/engyne09/LINK
 */

using UnityEngine;

public class EC_AudioTemporalSource : MonoBehaviour {

    [HideInInspector]
    public string groupName;


	public void StartInvoke(float time) {

        Invoke("Destroy", time);
	}

    private void Destroy() {

        Destroy(gameObject);
    }
}