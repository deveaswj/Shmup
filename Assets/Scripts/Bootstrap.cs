using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [Tooltip("The name of the scene to load on startup")]
    [SerializeField] private string startingScene = "Main Menu";
    LevelManager levelManager;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start()
    {
        levelManager.LoadSceneByName(startingScene);
    }
}
