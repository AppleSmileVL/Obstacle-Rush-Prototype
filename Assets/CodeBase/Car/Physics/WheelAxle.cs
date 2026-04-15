using System;
using UnityEngine;

[Serializable]
public class WheelAxle 
{
    [SerializeField] private WheelCollider leftWheelCollider;
    [SerializeField] private WheelCollider rightWheelCollider;

    [SerializeField] private Transform leftWheelMesh;
    [SerializeField] private Transform rightWheelMesh;

    [SerializeField] private bool isMotor; 
    [SerializeField] private bool isSteer;

    [SerializeField] private float wheelWidth; 

    [SerializeField] private float antiRollForce;

    [SerializeField] private float additionalWheelDownForce;

    [SerializeField] private float baseForwardStiffness = 1.5f;
    [SerializeField] private float stabilityForwardFactor = 1.0f;

    [SerializeField] private float baseSidewaysStiffness = 2.0f;
    [SerializeField] private float stabilitySidewaysFactor = 1.0f;

    private WheelHit leftWheelHit;
    private WheelHit rightWheelHit;

    public bool IsMotor => isMotor;
    public bool IsSteer => isSteer;

    // Public API
    public void Update()
    {
        UpdateWheelHits();   
        ApplyAntiRoll();     
        ApplyDownForce();    
        CorrectStiffness();  
        SyncMeshTransform(); 
    }

    public void ConfigureVehicleSubsteps(float speedTreshold, int speedBelowThreshold, int speedAboveThresold) 
    {
        leftWheelCollider.ConfigureVehicleSubsteps(speedTreshold, speedBelowThreshold, speedAboveThresold);
        rightWheelCollider.ConfigureVehicleSubsteps(speedTreshold, speedBelowThreshold, speedAboveThresold);
    }

    private void UpdateWheelHits()
    {
        leftWheelCollider.GetGroundHit(out leftWheelHit);
        rightWheelCollider.GetGroundHit(out rightWheelHit);
    }

    private void CorrectStiffness()
    {
        WheelFrictionCurve leftForward = leftWheelCollider.forwardFriction;
        WheelFrictionCurve rightForward = rightWheelCollider.forwardFriction;

        WheelFrictionCurve leftSideways = leftWheelCollider.sidewaysFriction;
        WheelFrictionCurve rightSideways = rightWheelCollider.sidewaysFriction;

        leftForward.stiffness = baseForwardStiffness + Mathf.Abs(leftWheelHit.forwardSlip) * stabilityForwardFactor;
        rightForward.stiffness = baseForwardStiffness + Mathf.Abs(rightWheelHit.forwardSlip) * stabilityForwardFactor;

        leftSideways.stiffness = baseSidewaysStiffness + Mathf.Abs(leftWheelHit.sidewaysSlip) * stabilitySidewaysFactor;
        rightSideways.stiffness = baseSidewaysStiffness + Mathf.Abs(rightWheelHit.sidewaysSlip) * stabilitySidewaysFactor;

        leftWheelCollider.forwardFriction = leftForward;
        rightWheelCollider.forwardFriction = rightForward;

        leftWheelCollider.sidewaysFriction = leftSideways;
        rightWheelCollider.sidewaysFriction = rightSideways;
    }

    private void ApplyDownForce()
    {
        if (leftWheelCollider.isGrounded == true)
        {
            leftWheelCollider.attachedRigidbody.AddForceAtPosition(leftWheelHit.normal * -additionalWheelDownForce * 
                leftWheelCollider.attachedRigidbody.velocity.magnitude, leftWheelCollider.transform.position);
        }

        if (rightWheelCollider.isGrounded == true)
        {
            rightWheelCollider.attachedRigidbody.AddForceAtPosition(rightWheelHit.normal * -additionalWheelDownForce * 
                rightWheelCollider.attachedRigidbody.velocity.magnitude, rightWheelCollider.transform.position);
        }
    }

    private void ApplyAntiRoll() 
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        if (leftWheelCollider.isGrounded == true)
        {
            travelL = (-leftWheelCollider.transform.InverseTransformPoint(leftWheelHit.point).y - leftWheelCollider.radius) / leftWheelCollider.suspensionDistance;
        }

        if (rightWheelCollider.isGrounded == true)
        {
            travelR = (-rightWheelCollider.transform.InverseTransformPoint(rightWheelHit.point).y - rightWheelCollider.radius) / rightWheelCollider.suspensionDistance;
        }

        float antiRoll = (travelL - travelR) * antiRollForce;
        
        if (leftWheelCollider.isGrounded == true)
        {
            leftWheelCollider.attachedRigidbody.AddForceAtPosition(leftWheelCollider.transform.up * -antiRoll, leftWheelCollider.transform.position);
        }
        
        if (rightWheelCollider.isGrounded == true)
        {
            rightWheelCollider.attachedRigidbody.AddForceAtPosition(rightWheelCollider.transform.up * antiRoll, rightWheelCollider.transform.position);
        }
    }

    public void ApplySteerAngle(float steerAngle, float wheelBaseLength) 
    {
        if (isSteer == false) return;

        float radius = Mathf.Abs(wheelBaseLength * Mathf.Tan(Mathf.Deg2Rad * (90 - Mathf.Abs(steerAngle))));
        float angleSing = Mathf.Sign(steerAngle);

        if (steerAngle > 0)
        {
            leftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (radius + (wheelWidth * 0.5f))) * angleSing;
            rightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (radius - (wheelWidth * 0.5f))) * angleSing;
        }
        else if (steerAngle < 0)
        {
            leftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (radius - (wheelWidth * 0.5f))) * angleSing;
            rightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (radius + (wheelWidth * 0.5f))) * angleSing;
        }
        else
        {
            leftWheelCollider.steerAngle = steerAngle;
            rightWheelCollider.steerAngle = steerAngle;
        }
    }

    public void ApplyMotorTorque(float motorTorque) 
    {
        if (isMotor == false) return;

        leftWheelCollider.motorTorque = motorTorque;
        rightWheelCollider.motorTorque = motorTorque;
    }

    public void ApplyBrakeTorque(float brakeTorque)
    {
        leftWheelCollider.brakeTorque = brakeTorque;
        rightWheelCollider.brakeTorque = brakeTorque;
    }

    public float GetAverageRpm()
    {
        return (leftWheelCollider.rpm + rightWheelCollider.rpm) * 0.5f;
    }

    public float GetRadius()
    {
        return leftWheelCollider.radius;
    }

    private void SyncMeshTransform() 
    {
        UpdateWheelTransform(leftWheelCollider, leftWheelMesh);
        UpdateWheelTransform(rightWheelCollider, rightWheelMesh);
    }

    private void UpdateWheelTransform(WheelCollider wheelCollider, Transform wheelTransform) 
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
