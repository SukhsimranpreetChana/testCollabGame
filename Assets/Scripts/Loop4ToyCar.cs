using UnityEngine;
using System.Collections;

public class Loop4ToyCar : MonoBehaviour
{
    [Header("Loop Manager")]
    public HallwayLoopManager loopManager;

    [Header("Player")]
    public Behaviour playerMovementScript;

    [Header("Forced Look")]
    public ForcedLook toyCarForcedLook;

    [Header("Toy Car")]
    public Animator carAnimator;
    public string carMoveAnimation = "carMove";

    [Header("Timing")]
    public float forcedLookTime = 5f;

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;
    private Coroutine carRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (loopManager == null)
            return;

        // Only work during Loop 4
        if (loopManager.loopCount != 4)
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
        // Stop player movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Force player to look at toy car
        if (toyCarForcedLook != null)
            toyCarForcedLook.StartForcedLook();

        // Play car animation
        if (carAnimator != null)
            carAnimator.Play(carMoveAnimation, 0, 0f);

        // Wait while car animation / forced look happens
        yield return new WaitForSeconds(forcedLookTime);

        // Give movement back
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        carRoutine = null;
    }

    public void ResetTrigger()
    {
        hasPlayed = false;

        if (carRoutine != null)
        {
            StopCoroutine(carRoutine);
            carRoutine = null;
        }
    }
}