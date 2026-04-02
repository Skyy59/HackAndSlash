using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataBase", menuName = "Game/Audio/AudioDataBase")]
public class AudioDataBase : ScriptableObject
{
    [SerializeField] private AudioData[] musicData;

    [SerializeField] private AudioData[] sfxData;

    public AudioData GetAudioData(string _dataKey)
    {
        for (int _i = 0; _i < musicData.Length; _i++)
        {
            if (musicData[_i].audioName == _dataKey) return musicData[_i];
        }

        for (int _i = 0; _i < sfxData.Length; _i++)
        {
            if (sfxData[_i].audioName == _dataKey) return sfxData[_i];
        }

        return null;
    }

    public AudioData GetMusicWithName(string _dataKey)
    {
        for (int _i = 0; _i < musicData.Length; _i++)
        {
            if (musicData[_i].audioName == _dataKey) return musicData[_i];
        }

        return null;
    }

    public AudioData GetSoundWithName(string _dataKey)
    {
        for (int _i = 0; _i < sfxData.Length; _i++)
        {
            if (sfxData[_i].audioName == _dataKey) return sfxData[_i];
        }

        return null;
    }
}
