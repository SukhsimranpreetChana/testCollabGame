using UnityEngine;
using System.Collections;
using VHS;

public class Loop4ToyCar : MonoBehaviour
{
    public HallwayLoopManager loopManager;

    public Behaviour playerMovementScript;
    public CameraController cameraController;

    public ForcedLook toyCarForcedLook;

    public GameObject toyCarObject;
    public Animator carAnimator;
    public string carMoveAnimation = "carMove";

    public AudioSource musicSource;
    public AudioSource thumpSource;

    public float forcedLookTime = 1f;

    public string playerTag = "Player";
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;
    private Coroutine carRoutine;

    private void Start()
    {
        UpdateToyCarActiveState();
    }

    private void Update()
    {
        UpdateToyCarActiveState();
    }

    private void OnDisable()
    {
        StopMusic();
        StopThump();

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
    }

    private void UpdateToyCarActiveState()
    {
        bool shouldBeActive = loopManager != null && loopManager.loopCount == 4;

        if (toyCarObject != null && toyCarObject.activeSelf != shouldBeActive)
            toyCarObject.SetActive(shouldBeActive);

        if (shouldBeActive)
            PlayMusic();
        else
            StopMusic();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (loopManager == null || loopManager.loopCount != 4)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        hasPlayed = true;

        if (carRoutine != null)
            StopCoroutine(carRoutine);

        carRoutine = StartCoroutine(ToyCarRoutine());
    }

    private IEnumerator ToyCarRoutine()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (toyCarForcedLook != null)
            toyCarForcedLook.StartForcedLook();

        PlayThump();

        if (carAnimator != null)
            carAnimator.Play(carMoveAnimation, 0, 0f);

        yield return new WaitForSeconds(forcedLookTime);

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        carRoutine = null;
    }

    private void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    private void PlayThump()
    {
        if (thumpSource == null)
            return;

        thumpSource.loop = false;
        thumpSource.Stop();
        thumpSource.Play();
    }

    private void StopThump()
    {
        if (thumpSource != null && thumpSource.isPlaying)
            thumpSource.Stop();
    }

    public void ResetTrigger()
    {
        hasPlayed = false;

        if (carRoutine != null)
        {
            StopCoroutine(carRoutine);
            carRoutine = null;
        }

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
    }
}