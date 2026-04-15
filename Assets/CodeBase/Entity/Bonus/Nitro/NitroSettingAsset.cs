using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class NitroSettingAsset : ScriptableObject
{
    [Header("Nitro Settings")]
    public float MaxNitro = 100f;
    public float ConsumptionRate = 20f;
    public float NitroForce = 500f;

    [Header("Audio")]
    public AudioClip NitroActivateSound;
    public AudioClip NitroPickupSound;

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup NitroPickupAudioMixerGroup;
    public AudioMixerGroup NitroActivateAudioMixerGroup;
}
