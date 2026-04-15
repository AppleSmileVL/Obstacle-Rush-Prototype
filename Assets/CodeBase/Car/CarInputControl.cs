using System;
using UnityEngine;

public class CarInputControl : MonoBehaviour 
{
    [SerializeField] private Car car;

    [SerializeField] private AnimationCurve brakeCurve;
    [SerializeField] private AnimationCurve steerCurve;

    [SerializeField][Range(0.0f, 1.0f)] private float autoBrakeSrtenght = 0.5f;

    private float wheelSpeed;
    private float verticalAxis;
    private float horizontalAxis;
    private float handbrakeAxis;

    public bool IsBlocked;

    public bool IsInvertedControls { get; set; }

    private void Update()
    {
        if (IsBlocked)
        {
            car.ThrottleControl = 0;
            car.SteerControl = 0;
            return;
        }

        wheelSpeed = car.WheelSpeed; 
        car.HandbrakeControl = handbrakeAxis; 

        UpdateAxis(); 

        UpdateThrottle(); 

        UpdateSteer(); 

        UpdateAutoBrake();  
    }

    private void UpdateThrottle()
    {
        float actualSpeed = car.LinearVelocity;

        if (verticalAxis > 0.1f)
        {
            if (actualSpeed > 0.5f && car.SelectedGearIndex < 0)
            {
                car.ThrottleControl = 0;
                car.BrakeControl = brakeCurve.Evaluate(actualSpeed / car.MaxSpeed);
            }
            
            else if (actualSpeed <= 2f && car.SelectedGearIndex < 0)
            {
                car.ShiftToFirstGear();
                car.ThrottleControl = verticalAxis;
                car.BrakeControl = 0;
            }
            
            else
            {
                car.ThrottleControl = verticalAxis;
                car.BrakeControl = 0;
            }
        }
        
        else if (verticalAxis < -0.1f)
        {
            
            if (actualSpeed > 0.5f && car.SelectedGearIndex >= 0)
            {
                car.ThrottleControl = 0;
                car.BrakeControl = brakeCurve.Evaluate(actualSpeed / car.MaxSpeed);
            }
            
            else if (actualSpeed <= 0.5f && car.SelectedGearIndex >= 0)
            {
                car.ShiftToReversGear();
                car.ThrottleControl = Mathf.Abs(verticalAxis);
                car.BrakeControl = 0;
            }
            
            else if (car.SelectedGearIndex < 0)
            {
                car.ThrottleControl = Mathf.Abs(verticalAxis);
                car.BrakeControl = 0;
            }
        }
        
        else
        {
            car.ThrottleControl = 0;
            car.BrakeControl = 0;
        }
    }

    private void UpdateSteer()
    {
        car.SteerControl = steerCurve.Evaluate(car.LinearVelocity / car.MaxSpeed) * horizontalAxis;
    }

    private void UpdateAutoBrake()
    {
        if (verticalAxis == 0)
        {
            car.BrakeControl = brakeCurve.Evaluate(car.LinearVelocity / car.MaxSpeed) * autoBrakeSrtenght;
        }
    }

    private void UpdateAxis()
    {
        verticalAxis = Input.GetAxis("Vertical");
        horizontalAxis = Input.GetAxis("Horizontal");
        if (IsInvertedControls) horizontalAxis *= -1; 
        handbrakeAxis = Input.GetAxis("Jump");
    }

    public void Reset()
    {
        verticalAxis = 0;
        horizontalAxis = 0;
        handbrakeAxis = 0;

        car.ThrottleControl = 0;
        car.SteerControl = 0;
        car.BrakeControl = 0;
    }

    public void Stop()
    {
        Reset();

        car.BrakeControl = 1;
    }
}
