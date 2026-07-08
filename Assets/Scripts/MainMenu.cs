using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public GameObject fade;
    public float fadeDuration = 1f;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        // Important when returning from a paused game.
        Time.timeScale = 1f;

        // Make sure the mouse works in the menu.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        FadeToNextScene();
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button pressed.");

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Debug.Log("Closing settings.");

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    // MAIN MENU QUIT BUTTON
    // Completely closes the game.
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // PAUSE MENU QUIT BUTTON
    // Returns the player to the main menu.
    public void QuitToMainMenu()
    {
        Debug.Log("Returning to main menu...");

        // Fix game being paused.
        Time.timeScale = 1f;

        // Unlock mouse before loading the menu.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void FadeToNextScene()
    {
        if (fade != null)
            fade.SetActive(true);

        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        float elapsed = 0f;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                color.a = Mathf.Lerp(
                    0f,
                    1f,
                    elapsed / fadeDuration
                );

                fadeImage.color = color;

                yield return null;
            }

            color.a = 1f;
            fadeImage.color = color;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }
}