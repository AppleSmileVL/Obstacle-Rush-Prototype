using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PauseAudioSource : MonoBehaviour, IDependency<Pauser>
{
    private new AudioSource audio;
    private Pauser pauser;
    public void Construct(Pauser obj) => pauser = obj;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        pauser.PauseStateChanged += OnPauseStateChanged;
    }

    private void OnDestroy()
    {
        pauser.PauseStateChanged -= OnPauseStateChanged;
    }

    private void OnPauseStateChanged(bool pause)
    {
        if (pause)
        {
            audio.Pause();
        }
        else
        {
            audio.UnPause();
        }

    }
}
