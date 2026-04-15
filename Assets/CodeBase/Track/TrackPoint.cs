using UnityEngine;
using UnityEngine.Events;

public class TrackPoint : MonoBehaviour
{
    public event UnityAction<TrackPoint> Triggered;

    protected virtual void OnPassed() { }
    protected virtual void OnAssignAsTarget() { }

    public TrackPoint Next;
    public bool IsFirst;
    public bool IsLast;

    protected bool isTarget;
    public bool IsTarget => isTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        Car car = null;
        if (other.attachedRigidbody != null)
            car = other.attachedRigidbody.GetComponentInParent<Car>();

        if (car == null)
            car = other.GetComponentInParent<Car>();

        if (car == null)
            car = other.transform.root.GetComponent<Car>();

        if (car == null) return;

        Triggered?.Invoke(this);
    }

    public void Passed()
    {
        isTarget = false;
        OnPassed();
    }

    public void AssignAsTarget()
    {
        isTarget = true;
        OnAssignAsTarget();
    }

    public void Reset()
    {
        Next = null;
        IsFirst = false;
        IsLast = false;
    }
}
