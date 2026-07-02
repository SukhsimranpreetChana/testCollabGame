using UnityEngine;
using VHS;

public class ForcedLook : MonoBehaviour
{
    [Header("Timing")]
    public float lerpTime;
    public float rotationDuration = 10f;
    public float pullStrength = 4f;

    [Header("Look Target")]
    public Transform target;

    [Tooltip("Optional: assign the monster object or monster look-at target object here. It will disappear when forced look finishes.")]
    public GameObject objectToHideWhenFinished;

    [Header("References")]
    public Behaviour playerMovement;
    public CameraController cameraController;
    public Behaviour camBreathe;

    [Header("Door Unlock")]
    public bool unlockDoorWhenFinished = true;
    public AutoDoor exitDoor;
    public GameObject teleporter;

    [Header("Monster Animation")]
    public Animator monsterAnimator;
    public string monsterAnimationTrigger = "PlayScare";
    public bool playMonsterAnimation = false;

    public bool isRotating = false;

    public void StartForcedLook()
    {
        Debug.Log("FORCED LOOK STARTED");

        if (target == null)
        {
            Debug.LogWarning("ForcedLook has no target assigned.");
            return;
        }

        if (cameraController == null)
        {
            Debug.LogWarning("ForcedLook has no CameraController assigned.");
            return;
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (camBreathe != null)
            camBreathe.enabled = false;

        if (playMonsterAnimation && monsterAnimator != null)
            monsterAnimator.SetTrigger(monsterAnimationTrigger);

        cameraController.StartForcedLook(target, pullStrength);

        isRotating = true;
        lerpTime = 0f;
    }

    private void Update()
    {
        if (!isRotating)
            return;

        lerpTime += Time.deltaTime;

        if (lerpTime >= rotationDuration)
        {
            FinishForcedLook();
        }
    }

    private void FinishForcedLook()
    {
        isRotating = false;

        if (cameraController != null)
            cameraController.StopForcedLook();

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (camBreathe != null)
            camBreathe.enabled = true;

        if (objectToHideWhenFinished != null)
            objectToHideWhenFinished.SetActive(false);

        if (unlockDoorWhenFinished)
        {
            if (exitDoor != null)
                exitDoor.locked = false;

            if (teleporter != null)
                teleporter.SetActive(true);
        }

        Debug.Log("FORCED LOOK FINISHED - TARGET HIDDEN AND DOOR UNLOCKED");
    }
}
