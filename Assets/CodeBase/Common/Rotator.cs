using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 speed;
    [SerializeField] private Transform targetTransform;

    private void Update()
    {
        targetTransform.Rotate(speed * Time.deltaTime);
    }
}
