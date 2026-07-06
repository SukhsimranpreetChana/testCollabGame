using UnityEngine;
using System.Collections;

public class Loop3MonsterTrigger : MonoBehaviour
{
    [Header("Loop Manager")]
    public HallwayLoopManager loopManager;

    [Header("Monster")]
    public Animator monsterAnimator;
    public AudioSource monsterAudioSource;

    [Header("Player")]
    public Behaviour playerMovementScript;
    public CharacterController playerCharacterController;
    public Rigidbody playerRigidbody;

    [Header("Player Audio")]
    public AudioSource playerAudioSource;
    public AudioClip monsterSeen;

    [Header("Animation")]
    public string monsterAnimation = "tryANim";

    [Header("Movement Stop")]
    public float movementStopTime = 3f;

    [Header("Audio Fade")]
    public float fadeOutDuration = 1.5f;

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private Coroutine monsterRoutine;

    private float originalPlayerAudioVolume = 1f;

    private void Start()
    {
        if (playerAudioSource != null)
        {
            originalPlayerAudioVolume = playerAudioSource.volume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (loopManager == null || loopManager.loopCount != 3)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (playerCharacterController == null)
        {
            playerCharacterController =
                other.GetComponent<CharacterController>();
        }

        if (playerRigidbody == null)
        {
            playerRigidbody =
                other.GetComponent<Rigidbody>();
        }

        if (monsterRoutine != null)
        {
            StopCoroutine(monsterRoutine);
        }

        monsterRoutine = StartCoroutine(
            PlayMonsterSequence()
        );
    }

    private IEnumerator PlayMonsterSequence()
    {
        // Stop player movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Disable CharacterController
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = false;
        }

        // Stop Rigidbody movement
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // Play monster animation
        if (monsterAnimator != null)
        {
            monsterAnimator.Play(
                monsterAnimation,
                0,
                0f
            );
        }

        // Stop baby / monster audio
        if (monsterAudioSource != null)
        {
            monsterAudioSource.Stop();
            monsterAudioSource.enabled = false;
        }

        // Play monsterSeen audio
        if (playerAudioSource != null && monsterSeen != null)
        {
            playerAudioSource.Stop();

            playerAudioSource.volume =
                originalPlayerAudioVolume;

            playerAudioSource.clip = monsterSeen;
            playerAudioSource.loop = false;
            playerAudioSource.time = 0f;

            playerAudioSource.Play();
        }

        // Wait before fading
        float waitBeforeFade =
            movementStopTime - fadeOutDuration;

        if (waitBeforeFade > 0f)
        {
            yield return new WaitForSeconds(
                waitBeforeFade
            );
        }

        // Fade monsterSeen audio
        if (playerAudioSource != null &&
            playerAudioSource.isPlaying)
        {
            yield return StartCoroutine(
                FadeOutPlayerAudio()
            );
        }

        // Restore CharacterController
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = true;
        }

        // Restore movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        monsterRoutine = null;
    }

    private IEnumerator FadeOutPlayerAudio()
    {
        if (playerAudioSource == null)
            yield break;

        float startVolume =
            playerAudioSource.volume;

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float fadeAmount =
                timer / fadeOutDuration;

            playerAudioSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                fadeAmount
            );

            yield return null;
        }

        playerAudioSource.volume = 0f;

        playerAudioSource.Stop();

        // Restore volume for the next audio clip
        playerAudioSource.volume =
            originalPlayerAudioVolume;
    }

    public void ResetTrigger()
    {
        hasTriggered = false;

        if (monsterRoutine != null)
        {
            StopCoroutine(monsterRoutine);
            monsterRoutine = null;
        }

        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = true;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (monsterAudioSource != null)
        {
            monsterAudioSource.enabled = true;
        }

        if (playerAudioSource != null)
        {
            playerAudioSource.volume =
                originalPlayerAudioVolume;
        }
    }
}