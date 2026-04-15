using UnityEngine;

public class LaserGateTrigger : MonoBehaviour
{
    [SerializeField] private LaserGateController controller;

    [SerializeField] private GameObject gateRoot;

    private void Awake()
    {
        if (controller == null)
        {
            controller = GetComponentInParent<LaserGateController>();
        }

        if (gateRoot == null)
            gateRoot = gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled || controller == null) return;

        controller.GateTriggered(other, gateRoot);
    }
}
