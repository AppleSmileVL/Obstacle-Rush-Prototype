using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class LaserGateAsset : ScriptableObject
{
    [Header("Cycle Settings")]
    public float ActiveDuretion = 4f;
    public float InactiveDuretion = 2f;

    [Header("Stun Settings")]
    public float StunDuration = 2f;
    public float SlowFactor = 0.2f;

    [Header("Audio")]
    public AudioClip StunSound;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup StunAudioMixerGroup;
}
