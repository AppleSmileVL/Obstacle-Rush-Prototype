using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NitroColorStage
{
    [Range(0, 1)] public float NormalizedTheshold; 
    public Color Color; 
}

public class CarNitroIndicator : MonoBehaviour
{
    [SerializeField] private Nitro nitro;
    [SerializeField] private Image nitroBar;

    [Header("Color Setting")]
    [SerializeField] private NitroColorStage[] colorStages;

    private void Start()
    {
        if (nitro != null)
        {
            nitro.NitroChanged += OnNitroChanged;

            OnNitroChanged(nitro.CurrentNitro, nitro.MaxNitro);
        }
    }

    private void OnDestroy()
    {
        nitro.NitroChanged -= OnNitroChanged;
    }

    private void OnNitroChanged(float currentNitro, float maxNitro)
    {
        if (nitroBar == null) return;

        float fillAmount = Mathf.Clamp01(currentNitro / maxNitro);
        nitroBar.fillAmount = fillAmount;

        for (int i = 0; i < colorStages.Length; i++)
        {
            if (fillAmount <= colorStages[i].NormalizedTheshold)
            {
                nitroBar.color = colorStages[i].Color;
                break;
            }
        }

    }
}
