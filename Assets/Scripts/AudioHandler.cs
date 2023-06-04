using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler singleton;
    List<AudioSource> audioSources = new List<AudioSource>();
    [Range(0, 1)] public float fxVolume = 1;
    [Range(0, 1)] public float masterVolume = 1;

    public enum FXClipName { empty,fire}
    [System.Serializable]
    public struct FXClip
    {
        public FXClipName name;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume;
    }
    public List<FXClip> fXClips = new List<FXClip>();

    private void Start()
    {
        singleton = this;
    }
    public void PlayClip(FXClipName clipName)
    {
        FXClip fXClip = GetFXClip(clipName);
        AudioSource source = GetAudioSource();
        source.clip = fXClip.audioClip;
        source.volume = fXClip.volume * fxVolume * masterVolume;
        source.Play();
    }

    FXClip GetFXClip(FXClipName clipName)
    {
        foreach(FXClip fXClip in fXClips) { if (clipName == fXClip.name) return fXClip; }
        Debug.LogWarning($"clipName:{clipName} not found");
        return default;
    }

    AudioSource GetAudioSource()
    {
        foreach(AudioSource source in audioSources)
        {
            if (!source.isPlaying) return source;
        }
        audioSources.Add(gameObject.AddComponent<AudioSource>());
        return audioSources[audioSources.Count - 1];
    }
}
