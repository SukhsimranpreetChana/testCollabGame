using UnityEngine;
using VHS;

public class ForcedLook : MonoBehaviour
{
    public float lerpTime;
    public float rotationDuration = 10f;

    [Header("Camera Pull")]
    public float pullStrength = 1.5f;

    public Transform target;

    public Behaviour playerMovement;
    public CameraController cameraController;
    public Behaviour camBreathe;

    public bool isRotating = false;

    public void StartForcedLook()
    {
        if (target == null || cameraController == null)
            return;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (camBreathe != null)
            camBreathe.enabled = false;

        isRotating = true;
        lerpTime = 0f;
    }

    void LateUpdate()
    {
        if (!isRotating || target == null || cameraController == null)
            return;

        lerpTime += Time.deltaTime;

        cameraController.PullYawToward(target, pullStrength);

        if (lerpTime >= rotationDuration)
        {
            isRotating = false;

            if (playerMovement != null)
                playerMovement.enabled = true;

            if (camBreathe != null)
                camBreathe.enabled = true;
        }
    }
}