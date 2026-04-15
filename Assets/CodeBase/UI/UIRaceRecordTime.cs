using TMPro;
using UnityEngine;

public class UIRaceRecordTime : MonoBehaviour, IDependency<RaceResultTime>, IDependency<RaceStateTracker>
{
    [SerializeField] private GameObject goldRecordObject;
    [SerializeField] private GameObject playerRecordObject;
    [SerializeField] TextMeshProUGUI goldRecordTime;
    [SerializeField] TextMeshProUGUI playerRecordTime;

    private RaceResultTime raceResultTime;
    public void Construct(RaceResultTime obj) => raceResultTime = obj;

    private RaceStateTracker raceStateTracker;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    private void Start()
    {
        raceStateTracker.Started += OnRaceStart;
        raceStateTracker.Completed += OnRaceComplete;

        goldRecordObject.SetActive(false);
        playerRecordObject.SetActive(false);
    }

    private void OnDestroy()
    {
        raceStateTracker.Started -= OnRaceStart;
        raceStateTracker.Completed -= OnRaceComplete;
    }

    private void OnRaceStart()
    {
        if (raceResultTime.PlayerRecordTime > raceResultTime.GoldTime || raceResultTime.RecordWasSet == false)
        {
            goldRecordObject.SetActive (true);
            goldRecordTime.text = StringTime.SecondToTimeString(raceResultTime.GoldTime);
        }
        
        if(raceResultTime.RecordWasSet == true)
        {
            playerRecordObject.SetActive (true);
            playerRecordTime.text = StringTime.SecondToTimeString(raceResultTime.PlayerRecordTime);
        }
    }

    private void OnRaceComplete()
    {
        goldRecordObject?.SetActive(false);
        playerRecordObject?.SetActive(false);
    }
}
