using UnityEngine;
using System.Collections.Generic;

public class DJ : MonoBehaviour
{
    // Singleton.
    public static DJ I;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip[] sounds;

    Dictionary<string, AudioClip> library = new();

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        foreach (AudioClip clip in sounds)
            library[clip.name] = clip;
    }

    // + Activity
    public void PlaySound(string soundName)
    {
        if (library.TryGetValue(soundName, out AudioClip clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
    }
}