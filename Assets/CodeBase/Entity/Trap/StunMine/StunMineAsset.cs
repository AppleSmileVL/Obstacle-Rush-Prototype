using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class StunMineAsset : ScriptableObject
{
    [Header("Stun Settings")]
    public float stunDuration = 2f;
    [Range(0f, 1f)]
    public float slowFactor = 0.2f;

    [Header("Audio and Effects")]
    public AudioClip stanSound;
    public GameObject explosionVFX;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup StunMineAudioMixerGroup;
}
