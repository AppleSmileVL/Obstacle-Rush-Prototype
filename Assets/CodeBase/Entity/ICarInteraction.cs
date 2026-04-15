using UnityEngine;

public interface ICarInteraction
{
    void ApplyImpulse(Vector3 direction, float force, ForceMode mode);

    void ApplyStun(float duration, float slowFactor);

    void ApplyHack(float duration);
}