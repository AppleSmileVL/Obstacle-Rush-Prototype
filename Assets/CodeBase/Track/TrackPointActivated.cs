using UnityEngine;

public class TrackPointActivated : TrackPoint
{
    [SerializeField] private GameObject hint;

    private void Start()
    {
        hint.SetActive(isTarget);
    }

    protected override void OnPassed()
    {
        hint.SetActive(false);
    }

    protected override void OnAssignAsTarget()
    {
        hint.SetActive(true);
    }
}
