using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CarChassis : MonoBehaviour 
{
    [SerializeField] private WheelAxle[] wheelAxles;
    [SerializeField] private float wheelBaseLength; 

    [SerializeField] private Transform centerOfMass; 
    [Header("DownForce Settings")]
    [SerializeField] private float downForceMin;
    [SerializeField] private float downForceMax;
    [SerializeField] private float downForceFactor;

    [Header("AngularDrag Settings")]
    [SerializeField] private float angularDragMin;
    [SerializeField] private float angularDragMax;
    [SerializeField] private float angularDragFactor; 

    // DEBUG
    public float MotorTorque;      
    public float SteerAngle;       
    public float BrakeTorque;      
    public float HandbrakeControl; 

    public float LinearVelocity => (float)(rigidbody.velocity.magnitude * 3.6);

    private new Rigidbody rigidbody;
    public Rigidbody Rigidbody => rigidbody == null? GetComponent<Rigidbody>(): rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rigidbody.centerOfMass = centerOfMass.localPosition;
        }

        for (int i = 0; i < wheelAxles.Length; i++)
        {
            wheelAxles[i].ConfigureVehicleSubsteps(50, 50, 50);
        }
    }

    private void FixedUpdate()
    {
        UpdateAngularDrag(); 

        UpdateDownForce(); 

        UpdateWheelAxles(); 
    }

    public float GetAverageRpm() 
    {
        float sum = 0;

        for (int i = 0; i<wheelAxles.Length; i++)
        {
            sum += wheelAxles[i].GetAverageRpm();
        }
        return sum / wheelAxles.Length;
    }

    public float GetWheelSpeed() 
    {
        return GetAverageRpm() * wheelAxles[0].GetRadius() * 2 * 0.1885f; 
    }

    private void UpdateAngularDrag() 
    {
        rigidbody.angularDrag = Mathf.Clamp(angularDragFactor * LinearVelocity, angularDragMin, angularDragMax);
    }

    private void UpdateDownForce() 
    {
        float downForce = Mathf.Clamp(downForceFactor * LinearVelocity, downForceMin, downForceMax);
        rigidbody.AddForce(-transform.up * downForce);
    }

    private void UpdateWheelAxles() 
    {
        int amountMotorWheels = 0;

        for (int i = 0; i<wheelAxles.Length; i++) 
        {
            if (wheelAxles[i].IsMotor == true)
            {
                amountMotorWheels += 2;
            }
        }

        for (int i = 0; i<wheelAxles.Length; i++) 
        {
            wheelAxles[i].Update();

            wheelAxles[i].ApplyMotorTorque(MotorTorque / amountMotorWheels);
            wheelAxles[i].ApplySteerAngle(SteerAngle, wheelBaseLength);

            float appliedBrake = BrakeTorque;

            if (HandbrakeControl > 0.1f && wheelAxles[i].IsSteer == false)
            {
                appliedBrake = 1000000f;
            }

            wheelAxles[i].ApplyBrakeTorque(appliedBrake);
        }
    }

    public void Reset()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
