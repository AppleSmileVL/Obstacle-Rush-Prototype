using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class BoostPadController : MonoBehaviour
{
    [SerializeField] BoostPadAsset asset;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (asset != null && asset.boostPadAudioMixerGroup != null)
            audioSource.outputAudioMixerGroup = asset.boostPadAudioMixerGroup;

        var collider = GetComponent<Collider>();
        if (collider != null && !collider.isTrigger)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        var interaction = other.GetComponent<ICarInteraction>();
        if (interaction == null)
        {
            interaction = other.GetComponentInParent<ICarInteraction>();
        }

        if (interaction != null)
        {
           
            interaction.ApplyImpulse(transform.forward, asset.boostForce, asset.forceMode);

            
            if (asset.boostSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(asset.boostSound);
            }
        }
    }
}
