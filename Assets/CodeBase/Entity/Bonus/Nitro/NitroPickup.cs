using System.Collections;
using UnityEngine;

public class NitroPickup : MonoBehaviour
{
    [SerializeField] private NitroSettingAsset asset;
    [SerializeField] private float nitroAmount = 30f;
    [SerializeField] private GameObject pickupEffect;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 10f;
    [SerializeField] private bool destroyAfterPickup = false;

    private Collider pickupCollider;
    private Renderer[] renderers;
    private bool isPickedUp;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;

        var nitroSystem = other.GetComponentInParent<Nitro>();
        if (nitroSystem == null) return;

        nitroSystem.AddNitro(nitroAmount);

        if (pickupEffect != null)
            SpawnPickupEffect();

        PlayPickupSound();

        if (destroyAfterPickup)
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);
            return;
        }

        StartCoroutine(HandlePickupAndRespawn());
    }

    private IEnumerator HandlePickupAndRespawn()
    {
        isPickedUp = true;
        SetVisible(false);

        if (pickupCollider != null) pickupCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        SetVisible(true);
        if (pickupCollider != null) pickupCollider.enabled = true;
        isPickedUp = false;
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null) return;
        foreach (var render in renderers)
            if (render != null) render.enabled = visible;
    }

    private void SpawnPickupEffect()
    {
        var spawned = Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (!spawned.activeSelf) spawned.SetActive(true);

        var systems = spawned.GetComponentsInChildren<ParticleSystem>(true);
        float maxLifetime = 0f; 

        foreach (var partical in systems) 
        {
            if (partical == null) continue;

            var main = partical.main;
            main.loop = false;

            var emission = partical.emission;
            emission.enabled = true;
            partical.Play();

            float startLifetime = 0f;
            var startLive = main.startLifetime;
            switch (startLive.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    startLifetime = startLive.constant;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    startLifetime = (startLive.constantMin + startLive.constantMax) * 0.5f;
                    break;
                default:
                    startLifetime = main.duration;
                    break;
            }

            float lifetime = main.duration + startLifetime;
            if (lifetime > maxLifetime) maxLifetime = lifetime;
        }

        if (maxLifetime <= 0f) maxLifetime = 5f;
        Destroy(spawned, maxLifetime + 0.1f);
    }

    private void PlayPickupSound()
    {
        if (asset == null || asset.NitroPickupSound == null) return;

        var clip = asset.NitroPickupSound;
        var go = new GameObject("NitroPickup_SFX");
        go.transform.position = transform.position;
        var audio = go.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.playOnAwake = false;
        audio.volume = 1f;
        audio.pitch = 1f;
        audio.spatialBlend = 0f;
        audio.dopplerLevel = 0f;
        audio.rolloffMode = AudioRolloffMode.Logarithmic;

        if (asset.NitroPickupAudioMixerGroup != null)
            audio.outputAudioMixerGroup = asset.NitroPickupAudioMixerGroup;

        audio.Play();
        Destroy(go, clip.length + 0.1f);
    }
}