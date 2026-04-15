using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResultRace : MonoBehaviour, IDependency<RaceResultTime>, IDependency<SceneLoader>
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private UISelectableButtonContainer buttonContainer;
    [SerializeField] private UIButton previousButton;
    [SerializeField] private UIButton nextButton;
    [SerializeField] TextMeshProUGUI recordTime;
    [SerializeField] TextMeshProUGUI playerTime;

    private RaceResultTime raceResultTime;
    public void Construct(RaceResultTime obj) => raceResultTime = obj;

    private SceneLoader sceneLoader;
    public void Construct(SceneLoader obj) => sceneLoader = obj;

    private void Start()
    {
        raceResultTime.ResultUpdated += OnUpdateResults;

        resultPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        raceResultTime.ResultUpdated -= OnUpdateResults;
    }

    private void OnUpdateResults()
    {
        resultPanel.SetActive(true);

        if (buttonContainer != null)
            buttonContainer.gameObject.SetActive(true);

        float absoluteRecord = raceResultTime.GetAbsoluteRecord();
        float currentTime = raceResultTime.CurrentTime;

        if (previousButton != null && sceneLoader != null)
        {
            bool isFirst = sceneLoader.IsFirstRace();
            previousButton.Interactable = !isFirst;

            var image = previousButton.GetComponent<Image>();
            if (image != null)
            {
                image.color = isFirst ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.white;
            }
        }

        if (nextButton != null && sceneLoader != null)
        {
            bool isTimeBeaten = currentTime <= absoluteRecord;

            nextButton.Interactable = isTimeBeaten;

            var image = nextButton.GetComponent<Image>();
            if (image != null)
            {
                image.color = isTimeBeaten ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        float displayRecordTime = currentTime < absoluteRecord ? currentTime : raceResultTime.PlayerRecordTime;

        recordTime.text = StringTime.SecondToTimeString(displayRecordTime);
        playerTime.text = StringTime.SecondToTimeString(currentTime);
    }
}
