using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RaceResultTime : MonoBehaviour, IDependency<RaceStateTracker>, IDependency<RaceTimeTracker>
{
    public const string SaveMark = "_player_bst_time";

    public event UnityAction ResultUpdated;

    [SerializeField] private RaceInfo currentRaceInfo;

    private float playerRecordTime;
    private float currentTime;

    public float GoldTime => currentRaceInfo.GoldTime;
    public float PlayerRecordTime => playerRecordTime;
    public float CurrentTime => currentTime;

    public bool RecordWasSet => playerRecordTime != 0;

    private RaceStateTracker raceStateTracker;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    private RaceTimeTracker raceTimeTracker;
    public void Construct(RaceTimeTracker obj) => raceTimeTracker = obj;

    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        raceStateTracker.Completed += OnRaceCompleted;
    }

    private void OnDestroy()
    {
        raceStateTracker.Completed -= OnRaceCompleted;
    }

    private void OnRaceCompleted()
    {
        float absoluteRecord = GetAbsoluteRecord();

        if (raceTimeTracker.CurrentTime < absoluteRecord || playerRecordTime == 0)
        {
            playerRecordTime = raceTimeTracker.CurrentTime;

            Save();
        }

        currentTime = raceTimeTracker.CurrentTime;

        ResultUpdated?.Invoke();
    }

    public float GetAbsoluteRecord()
    {
        if (playerRecordTime < GoldTime && playerRecordTime != 0)
            return playerRecordTime;
        else
            return GoldTime;
    }

    private void Load()
    {
        playerRecordTime = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + SaveMark, 0);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + SaveMark, playerRecordTime);
    }
}
