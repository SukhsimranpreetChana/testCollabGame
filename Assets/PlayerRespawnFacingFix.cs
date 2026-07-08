using UnityEngine;
using VHS;

public class PlayerRespawnFacingFix : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public CharacterController characterController;
    public CameraController cameraController;

    [Header("Respawn Point")]
    public Transform respawnPoint;

    [Header("Facing")]
    [Tooltip("Use the respawn point's Y rotation as the forward direction.")]
    public bool useRespawnPointYaw = true;

    [Tooltip("Used only if Use Respawn Point Yaw is false.")]
    public float manualYaw = 0f;

    [Tooltip("Add this if the player still faces 90 degrees off. Try 90 or -90.")]
    public float yawOffset = 0f;

    [Header("Options")]
    public bool resetPitch = true;
    public bool stopForcedLook = true;

    public void RespawnPlayer()
    {
        if (playerRoot == null)
        {
            Debug.LogWarning("Respawn failed: Player Root is not assigned.");
            return;
        }

        if (respawnPoint == null)
        {
            Debug.LogWarning("Respawn failed: Respawn Point is not assigned.");
            return;
        }

        bool controllerWasEnabled = false;

        if (characterController != null)
        {
            controllerWasEnabled = characterController.enabled;
            characterController.enabled = false;
        }

        playerRoot.position = respawnPoint.position;

        float targetYaw = useRespawnPointYaw
            ? respawnPoint.eulerAngles.y
            : manualYaw;

        targetYaw += yawOffset;

        playerRoot.rotation = Quaternion.Euler(0f, targetYaw, 0f);

        if (cameraController != null)
        {
            if (stopForcedLook)
                cameraController.StopForcedLook();

            if (resetPitch)
            {
                cameraController.SetLookRotation(
                    Quaternion.Euler(0f, targetYaw, 0f)
                );
            }
            else
            {
                cameraController.TeleportSetYaw(targetYaw);
            }
        }

        if (characterController != null)
            characterController.enabled = controllerWasEnabled;

        Debug.Log("Respawn complete. Facing yaw: " + targetYaw);
    }
}
