using System.Collections;
using UnityEngine;


public class CarInteractionHandler : MonoBehaviour, ICarInteraction, IDependency<Car>, IDependency<CarInputControl>, IDependency<CarChassis>
{
    private Car car;
    public void Construct(Car obj) => car = obj;

    private CarInputControl carInputControl;
    public void Construct(CarInputControl obj) => carInputControl = obj;

    private CarChassis carChassis;
    public void Construct(CarChassis obj) => carChassis = obj;

    private bool isStunned;

    private void ApplyImpulse (Vector3 direction, float force, ForceMode mode)
    {
        var rb = carChassis.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, mode);
        }
    }

    private void ApplyStun(float duration, float slowFactor)
    {
        if (isStunned) return;
        StartCoroutine(StunCoroutine(duration, slowFactor));
    }

    private IEnumerator StunCoroutine(float duration, float slowFactor)
    {
        isStunned = true;

        var renderer = GetComponentInChildren<MeshRenderer>();
        Color originalColor = renderer.material.color;

        renderer.material.color = Color.cyan;

        if (car != null)
        {
            car.ThrottleControl = 0;
            car.SteerControl = 0;
            car.BrakeControl = 0;
        }

        if (carInputControl != null) carInputControl.IsBlocked = true;

        var rb = carChassis.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity *= slowFactor;
            rb.angularVelocity *= slowFactor;
        }

        yield return new WaitForSeconds(duration);

        renderer.material.color = originalColor;

        if (carInputControl != null) carInputControl.IsBlocked = false;
        isStunned = false;
    }

    private IEnumerator HackCoroutine(float duration)
    {
        var renderer = GetComponentInChildren<MeshRenderer>();
        
        Color originalColor = renderer.material.color;

        renderer.material.color = Color.magenta;

        if  (carInputControl != null) carInputControl.IsInvertedControls = true;

        yield return new WaitForSeconds(duration);

        if (carInputControl != null) carInputControl.IsInvertedControls = false;

        renderer.material.color = originalColor;
    }

    void ICarInteraction.ApplyImpulse(Vector3 direction, float force, ForceMode mode)
    {
        ApplyImpulse(direction, force, mode);
    }

    void ICarInteraction.ApplyStun(float duration, float slowFactor)
    {
        ApplyStun(duration, slowFactor);
    }

    void ICarInteraction.ApplyHack(float duration)
    {
        StartCoroutine(HackCoroutine(duration));
    }
}
