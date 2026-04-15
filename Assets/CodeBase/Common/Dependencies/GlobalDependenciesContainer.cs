using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalDependenciesContainer : Dependency
{
    [SerializeField] private Pauser pauser;

    private static GlobalDependenciesContainer instance;

    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void BindAll(MonoBehaviour monoBehaviourInScene) 
    {
        Bind<Pauser>(pauser, monoBehaviourInScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAllObjectToBind();
    }
}
