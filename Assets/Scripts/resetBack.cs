using System.Collections;
using UnityEngine;

public class resetBack : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator doorAnimator;

    public Transform endDoorPoint;
    public Transform startDoorPoint;

    [Header("Look At Door")]
    public Transform lookTarget;
    public Transform cameraTransform;
    public float lookDuration = 0.25f;

    public VHS.CameraController cameraController;
    public VHS.FirstPersonController playerMovement;

    public int loopNum = 0;

    private CharacterController controller;
    private bool sequencePlaying = false;

    private Quaternion savedPlayerRotation;
    private Quaternion savedCameraLocalRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void StartDoorSequence()
    {
        if (sequencePlaying) return;

        sequencePlaying = true;
        StartCoroutine(DoorSequenceRoutine());
    }

    IEnumerator DoorSequenceRoutine()
    {
        playerMovement.enabled = false;
        cameraController.enabled = false;

        savedPlayerRotation = transform.rotation;
        savedCameraLocalRotation = cameraTransform.localRotation;

        yield return StartCoroutine(SmoothLookAtDoor());

        playerAnimator.ResetTrigger("EnterDoor");
        doorAnimator.ResetTrigger("Open");

        playerAnimator.SetTrigger("EnterDoor");
        doorAnimator.SetTrigger("Open");

        Invoke(nameof(EndSequence), 0.45f);
    }

    IEnumerator SmoothLookAtDoor()
    {
        Quaternion startPlayerRot = transform.rotation;
        Quaternion startCameraRot = cameraTransform.localRotation;

        Vector3 flatDirection = lookTarget.position - transform.position;
        flatDirection.y = 0f;

        Quaternion targetPlayerRot = Quaternion.LookRotation(flatDirection);

        Vector3 camDirection = lookTarget.position - cameraTransform.position;
        Quaternion targetWorldCameraRot = Quaternion.LookRotation(camDirection);

        Quaternion targetCameraLocalRot =
            Quaternion.Inverse(targetPlayerRot) * targetWorldCameraRot;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / lookDuration;
            float smoothT = t * t * (3f - 2f * t);

            transform.rotation = Quaternion.Slerp(
                startPlayerRot,
                targetPlayerRot,
                smoothT
            );

            cameraTransform.localRotation = Quaternion.Slerp(
                startCameraRot,
                targetCameraLocalRot,
                smoothT
            );

            yield return null;
        }

        transform.rotation = targetPlayerRot;
        cameraTransform.localRotation = targetCameraLocalRot;
    }

    // Called by the DOOR animation event
    public void TeleportPlayer()
    {
        if (controller != null)
            controller.enabled = false;

        Vector3 offset = endDoorPoint.InverseTransformPoint(transform.position);
        transform.position = startDoorPoint.TransformPoint(offset);

        Quaternion rotOffset = Quaternion.Inverse(endDoorPoint.rotation) * transform.rotation;
        transform.rotation = startDoorPoint.rotation * rotOffset;

        cameraTransform.localRotation = Quaternion.identity;

        if (controller != null)
            controller.enabled = true;
    }

    public void EndSequence()
    {
        CancelInvoke(nameof(EndSequence));

        cameraTransform.localRotation = Quaternion.identity;

        sequencePlaying = false;

        playerMovement.enabled = true;
        cameraController.enabled = true;

        loopNum++;
    }
}