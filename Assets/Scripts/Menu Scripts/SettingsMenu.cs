using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Tooltip("The games audio mixer.")]
    [SerializeField] private AudioMixer audioMixer = null;
    [Tooltip("The dropdown where resolution is changed.")]
    [SerializeField] private Dropdown resolutionsDropdown = null;

    //current monitor's resolutions
    Resolution[] resolutions;

    /// <summary>
    /// Collects current screens resolutions and stores them into
    /// list; adds them to resolution dropdown.
    /// </summary>
    private void Start()
    {
        //TODO: this is kind of ugly and will most definitely need to be changed when we add mixers for sfx and music
        Slider volumeSlider = GetComponentInChildren<Slider>();
        float currentVolume;

        audioMixer.GetFloat("Volume", out currentVolume);

        volumeSlider.value = currentVolume;


        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach(Resolution resolution in resolutions)
        {
            resolutionsDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        resolutionsDropdown.RefreshShownValue();
    }

    /// <summary>
    /// This sets the master volume.
    /// Min value is 60dB, max value is 0dB.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    /// <summary>
    /// Toggles fullscreen.
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// Refreshes the resolution to newest chosen resolution.
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionsDropdown.value].width, resolutions[resolutionsDropdown.value].height, Screen.fullScreen);
    }
}
