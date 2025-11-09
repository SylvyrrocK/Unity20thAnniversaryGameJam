using UnityEngine;
using UnityEngine.InputSystem;

public class SoundMenuController : MonoBehaviour
{
    [SerializeField] private GameObject soundMenuUI;
    private bool isSoundMenuOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSoundMenu();
        }
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
