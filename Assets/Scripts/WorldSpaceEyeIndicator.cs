using UnityEngine;

public class WorldSpaceEyeIndicator : MonoBehaviour
{
    [Header("Target")]
    public Transform playerCamera;

    [Header("Size")]
    public float baseScale = 0.01f;
    public float distanceScaleMultiplier = 0.08f;

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        // Always face the player/camera
        transform.LookAt(playerCamera);

        // Flip so the UI faces forward correctly
        transform.Rotate(0f, 180f, 0f);

        // Stay visually similar size from different distances
        float distance = Vector3.Distance(transform.position, playerCamera.position);
        float scale = baseScale * distance * distanceScaleMultiplier;

        transform.localScale = Vector3.one * scale;
    }
}