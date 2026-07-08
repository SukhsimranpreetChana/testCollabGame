using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialText : MonoBehaviour
{
    [Header("Tutorial Text")]
    public TextMeshProUGUI tutorialText;

    [Header("Player")]
    public CharacterController characterController;

    [Tooltip("Drag your CameraController / camera look script here.")]
    public Behaviour cameraMovementScript;

    [Header("Beginning Player Lock")]
    public bool lockPlayerAtBeginning = true;
    public float beginningLockTime = 17f;

    [Header("Movement Tutorial")]
    public string movementMessage = "WASD to move";
    public float movementTutorialDelay = 2f;

    [Header("Interaction Tutorial")]
    public string interactMessage = "Press E to interact";

    [Header("Fade Settings")]
    public float fadeInTime = 0.5f;
    public float stayTime = 2f;
    public float fadeOutTime = 0.5f;

    private bool showedMovementTutorial = false;
    private bool showedInteractTutorial = false;

    private Coroutine tutorialCoroutine;

    private void Start()
    {
        SetTextAlpha(0f);

        StartCoroutine(MovementTutorialRoutine());

        if (lockPlayerAtBeginning)
            StartCoroutine(BeginningPlayerLockRoutine());
    }

    private IEnumerator BeginningPlayerLockRoutine()
    {
        // Disable player movement.
        if (characterController != null)
            characterController.enabled = false;

        // Disable camera movement.
        if (cameraMovementScript != null)
            cameraMovementScript.enabled = false;

        yield return new WaitForSeconds(beginningLockTime);

        // Re-enable player movement.
        if (characterController != null)
            characterController.enabled = true;

        // Re-enable camera movement.
        if (cameraMovementScript != null)
            cameraMovementScript.enabled = true;
    }

    private IEnumerator MovementTutorialRoutine()
    {
        yield return new WaitForSeconds(movementTutorialDelay);

        if (showedMovementTutorial)
            yield break;

        showedMovementTutorial = true;

        ShowMessage(movementMessage);
    }

    public void ShowInteractTutorial()
    {
        if (showedInteractTutorial)
            return;

        showedInteractTutorial = true;

        ShowMessage(interactMessage);
    }

    public void ShowMessage(string message)
    {
        if (tutorialText == null)
        {
            Debug.LogWarning(
                "TutorialText: Tutorial Text is not assigned."
            );

            return;
        }

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        tutorialCoroutine = StartCoroutine(
            TutorialRoutine(message)
        );
    }

    private IEnumerator TutorialRoutine(string message)
    {
        tutorialText.text = message;

        SetTextAlpha(0f);

        yield return StartCoroutine(
            FadeAlpha(
                0f,
                1f,
                fadeInTime
            )
        );

        yield return new WaitForSeconds(stayTime);

        yield return StartCoroutine(
            FadeAlpha(
                1f,
                0f,
                fadeOutTime
            )
        );

        tutorialText.text = "";

        tutorialCoroutine = null;
    }

    private IEnumerator FadeAlpha(
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (duration <= 0f)
        {
            SetTextAlpha(targetAlpha);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float percent = Mathf.Clamp01(
                timer / duration
            );

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                percent
            );

            SetTextAlpha(alpha);

            yield return null;
        }

        SetTextAlpha(targetAlpha);
    }

    private void SetTextAlpha(float alpha)
    {
        if (tutorialText == null)
            return;

        Color textColor = tutorialText.color;

        textColor.a = alpha;

        tutorialText.color = textColor;
    }
}