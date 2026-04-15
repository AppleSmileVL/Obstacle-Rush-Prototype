using UnityEngine;

public class UIPausePanel : MonoBehaviour, IDependency<Pauser>
{
    [SerializeField] private GameObject pausePanel;

    private Pauser pauser;

    public void Construct(Pauser obj) => pauser = obj;

    void Start()
    {
        pausePanel.SetActive(false);
        pauser.PauseStateChanged += OnPauseStateChanged;
    }

    private void OnDestroy()
    {
        pauser.PauseStateChanged -= OnPauseStateChanged;
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            pauser.ChangePauseState();
        }
    }

    public void UnPause()
    {
        pauser.UnPause();
    }
}
