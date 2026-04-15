using UnityEngine;

public class NitroCarVFX : MonoBehaviour, IDependency<Nitro>
{
    [SerializeField] private ParticleSystem[] nitroFlames;
    private Nitro nitro;

    public void Construct(Nitro obj) => nitro = obj;

    private void Awake()
    {
        if (nitroFlames == null || nitroFlames.Length == 0)
            nitroFlames = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (nitro == null || nitroFlames == null || nitroFlames.Length == 0) return;

        bool active = nitro.IsNitroActive;

        foreach (var flamePS in nitroFlames)
        {
            if (flamePS == null) continue;

            var emission = flamePS.emission;
            if (emission.enabled == active) continue;

            emission.enabled = active;

            if (active)
            {
                if (!flamePS.isPlaying) flamePS.Play();
            }
            else
            {
                if (flamePS.isPlaying) flamePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
