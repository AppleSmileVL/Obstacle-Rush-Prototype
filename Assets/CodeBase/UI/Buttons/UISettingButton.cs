using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingButton : UISelectableButton, IScriptableObjectProperty, ISettingControl
{
    [SerializeField] private Setting setting;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image previousImage;
    [SerializeField] private Image nextImage;

    public void Increment() => SetNextValueSettings();
    public void Decrement() => SetPreviousValueSettings();

    private void Start()
    {
        ApplyProperty(setting);
    }

    public void SetNextValueSettings()
    {
        setting?.SetNextValue();
        setting?.Apply();
        UpdateInfo();
    }

    public void SetPreviousValueSettings()
    {
        setting?.SetPreviousValue();
        setting?.Apply();
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        titleText.text = setting.Title;
        valueText.text = setting.GetStringValue();

        if (previousImage != null)
            previousImage.enabled = !setting.isMinValue;

        if (nextImage != null)
            nextImage.enabled = !setting.isMaxValue;
    }

    public void ApplyProperty(ScriptableObject property)
    {
        if (property == null) return;

        if (property is Setting == false) return;
        setting = property as Setting;

        UpdateInfo();
    }
}