using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : PanelUI
{
    [Space(10)]
    [Header("Settings")]
    [Space(5)]

    [Header("Volumes")]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    private bool _changing = false;

    private void Start() 
    {
        _changing = true;

        masterVolume.value = PlayerPrefs.HasKey("Master Volume") ? PlayerPrefs.GetFloat("Master Volume") : 0.5f;
        musicVolume.value = PlayerPrefs.HasKey("Music Volume") ? PlayerPrefs.GetFloat("Music Volume") : 0.5f;
        sfxVolume.value = PlayerPrefs.HasKey("SFX Volume") ? PlayerPrefs.GetFloat("SFX Volume") : 0.5f;

        _changing = false;
    }

    public void SetMasterVolume()
    {
        if (_changing) return;
        PlayerPrefs.SetFloat("Master Volume", masterVolume.value);
        Audio_Controller.Instance.ChangeMasterVolume(masterVolume.value);
    }

    public void SetMusicVolume()
    {
        if (_changing) return;
        PlayerPrefs.SetFloat("Music Volume", musicVolume.value);
        Audio_Controller.Instance.ChangeMusicVolume(musicVolume.value);
    }

    public void SetSFXVolume()
    {
        if (_changing) return;
        PlayerPrefs.SetFloat("SFX Volume", sfxVolume.value);
        Audio_Controller.Instance.ChangeSFXVolume(sfxVolume.value);
    }
}
