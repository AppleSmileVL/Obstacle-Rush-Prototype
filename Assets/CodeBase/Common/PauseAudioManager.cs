using UnityEngine;
using UnityEngine.Audio;

public class PauseAudioManager : MonoBehaviour, IDependency<Pauser>, IDependency<RaceStateTracker>
{
    [SerializeField] private AudioMixerSnapshot pausedSnapshot;
    [SerializeField] private AudioMixerSnapshot unpausedSnapshot;
    [SerializeField] private AudioMixerSnapshot raceMutedSnapshot;
    [SerializeField] private float transitionTime = 0.0f;

    private Pauser pauser;
    private RaceStateTracker raceStateTracker;

    public void Construct(Pauser obj) => pauser = obj;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    private void Start()
    {
        if (pauser == null)
            pauser = FindObjectOfType<Pauser>();

        if (pauser != null)
            pauser.PauseStateChanged += OnPauseChanged;

        // если DI не вызвали, пробуем fallback
        if (raceStateTracker == null)
            raceStateTracker = FindObjectOfType<RaceStateTracker>();

        if (raceStateTracker != null)
        {
            raceStateTracker.PreparationStarted += OnRacePreparationStarted;
            raceStateTracker.Started += OnRaceStarted;
            raceStateTracker.Completed += OnRaceCompleted;
        }
    }

    private void OnDestroy()
    {
        if (pauser != null)
            pauser.PauseStateChanged -= OnPauseChanged;

        if (raceStateTracker != null)
        {
            raceStateTracker.PreparationStarted -= OnRacePreparationStarted;
            raceStateTracker.Started -= OnRaceStarted;
            raceStateTracker.Completed -= OnRaceCompleted;
        }
    }

    private void OnPauseChanged(bool paused)
    {
        if (paused)
        {
            if (pausedSnapshot != null) pausedSnapshot.TransitionTo(transitionTime);
        }
        else
        {
            if (unpausedSnapshot != null) unpausedSnapshot.TransitionTo(transitionTime);
        }
    }

    private void OnRacePreparationStarted()
    {
        if (raceMutedSnapshot != null) raceMutedSnapshot.TransitionTo(transitionTime);
    }

    private void OnRaceStarted()
    {
        if (raceMutedSnapshot != null) raceMutedSnapshot.TransitionTo(transitionTime);
    }

    private void OnRaceCompleted()
    {
        if (unpausedSnapshot != null) unpausedSnapshot.TransitionTo(transitionTime);
    }
}
