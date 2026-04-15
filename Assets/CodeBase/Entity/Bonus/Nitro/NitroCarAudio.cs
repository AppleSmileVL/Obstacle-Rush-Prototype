using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NitroCarAudio : MonoBehaviour, IDependency<Nitro>
{
    [SerializeField] private NitroSettingAsset asset;

    private AudioSource audioSource;
    private Nitro nitro;

    public void Construct(Nitro obj) => nitro = obj;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = asset.NitroActivateSound;

        if (asset != null)
        {
            if (asset.NitroActivateAudioMixerGroup != null)
                audioSource.outputAudioMixerGroup = asset.NitroActivateAudioMixerGroup;
        }
    }

    private void Update()
    {
        if (nitro == null) return;

        if (nitro.IsNitroActive && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!nitro.IsNitroActive && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
