using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingAnimations : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Animation States")]
    public string endingSceneState = "endingScene";
    public string endingIdleState = "endingIdle";

    [Header("Scene")]
    public string mainMenuScene = "MainMenu";

    [Header("Timing")]
    public float endingSceneLength = 10f;
    public float endingIdleTime = 5f;

    private void OnEnable()
    {
        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        if (animator == null)
            yield break;

        // Play endingScene immediately
        animator.Play(endingSceneState, 0, 0f);

        // Wait until it's finished
        yield return new WaitForSeconds(endingSceneLength);

        // Switch to idle
        animator.Play(endingIdleState, 0, 0f);

        // Stay on idle
        yield return new WaitForSeconds(endingIdleTime);

        // Load main menu
        SceneManager.LoadScene(mainMenuScene);
    }
}