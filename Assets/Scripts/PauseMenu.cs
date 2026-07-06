using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Player Control")]
    public Behaviour playerMovementScript;
    public Behaviour cameraLookScript;

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraLookScript != null)
            cameraLookScript.enabled = false;
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraLookScript != null)
            cameraLookScript.enabled = true;
    }

    public void OpenSettings()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void BackFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}