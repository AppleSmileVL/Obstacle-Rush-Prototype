using UnityEngine;

public class CarCameraShaker : CarCameraComponent
{
    [SerializeField]
    [Range(0f, 1f)] private float normalizeSpeedShake;
    [SerializeField] private float shakeAmount;

    private void Update()
    {
        if (car.NormalizedLinearVelocity >= normalizeSpeedShake)
        {
            transform.localPosition += Random.insideUnitSphere * shakeAmount * Time.deltaTime;
        }
    }
}
