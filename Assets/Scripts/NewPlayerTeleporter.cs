using UnityEngine;
using VHS;

public class NewPlayerTeleporter : MonoBehaviour
{
    public Transform TeleportZoneObject;
    public CameraController cameraController;

    [Header("Door Fix")]
    public AutoDoor doorToClose;

    private bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport || !other.CompareTag("Player")) return;

        canTeleport = false;

        CharacterController cc = other.GetComponent<CharacterController>();

        Vector3 localOffset = transform.InverseTransformPoint(other.transform.position);

        Quaternion relativeRotation =
            Quaternion.Inverse(transform.rotation) *
            cameraController.transform.rotation;

        if (cc != null)
            cc.enabled = false;

        other.transform.position = TeleportZoneObject.TransformPoint(localOffset);

        Quaternion newRotation =
            TeleportZoneObject.rotation * relativeRotation;

        float newYaw = newRotation.eulerAngles.y;

        other.transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        cameraController.TeleportSetYaw(newYaw);

        if (doorToClose != null)
            doorToClose.ForceClose();

        if (cc != null)
            cc.enabled = true;

        Invoke(nameof(ResetTeleport), 0.25f);
    }

    private void ResetTeleport()
    {
        canTeleport = true;
    }
}