using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SoundMenuController : MonoBehaviour
{
    [SerializeField] private GameObject soundMenuUI;
    private bool isSoundMenuOpen = true;

    private void Awake()
    {
        Time.timeScale = 0f; // Pause the game at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSoundMenu();
        }
    }

    public void RestartScene()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Restarting scene: " + currentScene.name);
        SceneManager.LoadScene(currentScene.name);
    }

    public void ResumeGame()
    {
        ToggleSoundMenu();
    }

    void ToggleSoundMenu()
    {
        isSoundMenuOpen = !isSoundMenuOpen;
        soundMenuUI.SetActive(isSoundMenuOpen);

        if(isSoundMenuOpen)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }
}
