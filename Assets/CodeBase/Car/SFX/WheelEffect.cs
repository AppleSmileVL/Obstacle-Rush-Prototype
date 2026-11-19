using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class WheelEffect : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] private WheelCollider[] wheels;
    [SerializeField] private ParticleSystem[] wheelsSmoke;
 
    [SerializeField] private float forwardSlipLimit;
    [SerializeField] private float sidewaysSlipLimit;

    [SerializeField] private new AudioSource audio;

    [SerializeField] private GameObject skidPrefab;

    private WheelHit wheelHit;
    private Transform[] skidTrail;
    private bool isPaused;
    private bool startedSound;
    private ObjectPool<GameObject> skidTrailPool;
    private bool[] hasActiveTrail;
    private Coroutine[] skidFades;

    private Pauser pauser;
    public void Construct(Pauser obj) => pauser = obj;

    private void Start()
    {
        skidTrail = new Transform[wheels.Length];
        hasActiveTrail = new bool[wheels.Length];
        skidFades = new Coroutine[wheels.Length];

        skidTrailPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(skidPrefab),
            actionOnGet: (obj) =>
            {
                var trail = obj.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.Clear();
                    trail.emitting = true;
                }
                obj.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(null);
            },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: wheels.Length,
            maxSize: wheels.Length * 2
        );

        if (pauser != null)
        {
            pauser.PauseStateChanged += OnPauseStateChanged;
        }
    }

    private void OnDisable()
    {
        ReturnActiveTrailsToPool();
    }

    private void OnDestroy()
    {
        if (pauser != null)
        {
            pauser.PauseStateChanged -= OnPauseStateChanged;
        }
        ReturnActiveTrailsToPool();
        skidTrailPool?.Dispose();
    }

    private void OnPauseStateChanged(bool pause)
    {
        isPaused = pause;
        if (startedSound)
        {
            if (isPaused)
            {
                PauseAllSounds();
                PauseActiveTrails();
            }
            else
            {
                ResumeAllSounds();
                ResumeActiveTrails();
            }
        }
    }

    private void PauseActiveTrails()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (hasActiveTrail[i] && skidTrail[i] != null)
            {
                var trail = skidTrail[i].GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.emitting = false;
                }
                var ps = skidTrail[i].GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Pause();
                }
            }
        }
    }

    private void PauseAllSounds()
    {
        if (audio != null && audio.isPlaying)
        {
            audio.Pause();
        }
    }

    private void ResumeAllSounds()
    {
        if (audio != null && !audio.isPlaying)
        {
            audio.UnPause();
        }
    }

    private void ResumeActiveTrails()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (hasActiveTrail[i] && skidTrail[i] != null)
            {
                var trail = skidTrail[i].GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.emitting = true;
                }
                var ps = skidTrail[i].GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                }
            }
        }
    }

    private void ReturnActiveTrailsToPool()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (hasActiveTrail[i] && skidTrail[i] != null)
            {
                if (skidFades[i] != null)
                {
                    StopCoroutine(skidFades[i]);
                    skidFades[i] = null;
                }
                skidTrailPool?.Release(skidTrail[i].gameObject);
                skidTrail[i] = null;
                hasActiveTrail[i] = false;
            }
        }
    }

    private void StartTrailFade(int wheelIndex)
    {
        if (skidFades[wheelIndex] != null)
        {
            StopCoroutine(skidFades[wheelIndex]);
        }

        skidFades[wheelIndex] = StartCoroutine(FadeOutTrail(wheelIndex));
    }

    private IEnumerator FadeOutTrail(int wheelIndex)
    {
        if (skidTrail[wheelIndex] != null)
        {
            GameObject trailObj = skidTrail[wheelIndex].gameObject;
            TrailRenderer trail = skidTrail[wheelIndex].GetComponent<TrailRenderer>();

            if (trail != null)
            {
                yield return new WaitForSecondsRealtime(trail.time);
            }

            skidFades[wheelIndex] = null;
            skidTrailPool?.Release(trailObj);
        }

        skidTrail[wheelIndex] = null;
        hasActiveTrail[wheelIndex] = false;
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        bool isSlip = false;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetGroundHit(out wheelHit);

            if (wheels[i].isGrounded == true)
            {
                if (wheelHit.forwardSlip > forwardSlipLimit || wheelHit.sidewaysSlip > sidewaysSlipLimit)
                {
                    if (!hasActiveTrail[i])
                    {
                        GameObject trailObj = skidTrailPool.Get();
                        skidTrail[i] = trailObj.transform;
                        hasActiveTrail[i] = true;
                    }

                    if (audio.isPlaying == false)
                    {
                        audio.Play();
                        startedSound = true;
                    }

                    if (hasActiveTrail[i] && skidTrail[i] != null)
                    {
                        skidTrail[i].position = wheels[i].transform.position - wheelHit.normal * wheels[i].radius;
                        skidTrail[i].forward = -wheelHit.normal;

                        wheelsSmoke[i].transform.position = skidTrail[i].position;
                        wheelsSmoke[i].Emit(1);
                    }

                    isSlip = true;

                    continue;
                }
            }

            if (hasActiveTrail[i])
            {
                StartTrailFade(i);
                wheelsSmoke[i].Stop();
            }
        }

        if (isSlip == false)
        {
            audio.Stop();
            startedSound = false;
        }
    }
}
