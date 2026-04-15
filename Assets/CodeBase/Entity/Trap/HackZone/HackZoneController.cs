using UnityEngine;

public class HackZoneController : MonoBehaviour
{
    [SerializeField] private HackZoneSettingAsset asset;

    private void OnTriggerEnter(Collider other)
    {
        var interaction = other.GetComponentInParent<ICarInteraction>();

        if (interaction != null)
        {
            interaction.ApplyHack(asset.HackDuration);

            if (asset.HackVFX != null)
            {
                Instantiate(asset.HackVFX, other.transform.position, Quaternion.identity);
            }

            if (asset.Hacksound != null)
            {
                var go = new GameObject("Hack_SFX");
                go.transform.position = other.transform.position;
                var audio = go.AddComponent<AudioSource>();
                audio.clip = asset.Hacksound;
                audio.spatialBlend = 0f;
                if (asset.HackAudioMixerGroup != null)
                    audio.outputAudioMixerGroup = asset.HackAudioMixerGroup;
                audio.Play();
                Destroy(go, asset.Hacksound.length + 0.1f);
            }
        }
    }
}
