using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio_Controller : MonoBehaviour
{
    public static Audio_Controller Instance;
    
    [SerializeField] private AudioDataBase audioDataBase;
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer gameMixer;
    [SerializeField] private string masterString;
    [SerializeField] private string musicString;
    [SerializeField] private string sfxString;

    [Header("Audio Groups")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private List<AudioSource> sfxASList = new List<AudioSource>();
    private List<AudioSource> musicASList = new List<AudioSource>();
    private List<AudioSource> voicesASList = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start() 
    {
        SetMixerValues();
    }

    private void Initialize()
    {
        // Initialize AudioSources
        // 1- Create Empty objects to store AudioSources
        // Main
        GameObject mainGO = new GameObject("Audio");
        mainGO.transform.parent = transform;
        // SFX, Ambience and Music
        GameObject sfxGO = new GameObject("SFXASs");
        sfxGO.transform.SetParent(mainGO.transform);
        GameObject musicGO = new GameObject("MusicASs");
        musicGO.transform.SetParent(mainGO.transform);

        //2- Create an AudioSource each type of sound (sfx, ambience and music) with the dessired configuration and store the reference.
        sfxASList = new List<AudioSource>
        {
            sfxGO.AddComponent<AudioSource>()
        };
        
        musicASList = new List<AudioSource>();
        AudioSource musicAS = musicGO.AddComponent<AudioSource>();
        musicAS.loop = true;
        musicASList.Add(musicAS);

        //3 - Assign AudioMixerGroups to each type of ASource.
        foreach (AudioSource aSource in sfxASList) AssignAudioGroup(aSource, sfxMixerGroup);

        foreach (AudioSource aSource in musicASList) AssignAudioGroup(aSource, musicMixerGroup);
    }

    public void PlaySFX(string name, float delay) => StartCoroutine(PlaySFXWithDelayCoroutine(name, delay));
    
    public void PlaySFX(string _sfxName)
    {
        AudioData _sfx = audioDataBase.GetSoundWithName(_sfxName);
        if (_sfx.clip == null) return;
        PlaySFX(_sfx);
    }

    public void PlaySFX(AudioData _sfxAudioData)
    {
        AudioSource audioSource = sfxASList[0];
        audioSource.PlayOneShot(_sfxAudioData.clip, _sfxAudioData.volume);
    }

    private IEnumerator PlaySFXWithDelayCoroutine(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(name);
    }

    public void PlayMusic(string _musicName)
    {
        AudioData _music = audioDataBase.GetMusicWithName(_musicName);

        if (_music.clip == null) return;

        AudioSource _audioSource = musicASList[0];
        _audioSource.clip = _music.clip;
        _audioSource.volume = _music.volume;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        AudioSource _audioSource = musicASList[0];
        if (_audioSource.isPlaying) _audioSource.Stop();
    }

    public AudioData GetAudioData(string _audioKey)
    {
        return audioDataBase.GetAudioData(_audioKey);
    }

    public void SetMixerValues()
    {
        PlayerPrefs.SetFloat("Master Volume", PlayerPrefs.HasKey("Master Volume") ? PlayerPrefs.GetFloat("Master Volume") : 0.5f);
        PlayerPrefs.SetFloat("Music Volume", PlayerPrefs.HasKey("Music Volume") ? PlayerPrefs.GetFloat("Music Volume") : 0.5f);
        PlayerPrefs.SetFloat("SFX Volume", PlayerPrefs.HasKey("SFX Volume") ? PlayerPrefs.GetFloat("SFX Volume") : 0.5f);
        
        ChangeMasterVolume(PlayerPrefs.GetFloat("Master Volume"));
        ChangeMusicVolume(PlayerPrefs.GetFloat("Music Volume"));
        ChangeSFXVolume(PlayerPrefs.GetFloat("SFX Volume"));

        PlayerPrefs.Save();
    }

    private void AssignAudioGroup(AudioSource _audioSource, AudioMixerGroup _mixerGroup)
    {
        _audioSource.outputAudioMixerGroup = _mixerGroup;
    }

    private float AudioConversion(float _volume)
    {
        return _volume <= 0.0001f ? -80f : Mathf.Log10(_volume) * 20f;
    }

    public void ChangeMasterVolume(float _volume)
    {
        gameMixer.SetFloat(masterString, AudioConversion(_volume));
    }

    public void ChangeMusicVolume(float _volume)
    {
        gameMixer.SetFloat(musicString, AudioConversion(_volume));
    }

    public void ChangeSFXVolume(float _volume)
    {
        gameMixer.SetFloat(sfxString, AudioConversion(_volume));
    }
}
