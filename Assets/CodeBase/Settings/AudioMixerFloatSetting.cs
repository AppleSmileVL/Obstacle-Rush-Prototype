using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioMixerFloatSetting : Setting 
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string nameParametry;

    [SerializeField] private float minRealValue;
    [SerializeField] private float maxRealValue;

    [SerializeField] private float virtualStep;
    [SerializeField] private float minVirtualValue;
    [SerializeField] private float maxVirtualValue;

    private float currentValue;
    private bool isInitialized = false;

    public float MinRealValue => minRealValue;
    public float MaxRealValue => maxRealValue;

    public override bool isMinValue {get => currentValue == minRealValue;}
    public override bool isMaxValue {get => currentValue == maxRealValue;}

    private void Initialize()
    {
        if (!isInitialized && audioMixer != null)
        {
            audioMixer.GetFloat(nameParametry, out float dbValue);

            currentValue = Mathf.Pow(10, dbValue / 20f);

            isInitialized = true;
        }
    }

    public override void SetNextValue()
    {
        Initialize();
        AddValue(Mathf.Abs(maxRealValue - minRealValue) / virtualStep);
    }

    public override void SetPreviousValue()
    {
        Initialize();
        AddValue(-Mathf.Abs(maxRealValue - minRealValue) / virtualStep);
    }

    public override string GetStringValue()
    {
        Initialize();
        float percentage = Mathf.InverseLerp(minRealValue, maxRealValue, currentValue) * 100f;
        return Mathf.RoundToInt(percentage).ToString();
    }

    public override object GetValue()
    {
        Initialize();
        return currentValue;
    }

    public override void SetValue(float value)
    {
        Initialize();
        currentValue = value;
    }

    private void AddValue(float value)
    {
        currentValue += value;
        currentValue = Mathf.Clamp(currentValue, minRealValue, maxRealValue);
    }

    public override void Apply()
    {
        Initialize();

        float clearValue = Mathf.Max(currentValue, 0.0001f);

        float dbValue = Mathf.Log10(clearValue) * 20;
        dbValue = Mathf.Clamp(dbValue, -80f, 0f);

        audioMixer.SetFloat(nameParametry, dbValue);
        Save();
    }

    public override void Load()
    {
        currentValue = PlayerPrefs.GetFloat(title, 0);
    }

    public override void Save()
    {
        PlayerPrefs.SetFloat(title, currentValue);
    }
}
