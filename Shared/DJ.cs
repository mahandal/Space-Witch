using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DJ : MonoBehaviour
{
    // Singleton.
    public static DJ I;

    [Header("Audio Listener")]
    public AudioListener audioListener;

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

        // +++ Load audio clips.

        // + Ambience
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Ambience"))
            library[clip.name] = clip;

        // + Music
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Music"))
            library[clip.name] = clip;

        // + Narrator
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Narrator"))
            library[clip.name] = clip;

        // Names
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Names"))
            library[clip.name] = clip;

        // + SFX
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("SFX"))
            library[clip.name] = clip;
    }

    // + Fixed Update
    void FixedUpdate()
    {
        audioListener.transform.position = Utility.GetMouseCoords();
    }

    // + Narrator

    public void Narrate(string soundName)
    {
        // If no sound found, return.
        if (!library.TryGetValue(soundName, out AudioClip clip))
            return;

        PlaySound(soundName, "Narrator");
    }

    public void NarrateName(Leader leader)
    {
        // Get sound name.
        string soundName = leader.myName;

        // If no sound found, use default.
        if (!library.TryGetValue(soundName, out AudioClip clip))
            soundName = "Merlin";

        PlaySound(soundName, "Narrator");
    }

    // + SFX

    // Create an audio source to use for a unit's movement.
    public AudioSource CreateMovementAudioSource(Unit unit)
    {
        // Create a new audio source by cloning our sfx audio source.
        AudioSource movementAudioSource = Object.Instantiate(sfxSource, unit.transform);

        // Get sound name.
        string soundName = unit.GetBaseName() + " movement";

        // Structures don't move!
        if (unit.cardType == "Structure")
            soundName = unit.GetBaseName() + " idle";

        // Find sound in library.
        if (library.TryGetValue(soundName, out AudioClip clip))
            movementAudioSource.clip = clip;
        else if (unit.cardType == "Structure")
            movementAudioSource.clip = null;
        else
            movementAudioSource.clip = library["Movement"];

        // Set spatial blend.
        movementAudioSource.spatialBlend = 1f;

        // Set loop.
        movementAudioSource.loop = true;

        return movementAudioSource;
    }

    // Play a unit's attack sound.
    public void PlayAttackSound(Unit unit)
    {
        // Get sound name.
        string soundName = unit.GetBaseName() + " attack";

        // If no sound found, use default.
        if (!library.TryGetValue(soundName, out AudioClip clip))
            soundName = "Attack";

        PlaySound(soundName, unit.transform.position);
    }

    // Play a unit's deployed sound.
    public void PlayDeployedSound(Unit unit)
    {
        // Get sound name.
        string soundName = unit.GetBaseName() + " deployed";

        // If no sound found, use default.
        if (!library.TryGetValue(soundName, out AudioClip clip))
        {
            // Units
            if (unit.cardType == "Unit")
                soundName = "Unit Ready";

            // Spells
            if (unit.cardType == "Spell")
                soundName = "Spell Cast";

            // Structures
            if (unit.cardType == "Structure")
                soundName = "Structure Complete";

            // Items
            if (unit.cardType == "Item")
                soundName = "Item Done";
        }

        PlaySound(soundName, unit.transform.position);
    }

    // Play a unit's death sound.
    public void PlayDeathSound(Unit unit)
    {
        // Get sound name.
        string soundName = unit.GetBaseName() + " death";

        // If no sound found, use default.
        if (!library.TryGetValue(soundName, out AudioClip clip))
            soundName = "Death";

        PlaySound(soundName, unit.transform.position);
    }

     // Play a sound effect, on the SFX channel by default.
    public void PlaySound(string soundName, string audioChannel = "SFX")
    {
        if (library.TryGetValue(soundName, out AudioClip clip))
        {
            AudioSource source = GetSource(audioChannel);
            if (source != null)
                source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
        }
    }

    // Play a sound effect positionally from a world position, on the SFX channel by default.
    public void PlaySound(string soundName, Vector3 position, string audioChannel = "SFX")
    {
        if (!library.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
            return;
        }
 
        AudioSource template = GetSource(audioChannel);
        if (template == null)
            return;

        // Clone a new audio source.
        AudioSource source = Object.Instantiate(template);

        // Set position.
        source.transform.position = position;

        // Set spatial blend.
        source.spatialBlend = 1f;
 
        // Set clip.
        source.clip = clip;

        // Play.
        source.Play();

        // Clean up.
        Destroy(source.gameObject, clip.length / Mathf.Max(0.01f, Mathf.Abs(source.pitch)));
    }


    // + Music and Ambience

    // Hit it!
    public static void HitIt(string whereAreWe)
    {
        DJ.I.PlayAmbienceAndMusic(whereAreWe);
    }

    public static void HitIt(Planet p)
    {
        // Play ambience.
        I.PlayAmbience("Ambience - " + p.myName);

        // Play music.
        I.PlayMusic(p.themeSong);
    }

    // This is what hit it means yes.
    public void PlayAmbienceAndMusic(string whereAreWe)
    {
        // Play ambience.
        PlayAmbience("Ambience - " + whereAreWe);

        // Play music.
        PlayMusic("Music - " + whereAreWe);
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

    // Play a music track.
    public void PlayMusic(string soundName)
    {
        if (!library.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"DJ: no sound named \"{soundName}\"");
            return;
        }

        // Already playing this one? Don't restart it.
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Get the AudioSource for a channel name.
    private AudioSource GetSource(string audioChannel)
    {
        if (audioChannel == "Master") return masterSource;
        else if (audioChannel == "Ambience") return ambienceSource;
        else if (audioChannel == "Music") return musicSource;
        else if (audioChannel == "Narrator") return narratorSource;
        else if (audioChannel == "SFX") return sfxSource;
        else return null;
    }
}