using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Data", menuName = "Game/New Audio Data")]
public class AudioData : ScriptableObject
{
    public AudioClip clip;
    public string audioName;
    [Range(0f, 1f)]
    public float volume;
}
