/*
 * EC_AudioTemporalSource.cs
 * #DESCRIPTION#
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/engyne09/LINK
 */

using UnityEngine;

public class EC_AudioTemporalSource : MonoBehaviour {

	public void StartInvoke(float time) {

        Invoke("Destroy", time);
	}

    private void Destroy() {
        Destroy(gameObject);
    }
}