using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class CarAudio : MonoBehaviour
{
    public enum EngineAudioOptions { Simple, FourChannel }

    public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;
    public AudioClip lowAccelClip;
    public AudioClip lowDecelClip;
    public AudioClip highAccelClip;
    public AudioClip highDecelClip;
    public float pitchMultiplier = 1f;
    public float lowPitchMin = 1f;
    public float lowPitchMax = 6f;
    public float highPitchMultiplier = 0.25f;
    public float maxRolloffDistance = 500;
    public float dopplerLevel = 1;
    public bool useDoppler = true;
    public AudioMixerGroup sfxMixerGroup;

    [SerializeField] private Car car;

    private AudioSource lowAccel;
    private AudioSource lowDecel;
    private AudioSource highAccel;
    private AudioSource highDecel;
    private bool startedSound;

    private void Start()
    {
        StartSound();
    }

    private void OnEnable()
    {
        StartSound();
    }

    private void OnDisable()
    {
        StopSound();
    }

    private void StartSound()
    {
        if (startedSound) return;

        lowAccel = SetUpEngineAudioSource(lowAccelClip);
        lowDecel = SetUpEngineAudioSource(lowDecelClip);
        highAccel = SetUpEngineAudioSource(highAccelClip);
        highDecel = SetUpEngineAudioSource(highDecelClip);

        startedSound = true;
    }

    private void StopSound()
    {
        if (!startedSound) return;

        if (lowAccel) Destroy(lowAccel.gameObject);
        if (lowDecel) Destroy(lowDecel.gameObject);
        if (highAccel) Destroy(highAccel.gameObject);
        if (highDecel) Destroy(highDecel.gameObject);

        startedSound = false;
    }

    private void Update()
    {
        if (!startedSound || car == null) return;

        if (lowAccel == null) return;

        float engineRpm = car.EngineRpm;
        float rpmFactor = engineRpm / car.EngineMaxRpm;

        float lowPitch = Mathf.Lerp(lowPitchMin, lowPitchMax, rpmFactor);
        lowPitch = Mathf.Min(lowPitchMax, lowPitch);

        if (engineSoundStyle == EngineAudioOptions.FourChannel)
        {
            float highFade = Mathf.InverseLerp(0.2f, 0.8f, rpmFactor);
            float lowFade = 1 - highFade;

            highFade = 1 - ((1 - highFade) * (1 - highFade));
            lowFade = 1 - ((1 - lowFade) * (1 - lowFade));

            float accFade = Mathf.Abs(car.ThrottleControl);
            float decFade = 1 - accFade;

            lowAccel.volume = lowFade * accFade;
            lowDecel.volume = lowFade * decFade;
            highAccel.volume = highFade * accFade;
            highDecel.volume = highFade * decFade;

            lowAccel.pitch = lowPitch * pitchMultiplier;
            lowDecel.pitch = lowPitch * pitchMultiplier;
            highAccel.pitch = lowPitch * highPitchMultiplier * pitchMultiplier;
            highDecel.pitch = lowPitch * highPitchMultiplier * pitchMultiplier;

            ApplyDoppler(lowAccel);
            ApplyDoppler(lowDecel);
            ApplyDoppler(highAccel);
            ApplyDoppler(highDecel);
        }
    }

    private void ApplyDoppler(AudioSource source)
    {
        if (source != null)
            source.dopplerLevel = useDoppler ? dopplerLevel : 0;
    }

    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        if (clip == null) return null;

        GameObject go = new GameObject("EngineAudio_" + clip.name);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;
        source.spatialBlend = 0f;

        source.time = Random.Range(0f, clip.length);
        source.minDistance = 5;
        source.maxDistance = maxRolloffDistance;
        source.outputAudioMixerGroup = sfxMixerGroup;

        source.Play();
        return source;
    }
}