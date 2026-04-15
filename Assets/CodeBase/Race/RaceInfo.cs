using UnityEngine;

[CreateAssetMenu]
public class RaceInfo : ScriptableObject
{
    [SerializeField] private string sceneName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;

    [SerializeField] private float goldTime;
    [SerializeField] private RaceInfo[] requiredTrackToUnlock;

    public string SceneName => sceneName;
    public Sprite Icon => icon;
    public string Title => title;
    public float GoldTime => goldTime;

    public bool IsUnlocked()
    {
        if (requiredTrackToUnlock == null || requiredTrackToUnlock.Length == 0)
            return true;

        foreach (var track in requiredTrackToUnlock)
        {
            float record = PlayerPrefs.GetFloat(track.SceneName + "_player_bst_time", 0f);

            if (record == 0f || record > track.GoldTime)
            {
                return false;
            }
        }

        return true;
    }
}
