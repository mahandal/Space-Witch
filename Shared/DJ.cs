using UnityEngine;
using System.Collections.Generic;

public class DJ : MonoBehaviour
{
    // Singleton.
    public static DJ I;

    [Header("Sources")]
    public AudioSource masterSource;
    public AudioSource ambienceSource;
    public AudioSource musicSource;
    public AudioSource narratorSource;
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

    // Play a sound effect, on the SFX channel by default.
    public void PlaySound(string soundName, string audioChannel = "SFX")
    {
        if (library.TryGetValue(soundName, out AudioClip clip))
        {
            // Master
            // if (audioChannel == "Master")
            //     masterSource.PlayOneShot(clip);
            // Ambience
            if (audioChannel == "Ambience")
                ambienceSource.PlayOneShot(clip);
            // Music
            if (audioChannel == "Music")
                musicSource.PlayOneShot(clip);
            // Narrator
            if (audioChannel == "Narrator")
                narratorSource.PlayOneShot(clip);
            // SFX
            if (audioChannel == "SFX")
                sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
        }
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