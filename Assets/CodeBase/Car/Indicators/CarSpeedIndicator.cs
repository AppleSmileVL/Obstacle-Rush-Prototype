using UnityEngine;
using TMPro;

public class CarSpeedIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Car car;

    private void Update()
    {
        int speed = Mathf.FloorToInt(car.LinearVelocity);

        speedText.text = $"<mspace=0.5em>{speed.ToString()}</mspace>";
    }
}
