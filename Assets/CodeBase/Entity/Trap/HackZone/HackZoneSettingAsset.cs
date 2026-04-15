using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu] 
public class HackZoneSettingAsset : ScriptableObject
{
    [Header("Settings")]
    public float HackDuration = 4f;

    [Header("Audio / Visual")]
    public Color HackColor = Color.red;
    public AudioClip Hacksound;
    public GameObject HackVFX;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup HackAudioMixerGroup;
}
