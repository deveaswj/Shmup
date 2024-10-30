using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LevelManager : MonoBehaviour
{

    [SerializeField] GameState gameState;

    // public int CurrentLevel { get; private set; } = 1;

    private int thisSceneLevel = 0;

    private void Start()
    {
        SetThisSceneLevel();
    }

    private void SetThisSceneLevel()
    {
        // Get the current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // Use regex to check if this scene's name starts with "Level"
        // followed by optional spaces, leading zeros, and a number
        Regex levelRegex = new(@"^Level\s*0*(\d+)$");
        Match match = levelRegex.Match(sceneName);

        if (match.Success)
        {
            // If a match is found, parse the level number
            thisSceneLevel = int.Parse(match.Groups[1].Value);

            // Update CurrentLevel in GameState only if thisSceneLevel is valid
            gameState.SetLevel(thisSceneLevel);
            Debug.Log("Current Level set to: " + gameState.GetLevel());
        }
        else
        {
            // If no valid level number is found, set thisSceneLevel to 0 or some default value
            thisSceneLevel = 0;
            Debug.Log("Non-level scene detected. thisSceneLevel not set.");
        }
    }

    public void NewGame()
    {
        Debug.Log("LM: New Game");
        gameState.ResetLevel();
        gameState.ResetScore();
        LoadCurrentLevel();
    }

    public void LoadNextLevel()
    {
        Debug.Log("LM: Next Level");
        gameState.NextLevel();
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        string sceneName = "Level " + gameState.GetLevel();
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
        // delay loading the gameover until the player death effects are done
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
        Debug.Log("LM: Loading " + sceneName);
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
