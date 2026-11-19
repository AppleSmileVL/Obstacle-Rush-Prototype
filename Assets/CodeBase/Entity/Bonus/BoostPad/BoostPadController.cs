using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class BoostPadController : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] BoostPadAsset asset;
    [SerializeField] private GameObject boostEffect;

    [Header("Pooling")]
    [SerializeField] private int boostEffectPoolSize = 4;

    private AudioSource audioSource;
    private bool isPaused;

    private ObjectPool<GameObject> boostEffectPool;
    private Coroutine boostEffectReleaseCoroutine;
    private GameObject activeBoostEffect;

    private Pauser pauser;
    public void Construct(Pauser obj) => pauser = obj;

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

        if (boostEffect != null)
        {
            boostEffectPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(boostEffect),
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
                defaultCapacity: boostEffectPoolSize,
                maxSize: boostEffectPoolSize
            );
        }

        if (pauser != null)
        {
            pauser.PauseStateChanged += OnPauseStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (pauser != null)
        {
            pauser.PauseStateChanged -= OnPauseStateChanged;
        }

        if (boostEffectReleaseCoroutine != null) StopCoroutine(boostEffectReleaseCoroutine);

        boostEffectPool?.Dispose();
    }

    private void OnPauseStateChanged(bool pause)
    {
        isPaused = pause;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPaused) return;

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

            if (boostEffect != null)
            {
                SpawnBoostEffect();
            }
        }
    }

    private void SpawnBoostEffect()
    {
        var spawned = boostEffectPool.Get();
        activeBoostEffect = spawned;
        spawned.transform.position = transform.position;
        spawned.transform.rotation = transform.rotation;

        var systems = spawned.GetComponentsInChildren<ParticleSystem>(true);
        float maxLifetime = 0f;

        foreach (var ps in systems)
        {
            if (ps == null) continue;

            var main = ps.main;
            main.loop = false;

            var emission = ps.emission;
            emission.enabled = true;
            ps.Play();

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

        if (boostEffectReleaseCoroutine != null) StopCoroutine(boostEffectReleaseCoroutine);
        boostEffectReleaseCoroutine = StartCoroutine(ReleaseBoostEffect(spawned, maxLifetime + 0.1f));
    }

    private IEnumerator ReleaseBoostEffect(GameObject spawned, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (activeBoostEffect == spawned)
        {
            activeBoostEffect = null;
            boostEffectPool.Release(spawned);
        }
        boostEffectReleaseCoroutine = null;
    }
}
