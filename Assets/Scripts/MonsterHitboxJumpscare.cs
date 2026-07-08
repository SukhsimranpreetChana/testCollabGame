using UnityEngine;
using System.Collections;
using VHS;

public class MonsterHitboxJumpscare : MonoBehaviour
{
    public HallwayLoopManager loopManager;

    [Header("Player")]
    public Transform player;
    public Transform loop5RespawnPoint;
    public Behaviour playerMovementScript;
    public CameraController cameraController;
    public CharacterController characterController;

    [Header("Loop 6 Monster Objects")]
    public GameObject monster;
    public GameObject monsterChase;

    [Header("Chase Music")]
    public AudioSource chaseMusic;
    public float musicFadeOutTime = 2f;

    [Header("Jumpscare")]
    public GameObject monsterJumpscareObject;
    public Animator monsterJumpscareAnimator;
    public string jumpscareAnimationName = "jumpscare";
    public float jumpscareDuration = 3f;

    [Header("Lights")]
    public GameObject lightsOn;
    public GameObject lightsOff;
    public GameObject redLights;

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool onlyDuringLoop6 = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (loopManager == null)
            return;

        if (onlyDuringLoop6 && loopManager.loopCount != 6)
            return;

        triggered = true;
        StartCoroutine(JumpscareRoutine());
    }

    private IEnumerator JumpscareRoutine()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraController != null)
        {
            cameraController.StopForcedLook();
            cameraController.enabled = false;
        }

        if (characterController != null)
            characterController.enabled = false;

        if (monster != null)
            monster.SetActive(false);

        if (monsterChase != null)
            monsterChase.SetActive(false);

        if (lightsOn != null)
            lightsOn.SetActive(false);

        if (redLights != null)
            redLights.SetActive(false);

        if (lightsOff != null)
            lightsOff.SetActive(true);

        if (monsterJumpscareObject != null)
            monsterJumpscareObject.SetActive(true);

        if (monsterJumpscareAnimator != null)
            monsterJumpscareAnimator.Play(jumpscareAnimationName, 0, 0f);

        yield return new WaitForSeconds(jumpscareDuration);

        if (monsterJumpscareObject != null)
            monsterJumpscareObject.SetActive(false);

        if (chaseMusic != null)
            StartCoroutine(FadeOutMusic());

        if (loopManager != null)
            loopManager.ForceLoop(5);

        RespawnPlayerFacingForward();

        // Restore normal lights after respawn
        if (lightsOff != null)
            lightsOff.SetActive(false);

        if (redLights != null)
            redLights.SetActive(false);

        if (lightsOn != null)
            lightsOn.SetActive(true);

        if (characterController != null)
            characterController.enabled = true;

        if (cameraController != null)
            cameraController.enabled = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        Time.timeScale = 1f;

        triggered = false;
    }

    private void RespawnPlayerFacingForward()
    {
        if (player == null || loop5RespawnPoint == null)
            return;

        player.position = loop5RespawnPoint.position;

        float yaw = loop5RespawnPoint.eulerAngles.y;
        player.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraController != null)
        {
            cameraController.StopForcedLook();
            cameraController.SetLookRotation(Quaternion.Euler(0f, yaw, 0f));
            cameraController.TeleportSetYaw(yaw);
        }
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = chaseMusic.volume;
        float timer = 0f;

        while (timer < musicFadeOutTime)
        {
            timer += Time.deltaTime;
            chaseMusic.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeOutTime);
            yield return null;
        }

        chaseMusic.Stop();
        chaseMusic.volume = startVolume;
    }
}