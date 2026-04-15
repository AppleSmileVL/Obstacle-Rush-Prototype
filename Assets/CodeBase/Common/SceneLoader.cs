using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string MainMenuSceneName = "main_menu";

    [SerializeField] private string[] raceSceneNames = new string[0];

    [SerializeField] private bool wrapAroundRaces = true;

    public void LoadMainMenuScene()
    {
        EnsureTimeScaleNormal();
        SceneManager.LoadScene(MainMenuSceneName);
    }

    public void Restart()
    {
        EnsureTimeScaleNormal();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextRace()
    {
        EnsureTimeScaleNormal();

        if (TryLoadFromRaceList(offset: 1)) return;

        int nextIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextIndex);
    }

    public void LoadPreviousRace()
    {
        EnsureTimeScaleNormal();
        
        if (TryLoadFromRaceList(offset: -1)) return;
        int count = SceneManager.sceneCountInBuildSettings;
        int prevIndex = (SceneManager.GetActiveScene().buildIndex - 1 + count) % count;
        SceneManager.LoadScene(prevIndex);
    }

    public bool IsFirstRace()
    {
        if (raceSceneNames == null || raceSceneNames.Length == 0) return true;

        int index = Array.IndexOf(raceSceneNames, SceneManager.GetActiveScene().name);

        return index <= 0;
    }

    private bool TryLoadFromRaceList(int offset)
    {
        if (raceSceneNames == null || raceSceneNames.Length == 0)
            return false;

        string currentName = SceneManager.GetActiveScene().name;
        int index = Array.IndexOf(raceSceneNames, currentName);

        if (index < 0) return false;

        int next = index + offset;

        if (wrapAroundRaces)
        {
            next = ((next % raceSceneNames.Length) + raceSceneNames.Length) % raceSceneNames.Length;
        }
        else
        {
            if (next < 0 || next >= raceSceneNames.Length)
            {
                SceneManager.LoadScene(MainMenuSceneName);
                return false;
            }
        }

        SceneManager.LoadScene(raceSceneNames[next]);
        return true;
    }

    private void EnsureTimeScaleNormal()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
    }
}
