using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingSlider : UISelectableButton, ISettingControl, IScriptableObjectProperty
{
    [SerializeField] private Setting setting;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] private Slider slider;

    private bool isUpdatingUI = false;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Start()
    {
        if (setting != null)
        {
            setting.Load();
            UpdateInfo();
        }
            
    }

    private void OnSliderValueChanged(float value)
    {
        if (isUpdatingUI || setting == null) return;

        setting.SetValue(value);
        setting.Apply();
    }

    private void UpdateInfo()
    {
        if (setting == null) return;
        
        isUpdatingUI = true;

        titleText.text = setting.Title;

        if (setting is AudioMixerFloatSetting audioSetting)
        {
            slider.minValue = audioSetting.MinRealValue;
            slider.maxValue = audioSetting.MaxRealValue;
        }

        slider.value = (float)setting.GetValue();

        isUpdatingUI = false;
    }

    public void Increment()
    {
        setting?.SetNextValue();
        setting?.Apply();
        UpdateInfo();
    }

    public void Decrement()
    {
        setting?.SetPreviousValue();
        setting?.Apply();
        UpdateInfo();
    }

    public void ApplyProperty(ScriptableObject property)
    {
        if (property is Setting newSetting)
        {
            setting = newSetting;
            setting.Load();
            UpdateInfo();
        }
    }

}
