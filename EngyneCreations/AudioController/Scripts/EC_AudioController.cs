/*
 * EC_AudioController.cs
 * 2D Audio Groups and Audio Clips holder. Can be called with EC_AudioController.Play(groupName, interrupt(optional)).
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/engyne09/Unity-AudioController
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EC_AudioController : MonoBehaviour {

    [System.Serializable]
    public class AudioTrack {
        public string name;
        [Tooltip("Audio to play")]
        public AudioClip clip;
        [Header("Audio Settings")]
        [MinMaxRange(0f, 1f)]
        public MinMaxRange volume;
        float randomVolume;
        [MinMaxRange(-3f, 3f)]
        public MinMaxRange pitch;
        float randomPitch;
        [Header("Player Settings")]
        [Tooltip("Even if a mixer is set, this will ignore it")]
        public bool bypassFX;
        [Tooltip("Avoid this clip from playing")]
        public bool mute;
    }

    [System.Serializable]
    public class AudioGroup {
        [Header("Audio Group ID")]
        public string groupName;
        [Header("Group Settings")]
        [Range(0, 256)]
        [Tooltip("Lower is better")]
        public int priority = 128;
        [Tooltip("Optional. Used with AudioMixer Groups")]
        public AudioMixerGroup mixGroup;
        [Header("Audio Clips")]
        public AudioTrack[] clips;
    }

    public AudioGroup[] audioGroup;
    private Transform parentHolder;


    void Start() {

        GameObject audioHolder = GameObject.FindGameObjectWithTag("AudioHolder");

        if (!audioHolder) {
            audioHolder = new GameObject("[AudioHolder]");
            audioHolder.tag = "AudioHolder";
        }

        parentHolder = audioHolder.transform;
    }

    public void Play(string group, bool loop = false, bool interrupt = false) {

		//Debug.Log ("Playing sound " + group + " on GameObject + " + this.gameObject.name + ".");

        AudioGroup tempGroup = null;

        for (int i = 0; i < audioGroup.Length; i++) {
            if (audioGroup[i].groupName == group) {
                tempGroup = audioGroup[i];
                break;
            }
        }
        if (tempGroup == null) {
			Debug.LogError("Audio Group: " + '"' + group + '"' + " namespace not found. Check if it's correctly spelled or created. Offending gameObject: " + gameObject.name);
            return;
        }

        List<int> clipsId = new List<int>();

        for (int i = 0; i < tempGroup.clips.Length;i++) {
            if (!tempGroup.clips[i].mute) {
                clipsId.Add(i);
            }
        }

        if (clipsId.Count == 0) {
            Debug.Log("Audio Group: " + '"' + group + '"' + " doesn't contain AudioClips or they are all muted.");
            return;
        }

        GenerateSource(group, tempGroup.clips[clipsId[Random.Range(0, clipsId.Count)]], tempGroup.priority, tempGroup.mixGroup, loop, interrupt);
    }

    private void GenerateSource(string group, AudioTrack track, int priority, AudioMixerGroup mixer, bool loop, bool interrupt) {

        // Stop all the other sounds
        if (interrupt) {
            int children = parentHolder.childCount;

            for (int i = 0; i < children; ++i) {
                parentHolder.transform.GetChild(i).GetComponent<AudioSource>().Stop();
                Destroy(parentHolder.transform.GetChild(i).gameObject);
            }
        }

        GameObject tempObj = new GameObject("Temporal AudioSource");
        tempObj.transform.SetParent(parentHolder, true);
        AudioSource source = tempObj.AddComponent<AudioSource>();
        EC_AudioTemporalSource tempAudio = tempObj.AddComponent<EC_AudioTemporalSource>();
        tempAudio.groupName = group;

        source.volume = track.volume.GetRandomValue();
        source.pitch = track.pitch.GetRandomValue();

        if (mixer && !track.bypassFX) {
            source.outputAudioMixerGroup = mixer;
        }

		source.clip = track.clip;

        if (loop) {
            source.loop = true;
			source.Play();
        } else {
			source.Play();
            tempAudio.StartInvoke(track.clip.length);
        }
    }

    public void Stop(string group) {

        int children = parentHolder.childCount;

        for (int i = 0; i < children; ++i) {

            Transform child = parentHolder.transform.GetChild(i);

            if (child.GetComponent<EC_AudioTemporalSource>().groupName == group) {
                child.GetComponent<AudioSource>().Stop();
				Destroy(child.gameObject);
            }
        }
    }
}