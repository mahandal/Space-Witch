using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingSlider : MonoBehaviour
{
   [Header("What For?")]
   public string whatFor = "";

   [Header("Slider")]
   public Slider slider;

   // + Initialization
   public void Initialize()
   {
        // Get remembered value, with 0.5f as the default.
        slider.value = PlayerPrefs.GetFloat(whatFor, 0.5f);

        // Set volume.
        B_SetVolume();
   }

   // + Buttons
   // Set the volume for a channel.
   public void B_SetVolume()
   {
        // SFX
        DJ.I.PlaySound("Tick", whatFor);

        // Read slider.
        float volume = slider.value;

        // Scale volume from 0 - 1, as the slider reads it, to -80 - 20 for decibels.
        Settings.I.mixer.SetFloat(whatFor, Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);

        // Remember in player prefs.
        PlayerPrefs.SetFloat(whatFor, volume);
        PlayerPrefs.Save();
   }
}
