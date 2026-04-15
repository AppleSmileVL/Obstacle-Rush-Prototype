using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LaserGateController : MonoBehaviour
{
    [SerializeField] private LaserGateAsset asset;

    [SerializeField] private GameObject leftGate;
    [SerializeField] private GameObject rightGate;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (asset != null && asset.StunAudioMixerGroup != null)
            audioSource.outputAudioMixerGroup = asset.StunAudioMixerGroup;
    }

    private void Start()
    {
        if (asset == null) return;

        if (leftGate != null && rightGate != null)
        {
            SetGateActive(leftGate, false);
            SetGateActive(rightGate, false);
            StartCoroutine(AlternatingLaserCycle());
            return;
        }
    }

    private IEnumerator AlternatingLaserCycle()
    {
        while (true)
        {
            SetGateActive(leftGate, true);
            SetGateActive(rightGate, false);
            yield return new WaitForSeconds(asset.ActiveDuretion);

            SetGateActive(leftGate, false);
            SetGateActive(rightGate, false);
            yield return new WaitForSeconds(asset.InactiveDuretion);

            SetGateActive(leftGate, false);
            SetGateActive(rightGate, true);
            yield return new WaitForSeconds(asset.ActiveDuretion);

            SetGateActive(leftGate, false);
            SetGateActive(rightGate, false);
            yield return new WaitForSeconds(asset.InactiveDuretion);
        }
    }

    private void SetGateActive(GameObject gate, bool active)
    {
        if (gate == null) return;
        gate.SetActive(active);
    }

    private bool IsPartOfGate(Collider other, GameObject gate)
    {
        if (gate == null) return false;
        var t = other.transform;
        return t == gate.transform || t.IsChildOf(gate.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other, null);
    }

    private void HandleTrigger(Collider other, GameObject gate)
    {
        var interaction = other.GetComponentInParent<ICarInteraction>();
        var nitro = other.GetComponentInParent<Nitro>();

        bool applied = false;

        if (leftGate != null && rightGate != null)
        {
            if (gate != null)
            {
                if (gate == leftGate && leftGate.activeInHierarchy) applied = true;
                else if (gate == rightGate && rightGate.activeInHierarchy) applied = true;
            }
            else
            {
                if (IsPartOfGate(other, leftGate) && leftGate.activeInHierarchy) applied = true;
                else if (IsPartOfGate(other, rightGate) && rightGate.activeInHierarchy) applied = true;
            }
        }
        else
        {
            applied = true;
        }

        if (applied && interaction != null)
        {
            interaction.ApplyStun(asset.StunDuration, asset.SlowFactor);

            if (asset.StunSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(asset.StunSound);
            }
        }

        if (applied && nitro != null)
        {
            nitro.AddNitro(-nitro.MaxNitro);
        }
    }

    public void GateTriggered(Collider other, GameObject gate)
    {
        HandleTrigger(other, gate);
    }
}
