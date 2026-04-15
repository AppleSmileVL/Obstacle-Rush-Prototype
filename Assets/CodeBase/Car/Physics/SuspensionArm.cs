using UnityEngine;

public class SuspensionArm : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float factor;

    private Vector3 initialLocalPos;
    private float targetInitialY;    

    private void Start()
    {
        initialLocalPos = transform.localPosition;

        if (target != null)
            targetInitialY = target.localPosition.y;
    }

    private void Update()
    {
        if (target == null) return;

        float deltaY = target.localPosition.y - targetInitialY;

        transform.localPosition = initialLocalPos + Vector3.forward * (deltaY * factor);
    }
}