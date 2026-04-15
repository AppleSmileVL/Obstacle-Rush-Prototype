using UnityEngine;

public class CarRespawner : MonoBehaviour, IDependency<RaceStateTracker>, IDependency<Car>, IDependency<CarInputControl>
{
    [SerializeField] private float respawnHeight;
    [SerializeField] private float respawnOffset;

    private TrackPoint respawnTrackPoint;

    private RaceStateTracker raceStateTracker;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    private CarInputControl carInputControl;
    public void Construct(CarInputControl obj) => carInputControl = obj;

    private Car car;
    public void Construct(Car obj) => car = obj;

    private void Start()
    {
        raceStateTracker.TrackPointPassed += OnTrackPointPassed;
    }

    private void OnDestroy()
    {
        raceStateTracker.TrackPointPassed -= OnTrackPointPassed;
    }

    private void OnTrackPointPassed(TrackPoint point)
    {
        respawnTrackPoint = point;
    }

    public void Respawn()
    {
        if (respawnTrackPoint == null) return;

        if (raceStateTracker.State != RaceState.Race) return;

        Vector3 respawnPosition = respawnTrackPoint.transform.position
            + respawnTrackPoint.transform.up * respawnHeight
            + respawnTrackPoint.transform.right * respawnOffset;

        car.Respawn(respawnPosition, respawnTrackPoint.transform.rotation);

        carInputControl.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            Respawn();
        }
    }
}
