using UnityEngine;
using System.Collections;

public class Loop7Script : MonoBehaviour
{
    public HallwayLoopManager loopManager;

    public Behaviour playerMovementScript;
    public Behaviour cameraLookScript;
    public CharacterController characterController;

    public GameObject indicatorObject;
    public float indicatorTime = 5f;
    public string playerTag = "Player";

    private bool triggered = false;

    private void Start()
    {
        if (indicatorObject != null)
            indicatorObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (loopManager == null || loopManager.loopCount != 7)
            return;

        if (!other.CompareTag(playerTag))
            return;

        triggered = true;

        if (characterController == null)
            characterController = other.GetComponent<CharacterController>();

        StartCoroutine(ShowIndicator());
    }

    private IEnumerator ShowIndicator()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraLookScript != null)
            cameraLookScript.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        if (indicatorObject != null)
            indicatorObject.SetActive(true);

        yield return new WaitForSeconds(indicatorTime);

        if (indicatorObject != null)
            indicatorObject.SetActive(false);

        if (characterController != null)
            characterController.enabled = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraLookScript != null)
            cameraLookScript.enabled = true;
    }
}