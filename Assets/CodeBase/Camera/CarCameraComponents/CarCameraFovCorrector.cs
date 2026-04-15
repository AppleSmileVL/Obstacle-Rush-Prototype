using UnityEngine;

public class CarCameraFovCorrector : CarCameraComponent
{
    [SerializeField] private float minFieldOfView;
    [SerializeField] private float maxFieldOfView;

    private float defaultFieldOfView;

    private void Start()
    {
        defaultFieldOfView = camera.fieldOfView;
    }

    private void Update()
    {
        float targetFieldOfView = Mathf.Lerp(minFieldOfView, maxFieldOfView, car.NormalizedLinearVelocity); 

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFieldOfView, Time.deltaTime * 5f); 
    }
}
