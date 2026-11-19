using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class NitroPickup : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] private NitroSettingAsset asset;
    [SerializeField] private float nitroAmount = 30f;
    [SerializeField] private GameObject pickupEffect;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 10f;
    [SerializeField] private bool destroyAfterPickup = false;

    [Header("Pooling")]
    [SerializeField] private int pickupEffectPoolSize = 4;
    [SerializeField] private int nitroPickupSfxPoolSize = 2;

    private Collider pickupCollider;
    private Renderer[] renderers;
    private bool isPickedUp;
    private bool isPaused;

    private ObjectPool<GameObject> pickupEffectPool;
    private ObjectPool<GameObject> nitroPickupSfxPool;
    private Coroutine pickupEffectReleaseCoroutine;
    private Coroutine nitroPickupSfxReleaseCoroutine;
    private GameObject activePickupEffect;
    private GameObject activeNitroPickupSfx;

    private Pauser pauser;
    public void Construct(Pauser obj) => pauser = obj;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>(true);

        pickupEffectPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(pickupEffect),
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                var systems = obj.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var ps in systems)
                {
                    if (ps == null) continue;
                    ps.Stop();
                }
            },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: pickupEffectPoolSize,
            maxSize: pickupEffectPoolSize
        );

        nitroPickupSfxPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var go = new GameObject("NitroPickup_SFX");
                go.AddComponent<AudioSource>();
                return go;
            },
            actionOnGet: (obj) =>
            {
                var audio = obj.GetComponent<AudioSource>();
                if (audio == null) return;

                audio.clip = asset != null ? asset.NitroPickupSound : null;
                audio.playOnAwake = false;
                audio.volume = 1f;
                audio.pitch = 1f;
                audio.spatialBlend = 0f;
                audio.dopplerLevel = 0f;
                audio.rolloffMode = AudioRolloffMode.Logarithmic;
                if (asset != null && asset.NitroPickupAudioMixerGroup != null)
                    audio.outputAudioMixerGroup = asset.NitroPickupAudioMixerGroup;

                obj.SetActive(true);
                audio.Play();
            },
            actionOnRelease: (obj) =>
            {
                var audio = obj.GetComponent<AudioSource>();
                if (audio != null) audio.Stop();
                obj.SetActive(false);
            },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: nitroPickupSfxPoolSize,
            maxSize: nitroPickupSfxPoolSize
        );

        if (pauser != null)
        {
            pauser.PauseStateChanged += OnPauseStateChanged;
        }
    }

    private void OnDisable()
    {
        if (pickupEffectReleaseCoroutine != null)
        {
            StopCoroutine(pickupEffectReleaseCoroutine);
            pickupEffectReleaseCoroutine = null;
        }

        if (nitroPickupSfxReleaseCoroutine != null)
        {
            StopCoroutine(nitroPickupSfxReleaseCoroutine);
            nitroPickupSfxReleaseCoroutine = null;
        }

        if (activePickupEffect != null)
        {
            pickupEffectPool.Release(activePickupEffect);
            activePickupEffect = null;
        }

        if (activeNitroPickupSfx != null)
        {
            nitroPickupSfxPool.Release(activeNitroPickupSfx);
            activeNitroPickupSfx = null;
        }
    }

    private void OnDestroy()
    {
        if (pauser != null)
        {
            pauser.PauseStateChanged -= OnPauseStateChanged;
        }

        if (pickupEffectReleaseCoroutine != null) StopCoroutine(pickupEffectReleaseCoroutine);
        if (nitroPickupSfxReleaseCoroutine != null) StopCoroutine(nitroPickupSfxReleaseCoroutine);

        pickupEffectPool?.Dispose();
        nitroPickupSfxPool?.Dispose();
    }

    private void OnPauseStateChanged(bool pause)
    {
        isPaused = pause;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (isPaused) return;

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
        var spawned = pickupEffectPool.Get();
        activePickupEffect = spawned;
        spawned.transform.position = transform.position;

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

        if (pickupEffectReleaseCoroutine != null) StopCoroutine(pickupEffectReleaseCoroutine);
        pickupEffectReleaseCoroutine = StartCoroutine(ReleasePickupEffect(spawned, maxLifetime + 0.1f));
    }

    private IEnumerator ReleasePickupEffect(GameObject spawned, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (activePickupEffect == spawned)
        {
            activePickupEffect = null;
            pickupEffectPool.Release(spawned);
        }
        pickupEffectReleaseCoroutine = null;
    }

    private void PlayPickupSound()
    {
        if (asset == null || asset.NitroPickupSound == null) return;

        var clip = asset.NitroPickupSound;
        var go = nitroPickupSfxPool.Get();
        activeNitroPickupSfx = go;

        if (nitroPickupSfxReleaseCoroutine != null) StopCoroutine(nitroPickupSfxReleaseCoroutine);
        nitroPickupSfxReleaseCoroutine = StartCoroutine(ReleasePickupSound(go, clip.length + 0.1f));
    }

    private IEnumerator ReleasePickupSound(GameObject go, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (activeNitroPickupSfx == go)
        {
            activeNitroPickupSfx = null;
            nitroPickupSfxPool.Release(go);
        }
        nitroPickupSfxReleaseCoroutine = null;
    }
}