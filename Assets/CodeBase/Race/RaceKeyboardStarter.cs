using UnityEngine;

public class RaceKeyboardStarter : MonoBehaviour, IDependency<RaceStateTracker>
{
    private RaceStateTracker raceStateTracker;
    public void Construct(RaceStateTracker obj) => raceStateTracker = obj;

    void Start()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) == true)
        {
            raceStateTracker.LaunchPreparationStarted();
        }
    }
}
