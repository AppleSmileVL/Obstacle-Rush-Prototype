using UnityEngine;
using UnityEngine.Events;


public enum RaceState
{
    Preparation,
    CountDown,
    Race,
    Passed
}

public class RaceStateTracker : MonoBehaviour, IDependency<TrackpointCircuit>
{
    public event UnityAction PreparationStarted;
    public event UnityAction Started;
    public event UnityAction Completed;
    public event UnityAction<TrackPoint> TrackPointPassed;
    public event UnityAction<int> LapCompleted;

    private TrackpointCircuit trackPointCircuit;
    public void Construct(TrackpointCircuit obj) => trackPointCircuit = obj;
   
    [SerializeField] private Timer countdowmTimer;
    [SerializeField] private int lapsToComplete;

    public Timer CountDownTimer => countdowmTimer;

    private RaceState state;
    public RaceState State => state;

    private void StartState(RaceState state)
    {
        this.state = state;
    }

    private void Start()
    {
        StartState(RaceState.Preparation);

        countdowmTimer.enabled = false;

        countdowmTimer.Finished += OnCountdowmTimerFinished;
        
        if (trackPointCircuit != null)
        {
            trackPointCircuit.TrackPointTriggered += OnTrackPointTriggered;
            trackPointCircuit.LapCompleted += OnLapCompleted;
        }
    }

    private void OnDestroy()
    {
        countdowmTimer.Finished -= OnCountdowmTimerFinished;
        
        if (trackPointCircuit != null)
        {
            trackPointCircuit.TrackPointTriggered -= OnTrackPointTriggered;
            trackPointCircuit.LapCompleted -= OnLapCompleted;
        }
    }

    private void OnTrackPointTriggered(TrackPoint trackPoint)
    {
        TrackPointPassed?.Invoke(trackPoint);
    }

    private void OnCountdowmTimerFinished()
    {
        StartRace();
    }

    private void OnLapCompleted(int lapAmount)
    {
        if (trackPointCircuit.Type == TrackType.Sprint)
        {
            CompleteRace();
        }

        if (trackPointCircuit.Type == TrackType.Circular)
        {
            if (lapAmount == lapsToComplete)
                CompleteRace();
            else
                CompleteLap(lapAmount);
        }
    }

    public void LaunchPreparationStarted()
    {
        if (state != RaceState.Preparation) return;

        StartState(RaceState.CountDown);

        countdowmTimer.enabled = true;
        PreparationStarted?.Invoke();
    }

    private void StartRace()
    {
        if (state != RaceState.CountDown) return;

        StartState(RaceState.Race);

        Started?.Invoke();
    }

    private void CompleteRace()
    {
        if (state != RaceState.Race) return;

        StartState(RaceState.Passed);

        Completed?.Invoke();
    }

    private void CompleteLap(int lapAmount)
    {
        LapCompleted?.Invoke(lapAmount);
    }
}
