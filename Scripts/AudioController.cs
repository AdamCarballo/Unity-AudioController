/*
 * AudioController.cs
 * 2D Audio Groups and Audio Clips holder.
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamCarballo/Unity-AudioController
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace F10dev.Audio {
    public class AudioController : MonoBehaviour {

        [Serializable]
        public class AudioTrack {
            [HideInInspector]
            [SerializeField]
            [UsedImplicitly]
            private string _name;

            [Tooltip("Audio to play")]
            [SerializeField]
            private AudioClip _clip;

            [Header("Audio Settings")]
            [MinMaxRange(0f, 1f)]
            [SerializeField]
            private MinMaxRange _volume;

            [FormerlySerializedAs("pitch")]
            [MinMaxRange(-3f, 3f)]
            [SerializeField]
            private MinMaxRange _pitch;

            [Tooltip("Even if a mixer is set, this will ignore it")]
            [SerializeField]
            private bool _bypassFX;

            [Tooltip("Avoid this clip from playing")]
            [SerializeField]
            private bool _mute;

            public AudioClip Clip => _clip;

            public bool BypassFX => _bypassFX;

            public bool Mute => _mute;

            public float GetVolume() {
                return _volume.GetRandomValue();
            }

            public float GetPitch() {
                return _pitch.GetRandomValue();
            }

#if UNITY_EDITOR
            public void SetInspectorName() {
                _name = _clip.name;
            }
#endif
        }

        [Serializable]
        public class AudioGroup {
            [SerializeField]
            private string _groupName;

            [Header("Group Settings")]

            [Range(0, 256)]
            [Tooltip("Lower is better")]
            [SerializeField]
            private int _priority = 128;

            [Tooltip("Optional. Used with AudioMixer Groups")]
            [SerializeField]
            private AudioMixerGroup _mixGroup;

            [Header("Audio Tracks")]
            [SerializeField]
            private AudioTrack[] _tracks;

            public string GroupName => _groupName;

            public int Priority => _priority;

            public AudioMixerGroup MixGroup => _mixGroup;

            public AudioTrack[] Tracks => _tracks;

            public IReadOnlyList<AudioTrack> GetAllNonMutedTracks() {
                return _tracks.Where(t => !t.Mute).ToList();
            }
        }

        private Transform _parentHolder;

        [SerializeField]
        private AudioGroup[] _audioGroups;



        private void Start() {

            // GameObject audioHolder = GameObject.FindGameObjectWithTag("AudioHolder");
            //
            // if (!audioHolder) {
            //     audioHolder = new GameObject("[AudioHolder]");
            //     audioHolder.tag = "AudioHolder";
            // }

            //parentHolder = audioHolder.transform;
            _parentHolder = transform;
        }

        public void Play(string groupId, bool loop = false, bool interrupt = false) {
            Debug.Log($"Playing sound \"{groupId}\" on GameObject: {name}.");

            var group = _audioGroups.FirstOrDefault(t => t.GroupName == groupId);

            if (group == null) {
                Debug.LogError(
                    $"Audio Group: \"{groupId}\" namespace not found. Check if it's correctly spelled or created. Offending GameObject: {name}");
                return;
            }

            var tracks = group.GetAllNonMutedTracks();
            if (tracks.Count == 0) {
                Debug.LogWarning($"Audio Group: \"{groupId}\" doesn't contain AudioClips or they are all muted.");
                return;
            }

            GenerateSource(groupId, tracks[Random.Range(0, tracks.Count)], group.Priority, group.MixGroup, loop,
                interrupt);
        }

        private void GenerateSource(string id, AudioTrack track, int priority, AudioMixerGroup mixer, bool loop,
            bool interrupt) {
            // Stop all the other sounds
            if (interrupt) {
                var children = _parentHolder.childCount;

                for (var i = 0; i < children; ++i) {
                    var child = _parentHolder.transform.GetChild(i).gameObject;
                    child.GetComponent<AudioSource>().Stop();
                    Destroy(child);
                }
            }

            var tempObj = new GameObject("Temporary_AudioSource");
            tempObj.transform.SetParent(_parentHolder, true);

            var source = tempObj.AddComponent<AudioSource>();
            source.clip = track.Clip;
            source.volume = track.GetVolume();
            source.pitch = track.GetPitch();
            source.priority = priority;
            if (mixer && !track.BypassFX) {
                source.outputAudioMixerGroup = mixer;
            }

            var tempAudio = tempObj.AddComponent<AudioTemporalSource>();
            tempAudio.groupName = id;

            if (loop) {
                source.loop = true;
                source.Play();
            }
            else {
                source.Play();
                tempAudio.StartInvoke(track.Clip.length);
            }
        }

        public void Stop(string group) {

            var children = _parentHolder.childCount;

            for (var i = 0; i < children; ++i) {

                var child = _parentHolder.transform.GetChild(i);

                if (child.GetComponent<AudioTemporalSource>().groupName == group) {
                    child.GetComponent<AudioSource>().Stop();
                    Destroy(child.gameObject);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate() {
            foreach (var group in _audioGroups) {
                foreach (var track in group.Tracks) {
                    if (track.Clip != null) {
                        track.SetInspectorName();
                    }
                }
            }
        }
#endif
    }
}