using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when the Play button is pressed

    public Image fadeImage;
    public GameObject fade;
    public float fadeDuration = 1f;
    public void PlayGame()
    {
        // Loads the next scene in the Build Settings
        FadeToNextScene();
    }

    // Called when the Settings button is pressed
    public void OpenSettings()
    {
        Debug.Log("Settings button pressed.");

        // TODO:
        // settingsPanel.SetActive(true);
    }

    // Called when the Quit button is pressed
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void FadeToNextScene()
    {
        fade.SetActive(true);
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

