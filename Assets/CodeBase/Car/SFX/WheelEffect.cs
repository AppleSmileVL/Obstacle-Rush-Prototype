using UnityEngine;

public class WheelEffect : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] private WheelCollider[] wheels;
    [SerializeField] private ParticleSystem[] wheelsSmoke;
 
    [SerializeField] private float forwardSlipLimit;
    [SerializeField] private float sidewaysSlipLimit;

    [SerializeField] private new AudioSource audio;

    [SerializeField] private GameObject skidPrefab;

    private WheelHit wheelHit;
    private Transform[] skidTrail;
    private bool isPaused;
    private bool startedSound;

    private Pauser pauser;
    public void Construct(Pauser obj) => pauser = obj;

    private void Start()
    {
        skidTrail = new Transform[wheels.Length];

        if (pauser != null)
        {
            pauser.PauseStateChanged += OnPauseStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (pauser != null)
        {
            pauser.PauseStateChanged -= OnPauseStateChanged;
        }
    }

    private void OnPauseStateChanged(bool pause)
    {
        isPaused = pause;
        if (startedSound)
        {
            if (isPaused)
            {
                PauseAllSounds();
            }
            else
            {
                ResumeAllSounds();
            }
        }
    }

    private void PauseAllSounds()
    {
        if (audio != null && audio.isPlaying)
        {
            audio.Pause();
        }
    }

    private void ResumeAllSounds()
    {
        if (audio != null && !audio.isPlaying)
        {
            audio.UnPause();
        }
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        bool isSlip = false;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetGroundHit(out wheelHit);

            if (wheels[i].isGrounded == true)
            {
                if (wheelHit.forwardSlip > forwardSlipLimit || wheelHit.sidewaysSlip > sidewaysSlipLimit)
                {
                    if (skidTrail[i] == null)
                    {
                        skidTrail[i] = Instantiate(skidPrefab).transform;
                    }

                    if (audio.isPlaying == false) 
                    {
                        audio.Play();
                        startedSound = true;
                    }   

                    if (skidTrail[i] != null)
                    {
                        skidTrail[i].position = wheels[i].transform.position - wheelHit.normal * wheels[i].radius;
                        skidTrail[i].forward = -wheelHit.normal;

                        wheelsSmoke[i].transform.position = skidTrail[i].position;
                        wheelsSmoke[i].Emit(1); 
                    }

                    isSlip = true;

                    continue;
                }
            }

            skidTrail[i] = null;
            wheelsSmoke[i].Stop();
        }

        if (isSlip == false)
        {
            audio.Stop();
            startedSound = false;
        }
    }
}
