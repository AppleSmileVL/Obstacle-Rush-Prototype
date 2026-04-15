using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class StunMineController : MonoBehaviour
{
    [SerializeField] private StunMineAsset asset;
    [SerializeField] private bool destroyOnTrigger = true;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 10f;

    private Collider mineCollider;
    private Renderer[] renderers;
    private bool isTriggered;

    private void Awake()
    {
        mineCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        var interaction = other.GetComponentInParent<ICarInteraction>();
        if (interaction == null) return;

        interaction.ApplyStun(asset.stunDuration, asset.slowFactor);

        ExecuteEffects();

        if (destroyOnTrigger)
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 1.0f);
            return;
        }

        StartCoroutine(HandleTriggerAndRespawn());
    }

    private IEnumerator HandleTriggerAndRespawn()
    {
        isTriggered = true;

        SetVisible(false);
        if (mineCollider != null) mineCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        SetVisible(true);
        if (mineCollider != null) mineCollider.enabled = true;
        isTriggered = false;
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            if (r != null) r.enabled = visible;
    }

    private void ExecuteEffects()
    {
        if (asset.explosionVFX != null)
        {
            Instantiate(asset.explosionVFX, transform.position, Quaternion.identity);
        }

        if (asset != null && asset.stanSound != null)
        {
            var clip = asset.stanSound;
            var go = new GameObject("StanSound_SFX");
            var audio = go.AddComponent<AudioSource>();
            audio.clip = clip;
            audio.playOnAwake = false;
            audio.volume = 1f;
            audio.pitch = 1f;
            audio.spatialBlend = 0f;
            audio.dopplerLevel = 0f;
            audio.rolloffMode = AudioRolloffMode.Logarithmic;

            if (asset.StunMineAudioMixerGroup != null)
                audio.outputAudioMixerGroup = asset.StunMineAudioMixerGroup;

            audio.Play();
            Destroy(go, clip.length + 0.1f);
        }
    }
}
