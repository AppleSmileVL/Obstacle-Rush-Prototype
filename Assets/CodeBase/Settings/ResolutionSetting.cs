using UnityEngine;

[CreateAssetMenu]
public class ResolutionSetting : Setting
{
    [SerializeField]
    private Vector2Int[] availableResolutions = new Vector2Int[]
    {
        new Vector2Int(1024, 768),
        new Vector2Int(1280, 720),
        new Vector2Int(1280, 960), 
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
    };

    private int currentResolutionIndex = 0;

    public override bool isMinValue => currentResolutionIndex == 0;
    public override bool isMaxValue => currentResolutionIndex == availableResolutions.Length - 1;

    public override void SetNextValue()
    {
        if (!isMaxValue)
        {
            currentResolutionIndex++;
            Apply();
        }
    }

    public override void SetPreviousValue()
    {
        if (!isMinValue)
        {
            currentResolutionIndex--;
            Apply();
        }
    }

    public override object GetValue()
    {
        return availableResolutions[currentResolutionIndex];
    }

    public override string GetStringValue()
    {
        Vector2Int res = availableResolutions[currentResolutionIndex];
        return $"{res.x}×{res.y}";
    }

    public override void Apply()
    {
        Vector2Int resolution = availableResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        Save();
    }
    public override void Load()
    {
        currentResolutionIndex = PlayerPrefs.GetInt(title, availableResolutions.Length - 1);
    }

    public override void Save()
    {
        PlayerPrefs.SetInt(title, currentResolutionIndex);
    }
}
