using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraSpeedEffects : MonoBehaviour, IDependency<Nitro>
{
    [Header("FOV Settings")]
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float nitroFOV = 80f;
    [SerializeField] private float lerpSpeed = 5f;

    [Header("Post-Process Settings")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private float nitroShutterAngle = 270f;

    private Camera SpeedCamera;
    private Nitro nitro;
    private MotionBlur motionBlur;

    public void Construct(Nitro obj) => nitro = obj;

    private void Awake()
    {
        SpeedCamera = GetComponent<Camera>();
        SpeedCamera.fieldOfView = baseFOV;

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out motionBlur);
        }
    }

    private void Update()
    {
        if (motionBlur == null)
        {
            Debug.LogError("Motion Blur не найден в профиле!");
            return;
        }

        if (nitro != null)
        {
            float targetFOV = nitro.IsNitroActive ? nitroFOV : baseFOV;

            SpeedCamera.fieldOfView = Mathf.Lerp(SpeedCamera.fieldOfView, targetFOV, Time.deltaTime * lerpSpeed);
        }

        if (motionBlur != null)
        {
            float targetBlur = nitro.IsNitroActive ? nitroShutterAngle : 0f; 

            motionBlur.shutterAngle.value = Mathf.Lerp(motionBlur.shutterAngle.value, targetBlur, Time.deltaTime * lerpSpeed); 
        }
    }
}
