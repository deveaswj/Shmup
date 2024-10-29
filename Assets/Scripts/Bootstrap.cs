using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [Tooltip("The name of the scene to load on startup")]
    [SerializeField] private string startingScene = "Main Menu";

    private void Start()
    {
        LevelManager.Instance.LoadSceneByName(startingScene);
    }
}
