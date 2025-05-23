using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float gatherVolume;

    [Header("Audio Manager nonsemse")]
    // Audio sources for playing music
    public AudioSource musicSource;
    private Dictionary<string, AudioSource> musicSources = new Dictionary<string, AudioSource>();
    
    // Dictionary to store audio clips by name
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> effectClips = new Dictionary<string, AudioClip>(); 

    public bool nextSongPreloaded = false;
    
    void Awake()
    {
        // Load saved volume settings (or use defaults if none exist)
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("EffectVolume", 1.0f);
        gatherVolume = PlayerPrefs.GetFloat("GatherVolume", 1.0f);

        // Add music AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        
        // Load clips
        LoadMusicClips();
        LoadEffectClips();
    }

    void Start()
    {
        // - Set volume sliders

        // Check where we are
        if (GM.I == null)
        {
            // Menu
            MainMenu.I.masterVolume.value = masterVolume;
            MainMenu.I.musicVolume.value = musicVolume;
            MainMenu.I.sfxVolume.value = sfxVolume;
            MainMenu.I.gatherVolume.value = gatherVolume;
        } 
        else 
        {
            // Game
            GM.I.ui.masterVolume.value = masterVolume;
            GM.I.ui.musicVolume.value = musicVolume;
            GM.I.ui.sfxVolume.value = sfxVolume;
            GM.I.ui.gatherVolume.value = gatherVolume;
        }
    }

    void Update()
    {
        // wait for game
        if (GM.I == null)
            return;
        
        // Check if current song is almost over
        if (GM.I.songTimer < 6 && !nextSongPreloaded)
        {
            // Preload next song
            //PreloadSong(GM.I.songs[(GM.I.songIndex + 1) % GM.I.songs.Count].myName);
            PreloadSong();
            nextSongPreloaded = true;
        }

        // Reset nextSongPreloaded
        if (GM.I.songTimer > 69 && nextSongPreloaded)
            nextSongPreloaded = false;
    }

    private void LoadMusicClips()
    {
        // This assumes your WAV files are in a Resources/Music folder
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Music");
        
        foreach (AudioClip clip in clips)
        {
            // Create a new AudioSource component for each clip
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = false;
            source.playOnAwake = false;
            source.volume = 0f; // Start with volume at 0
            
            // Add to dictionaries
            musicSources.Add(clip.name, source);
            musicClips.Add(clip.name, clip);
        }
    }

    // Load all effect clips from Resources folder
    private void LoadEffectClips()
    {
        // This assumes your effect files are in a Resources/SFX folder
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        
        foreach (AudioClip clip in clips)
        {
            effectClips.Add(clip.name, clip);
        }
    }
    
    // Play music by name
    public void PlayMusic(string songName)
    {
        if (!musicClips.ContainsKey(songName))
        {
            Debug.LogWarning("Song not found: " + songName);
            return;
        }

        //musicSource.clip = musicClips[songName];
        musicSource = musicSources[songName];
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }
    
    // Stop music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Play effect at specific world position
    public void PlayEffect(string effectName, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        if (!effectClips.ContainsKey(effectName))
        {
            Debug.LogWarning("Effect not found: " + effectName);
            return;
        }
        
        // Create temporary GameObject at position
        GameObject tempAudio = new GameObject("TempAudio_" + effectName);
        tempAudio.transform.position = position;
        
        // Add AudioSource component
        AudioSource effectSource = tempAudio.AddComponent<AudioSource>();
        
        // Configure the source
        effectSource.clip = effectClips[effectName];
        effectSource.spatialBlend = 1f;  // Use space
        effectSource.minDistance = 1f;    // Adjust based on your game scale
        effectSource.maxDistance = 13f;   // How far the sound can be heard
        effectSource.volume = volume * sfxVolume * masterVolume;
        effectSource.rolloffMode = AudioRolloffMode.Linear;  // Linear falloff with distance
        effectSource.loop = loop;
        
        // Play the sound
        effectSource.Play();
        
        // Destroy the GameObject after sound finishes
        if (!loop)
            Destroy(tempAudio, effectSource.clip.length + 0.1f);
    }

    public void PlayGatherSound(string effectName, Vector3 position, float volume = 1.0f)
    {
        if (!effectClips.ContainsKey(effectName))
        {
            Debug.LogWarning("Effect not found: " + effectName);
            return;
        }
        
        // Create temporary GameObject at position
        GameObject tempAudio = new GameObject("TempAudio_" + effectName);
        tempAudio.transform.position = position;
        
        // Add AudioSource component
        AudioSource effectSource = tempAudio.AddComponent<AudioSource>();
        
        // Configure the source
        effectSource.clip = effectClips[effectName];
        effectSource.spatialBlend = 1f;
        effectSource.minDistance = 1f;
        effectSource.maxDistance = 13f;
        effectSource.volume = volume * gatherVolume * masterVolume;
        effectSource.rolloffMode = AudioRolloffMode.Linear;
        
        // Play the sound
        effectSource.Play();
        
        // Destroy the GameObject after sound finishes
        Destroy(tempAudio, effectSource.clip.length + 0.1f);
    }

    // Adjust master volume
    public void SetMasterVolume(float volume)
    {
        // Set master volume
        masterVolume = volume;

        // Update music volume
        musicSource.volume = musicVolume * masterVolume;

        // Save the setting
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    // Adjust music volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume * masterVolume;

        // Save the setting
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    // Adjust sfx volume
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        // Save the setting
        PlayerPrefs.SetFloat("EffectVolume", volume);
        PlayerPrefs.Save();
    }

    // Adjust gather volume
    public void SetGatherVolume(float volume)
    {
        gatherVolume = volume;
        
        // Save the setting
        PlayerPrefs.SetFloat("GatherVolume", volume);
        PlayerPrefs.Save();
    }

    // Preload the next song
    public void PreloadSong()
    {
        // Get next index
        int nextIndex = GM.I.songIndex + 1;
        if (nextIndex >= GM.I.songs.Count)
            nextIndex = 0;

        // Get next song name
        string songName = GM.I.songs[nextIndex].myName;

        // Ensure the audio source is ready
        AudioSource source = musicSources[songName];
        
        // This forces Unity to load the audio data into memory
        source.Play();
        source.Pause();
        source.time = 0;
    }

    // Starts the song at the given index.
    // Sets songIndex, songTimer, and bpm.
    public void StartSong(int index)
    {
        // Set songIndex
        GM.I.songIndex = index;

        // Set songTimer
        GM.I.songTimer = GM.I.songs[index].duration;

        // Set bpm
        GM.I.bpm = GM.I.songs[index].bpm;

        // Play the audio file
        PlayMusic(GM.I.songs[index].myName);

        // Preload the next song?
        PreloadSong();
    }

    // Starts playing the next song.
    public void PlayNextSong()
    {
        // Increment song index
        IncrementSongIndex();

        // Play song
        StartSong(GM.I.songIndex);
    }

    // Small helper function to cleanly increment our song index.
    public void IncrementSongIndex()
    {
        // Increment
        GM.I.songIndex++;

        // Loop to 0 if it overflows
        if (GM.I.songIndex >= GM.I.songs.Count)
            GM.I.songIndex = 0;
    }
}