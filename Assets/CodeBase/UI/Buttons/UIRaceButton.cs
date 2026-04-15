using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIRaceButton : UISelectableButton, IScriptableObjectProperty
{
    [SerializeField] private RaceInfo raceInfo;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;

    private void Start()
    {
        ApplyProperty(raceInfo);
        onClick.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        onClick.RemoveListener(OnSubmit);
    }

    public override void Submit()
    {
        base.Submit();
        OnSubmit();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    private void OnSubmit()
    {
        if (interactable == false || raceInfo == null) return;

        var container = GetComponentInParent<UISelectableButtonContainer>();
        if (container != null) container.Interactable = false;

        SceneManager.LoadScene(raceInfo.SceneName);
    }

    public void ApplyProperty(ScriptableObject property)
    {
        if (property == null || property is RaceInfo == false) return;

        raceInfo = property as RaceInfo;
        icon.sprite = raceInfo.Icon;
        title.text = raceInfo.Title;

        interactable = raceInfo.IsUnlocked();

        if (icon != null)
            icon.color = interactable ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);

        if (title != null)
            title.color = interactable ? Color.white : Color.gray;
    }
}
