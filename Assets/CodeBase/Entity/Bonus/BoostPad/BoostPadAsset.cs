using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class BoostPadAsset : ScriptableObject
{
    [Header("Force Settings")]
    public float boostForce = 50f;
    public ForceMode forceMode = ForceMode.VelocityChange; 

    [Header("Sound Effect")]
    public AudioClip boostSound;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup boostPadAudioMixerGroup;
}
