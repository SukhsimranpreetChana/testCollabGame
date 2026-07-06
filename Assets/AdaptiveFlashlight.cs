using UnityEngine;

public class AdaptiveFlashlight : MonoBehaviour
{
    [Header("Flashlight")]
    public Light flashlight;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Intensity")]
    public float closeIntensity = 1.5f;
    public float farIntensity = 15f;

    [Header("Distance")]
    public float closeDistance = 1f;
    public float farDistance = 15f;

    [Header("Smoothing")]
    public float smoothSpeed = 5f;

    [Header("Raycast")]
    public LayerMask flashlightLayers = ~0;

    private void Update()
    {
        UpdateFlashlightIntensity();
    }

    private void UpdateFlashlightIntensity()
    {
        if (flashlight == null || playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        float hitDistance = farDistance;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            farDistance,
            flashlightLayers,
            QueryTriggerInteraction.Ignore))
        {
            hitDistance = hit.distance;
        }

        float distancePercent = Mathf.InverseLerp(
            closeDistance,
            farDistance,
            hitDistance
        );

        float targetIntensity = Mathf.Lerp(
            closeIntensity,
            farIntensity,
            distancePercent
        );

        flashlight.intensity = Mathf.Lerp(
            flashlight.intensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed
        );
    }
}