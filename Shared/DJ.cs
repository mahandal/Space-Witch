using UnityEngine;
using System.Collections.Generic;

public class DJ : MonoBehaviour
{
    // Singleton.
    public static DJ I;

    [Header("Ambience")]
    public AudioSource ambienceSource;

    [Header("SFX")]
    public AudioSource sfxSource;

    [Header("Audio clips")]
    Dictionary<string, AudioClip> library = new();

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        // + Load audio clips.

        // SFX
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("SFX"))
            library[clip.name] = clip;

        // Ambience
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Ambience"))
            library[clip.name] = clip;
    }

    // + Activity
    // Hit it!
    public static void HitIt(string whereAreWe)
    {
        DJ.I.PlayAmbienceAndMusic(whereAreWe);
    }

    // This is what hit it means yes.
    public void PlayAmbienceAndMusic(string whereAreWe)
    {
        // Play ambience.
        PlayAmbience("Ambience - " + whereAreWe);

        // TBD: Play music!
    }

    // Play a sound effect.
    public void PlaySound(string soundName)
    {
        if (library.TryGetValue(soundName, out AudioClip clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
    }

    // Play an ambience track.
    public void PlayAmbience(string soundName)
    {
        if (!library.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
            return;
        }

        // Already playing this one? Don't restart it.
        if (ambienceSource.clip == clip && ambienceSource.isPlaying)
            return;

        ambienceSource.clip = clip;
        ambienceSource.Play();
    }
}