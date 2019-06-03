using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefinitiveScript;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    private bool previouslyLockedCursor;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        GameManager.Instance.LocalPlayer.stopInput = false;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        if(previouslyLockedCursor) GameManager.Instance.CursorController.LockCursor();
        previouslyLockedCursor = false;
    }
    private void Pause()
    {
        GameManager.Instance.LocalPlayer.stopInput = true;
        previouslyLockedCursor = GameManager.Instance.CursorController.LockedCursor();

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        GameManager.Instance.CursorController.UnlockCursor();
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SceneController.BackToMenu();
    }
}
