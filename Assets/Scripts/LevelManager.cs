using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int CurrentLevel { get; private set; } = 1;

    private void Awake()
    {
        ManageSingleton();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void ManageSingleton()
    {
        Debug.Log("Scene is " + SceneManager.GetActiveScene().name);
        if (Instance != null && Instance != this)
        {
            Debug.Log("LM: Destroying duplicate");
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("LM: Using this as the Instance");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void NewGame()
    {
        Debug.Log("LM: New Game");
        CurrentLevel = 1;
        LoadCurrentLevel();
    }

    public void LoadNextLevel()
    {
        Debug.Log("LM: Next Level");
        CurrentLevel++;
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        string sceneName = "Level " + CurrentLevel;
        Debug.Log("LM: Load " + sceneName);
        LoadSceneByName(sceneName);
    }

    public void LoadMainMenu()
    {
        Debug.Log("LM: Main Menu");
        LoadSceneByName("Main Menu");
    }

    public void LoadGameOver()
    {
        float delay = 3.0f;
        Debug.Log("LM: Game Over");
        LoadSceneByName("Game Over", delay);
    }

    public void QuitGame()
    {
        Debug.Log("LM: Quitting Game");
        Application.Quit();
    }

    // public because called from Bootstrap
    public void LoadSceneByName(string sceneName)
    {
        // CleanupPowerUps();
        SceneManager.LoadScene(sceneName);
    }

    void LoadSceneByName(string sceneName, float delay)
    {
        StartCoroutine(WaitLoadSceneByName(sceneName, delay));
    }

    IEnumerator WaitLoadSceneByName(string sceneName, float delay)
    {
        // CleanupPowerUps();
        yield return new WaitForSeconds(delay);
        LoadSceneByName(sceneName);
    }

    // void CleanupPowerUps()
    // {
    //     GameObject[] powerUpObjects = GameObject.FindGameObjectsWithTag("PowerUp");
    //     Debug.Log("LM: Found " + powerUpObjects.Length + " PowerUps");
    //     foreach (var powerUpObject in powerUpObjects)
    //     {
    //         Debug.Log("LM: Destroying " + powerUpObject.name);
    //         // belt and suspenders
    //         if (powerUpObject.TryGetComponent(out DropPowerUp dropPowerUp))
    //         {
    //             dropPowerUp.DisableDrop();
    //         }
    //         powerUpObject.SetActive(false);
    //         Destroy(powerUpObject);
    //     }
    // }
}
