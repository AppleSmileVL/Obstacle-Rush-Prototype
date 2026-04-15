using System;
using UnityEngine;

public class Nitro : MonoBehaviour, IDependency<CarChassis>, IDependency<Car>
{
    [SerializeField] private NitroSettingAsset nitroSettingAsset;

    private CarChassis carChassis;

    public void Construct(CarChassis obj) => carChassis = obj;

    private Car car;

    public void Construct(Car obj) => car = obj;

    //debag
    [SerializeField] private float currentNitro;
    public float CurrentNitro => currentNitro;

    public bool IsNitroActive => isNitroActive;

    public float MaxNitro => nitroSettingAsset.MaxNitro;

    public event Action<float, float> NitroChanged;

    private bool isNitroActive;
    
    private bool canUseNitro => car.ThrottleControl > 0.1f && car.SelectedGearIndex >= 0 && car.LinearVelocity > 1f;

    private void Awake()
    {
        currentNitro = 0;
    }

    public void AddNitro(float amount)
    {
        currentNitro = Mathf.Clamp(currentNitro + amount, 0,nitroSettingAsset.MaxNitro);
        NitroChanged?.Invoke(currentNitro, nitroSettingAsset.MaxNitro);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canUseNitro && currentNitro > 0)
        {
            isNitroActive = true;
            ConsumeNitro();
        }
        else
        {
            isNitroActive = false;   
        }
        NitroChanged?.Invoke(currentNitro, nitroSettingAsset.MaxNitro);
    }

    private void FixedUpdate()
    {
        if (isNitroActive)
        {
            var rb = carChassis.GetComponent<Rigidbody>();
            rb.AddForce(carChassis.transform.forward * nitroSettingAsset.NitroForce, ForceMode.Acceleration);
        }
    }

    private void ConsumeNitro()
    {
        currentNitro -= nitroSettingAsset.ConsumptionRate * Time.deltaTime;
        if (currentNitro < 0) currentNitro = 0;
    }
}
