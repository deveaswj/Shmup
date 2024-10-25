using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        Debug.Log("LM: Level 1");
        ChangeScene("Level 1");
    }

    public void LoadMainMenu()
    {
        Debug.Log("LM: Main Menu");
        ChangeScene("Main Menu");
    }

    public void LoadGameOver(float delay = 3f)
    {
        Debug.Log("LM: Game Over");
        StartCoroutine(DelayLoadScene("Game Over", delay));
    }

    private IEnumerator DelayLoadScene(string sceneName, float delay)
    {
        CleanupPowerUps();
        yield return new WaitForSeconds(delay);
        ChangeScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("LM: Quitting Game");
        Application.Quit();
    }

    private void ChangeScene(string sceneName)
    {
        CleanupPowerUps();
        SceneManager.LoadScene(sceneName);
    }

    private void CleanupPowerUps()
    {
        GameObject[] powerUpObjects = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (var powerUpObject in powerUpObjects)
        {
            Debug.Log("LM: Destroying " + powerUpObject.name);
            Destroy(powerUpObject);
        }
    }
}
