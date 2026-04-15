using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CarChassis))]
public class Car : MonoBehaviour 
{
    public event UnityAction<string> GearChanged; 

    private bool hasShiftedThisFrame = false;

    [SerializeField] private float maxSteerAngle;  
    [SerializeField] private float maxBrakeTorque; 

    [Header("Engine")]
    [SerializeField] private AnimationCurve engineTorqueCurve;
    [SerializeField] private float engineMaxTorque; 
    //DEBUG
    [SerializeField] private float engineTorque; 
    //DEBUG
    [SerializeField] private float engineRpm;    
    [SerializeField] private float engineMinRpm; 
    [SerializeField] private float engineMaxRpm; 
    
    [Header("Gearbox")]
    [SerializeField] private float[] gears; 
    [SerializeField] private float finalDriveRatio;

    //DEBUG
    [SerializeField] private float selectedGear;
    [SerializeField] private float rearGear;
    [SerializeField] private float upShiftEngineRpm;
    [SerializeField] private float downShitftEngineRpm;

    // DEBUG
    [SerializeField] private int selectedGearIndex; 

    [SerializeField] private int maxSpeed;
    public int SelectedGearIndex => selectedGearIndex;
    public float EngineMaxRpm => engineMaxRpm;
    public float EngineRpm => engineRpm;

    private CarChassis chassis;
    public Rigidbody Rigidbody => chassis == null? GetComponent<CarChassis>().Rigidbody : chassis.Rigidbody;

    public float LinearVelocity => chassis.LinearVelocity; 
    public float NormalizedLinearVelocity => chassis.LinearVelocity / maxSpeed; 
    public float WheelSpeed => chassis.GetWheelSpeed();
    public float MaxSpeed => maxSpeed;


    private float linearVelocity;

    public float ThrottleControl;  
    public float SteerControl;     
    public float BrakeControl;     
    public float HandbrakeControl; 

    private void Start()
    {
        chassis = GetComponent<CarChassis>();
    }

    private void Update() 
    {
        linearVelocity = LinearVelocity;

        UpdateEngineTorque();

        AutoGearShift();

        if (LinearVelocity >= maxSpeed) 
        {
            engineTorque = 0f;
        }

        chassis.MotorTorque = ThrottleControl * engineTorque; 
        chassis.SteerAngle = SteerControl * maxSteerAngle;    
        chassis.BrakeTorque = BrakeControl * maxBrakeTorque;  
        chassis.HandbrakeControl = HandbrakeControl;          

        hasShiftedThisFrame = false;
    }

    public string GetSelectedGearName()
    {
        if (selectedGear == rearGear)
            return "R"; 
        else if (selectedGear == 0)
            return "N"; 
        else
            return (selectedGearIndex + 1).ToString(); 
    }
    private void AutoGearShift() 
    {
        if (selectedGear < 0) return; 
        if (hasShiftedThisFrame) return; 

        if (engineRpm >= upShiftEngineRpm)
        {
            UpGear();
            hasShiftedThisFrame = true;
        }
        else if (engineRpm < downShitftEngineRpm)
        {
            DownGear();
            hasShiftedThisFrame = true;
        }
    }

    public void UpGear()
    {
        ShiftGear(selectedGearIndex + 1); 
    }

    public void DownGear()
    {
        ShiftGear(selectedGearIndex - 1); 
    }

    public void ShiftToReversGear()
    {
        selectedGear = rearGear; 
        selectedGearIndex = -1; 
        GearChanged?.Invoke(GetSelectedGearName()); 
    }

    public void ShiftToFirstGear() 
    {
        ShiftGear(0); 
    }

    public void ShiftToNetral()
    {
        selectedGear = 0; 
        selectedGearIndex = 0;
        GearChanged?.Invoke(GetSelectedGearName()); 
    }

    private void ShiftGear(int gearIndex)
    {
        gearIndex = Mathf.Clamp(gearIndex, 0, gears.Length - 1); 
        selectedGear = gears[gearIndex]; 
        selectedGearIndex = gearIndex; 
        GearChanged?.Invoke(GetSelectedGearName()); 
    }

    private void UpdateEngineTorque()
    {
        
        float absGear = Mathf.Abs(selectedGear);
        
        engineRpm = engineMinRpm + Mathf.Abs(chassis.GetAverageRpm() * absGear * finalDriveRatio);
        engineRpm = Mathf.Clamp(engineRpm, engineMinRpm, engineMaxRpm);

        
        engineTorque = engineTorqueCurve.Evaluate(engineRpm / engineMaxRpm) * engineMaxTorque * finalDriveRatio * Mathf.Sign(selectedGear) * absGear;
    }

    public void Reset()
    {
        chassis.Reset();

        chassis.MotorTorque = 0;
        chassis.BrakeTorque = 0;
        chassis.SteerAngle = 0;

        ThrottleControl = 0;
        BrakeControl = 0;
        SteerControl = 0;
    }

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        Reset();

        transform.position = position;
        transform.rotation = rotation;
    }
}
