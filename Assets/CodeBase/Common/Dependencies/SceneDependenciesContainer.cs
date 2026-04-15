using UnityEngine;

public class SceneDependenciesContainer : Dependency
{
    [SerializeField] private TrackpointCircuit trackpointCircuit;
    [SerializeField] private RaceStateTracker raceStateTracker;
    [SerializeField] private RaceTimeTracker RaceTimeTracker;
    [SerializeField] private RaceResultTime RaceResultTime;
    [SerializeField] private CarInputControl carInputControl;
    [SerializeField] private CarCameraController carCameraController;
    [SerializeField] private Car car;
    [SerializeField] private CarChassis carChassis;
    [SerializeField] private Nitro nitro;
    [SerializeField] private SceneLoader sceneLoader;


    protected override void BindAll(MonoBehaviour monoBehaviourInScene)
    {
        Bind<TrackpointCircuit>(trackpointCircuit, monoBehaviourInScene);
        Bind<RaceStateTracker>(raceStateTracker, monoBehaviourInScene);
        Bind<RaceTimeTracker>(RaceTimeTracker, monoBehaviourInScene);
        Bind<RaceResultTime>(RaceResultTime, monoBehaviourInScene);
        Bind<CarInputControl>(carInputControl, monoBehaviourInScene);
        Bind<CarCameraController>(carCameraController, monoBehaviourInScene);
        Bind<Car>(car, monoBehaviourInScene);
        Bind<CarChassis>(carChassis, monoBehaviourInScene);
        Bind<Nitro>(nitro, monoBehaviourInScene);
        Bind<SceneLoader>(sceneLoader, monoBehaviourInScene);
    }

    private void Awake()
    {
        FindAllObjectToBind();
    }
}
