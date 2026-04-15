using TMPro;
using UnityEngine;

public class UIStartEnter : MonoBehaviour, IDependency<RaceStateTracker>
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] GameObject title;

    private RaceStateTracker raceStateTracker;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    private void Start()
    {
        raceStateTracker.PreparationStarted += OnRaceStarted;

        text.enabled = true;
        title.SetActive(true);
        enabled = true;
    }
    private void OnDestroy()
    {
        raceStateTracker.PreparationStarted -= OnRaceStarted;
    }
    private void OnRaceStarted()
    {
        text.enabled = false;
        title.SetActive(false);
        enabled = false;
    }
}
