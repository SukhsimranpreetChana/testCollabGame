using UnityEngine;

public class CrosshairInteract : MonoBehaviour
{
    [Header("Crosshair Objects")]
    public GameObject crosshair;
    public GameObject crosshairHit;

    [Header("Interaction")]
    public string interactableTag = "Interactable";
    public float interactDistance = 3f;

    [Header("Crosshair Animation")]
    public float hoverSizeMultiplier = 1.5f;
    public float lerpSpeed = 5f;

    [Header("Camera")]
    public Camera playerCamera;

    private RectTransform normalCrosshairRect;
    private RectTransform hitCrosshairRect;

    private Vector2 normalInitialSize;
    private Vector2 hitInitialSize;

    private bool isLookingAtInteractable = false;

    private void Start()
    {
        if (crosshair != null)
        {
            normalCrosshairRect = crosshair.GetComponent<RectTransform>();

            if (normalCrosshairRect != null)
                normalInitialSize = normalCrosshairRect.sizeDelta;
        }

        if (crosshairHit != null)
        {
            hitCrosshairRect = crosshairHit.GetComponent<RectTransform>();

            if (hitCrosshairRect != null)
                hitInitialSize = hitCrosshairRect.sizeDelta;
        }

        SetNormalCrosshairInstant();
    }

    private void Update()
    {
        CheckForInteractable();
        AnimateCrosshair();
    }

    private void CheckForInteractable()
    {
        isLookingAtInteractable = false;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                isLookingAtInteractable = true;
            }
        }
    }

    private void AnimateCrosshair()
    {
        if (isLookingAtInteractable)
        {
            ShowInteractableCrosshair();

            if (hitCrosshairRect != null)
            {
                Vector2 targetSize =
                    hitInitialSize * hoverSizeMultiplier;

                hitCrosshairRect.sizeDelta = Vector2.Lerp(
                    hitCrosshairRect.sizeDelta,
                    targetSize,
                    Time.deltaTime * lerpSpeed
                );
            }
        }
        else
        {
            ShowNormalCrosshair();

            if (normalCrosshairRect != null)
            {
                normalCrosshairRect.sizeDelta = Vector2.Lerp(
                    normalCrosshairRect.sizeDelta,
                    normalInitialSize,
                    Time.deltaTime * lerpSpeed
                );
            }
        }
    }

    private void ShowNormalCrosshair()
    {
        if (crosshair != null)
            crosshair.SetActive(true);

        if (crosshairHit != null)
            crosshairHit.SetActive(false);

        if (hitCrosshairRect != null)
            hitCrosshairRect.sizeDelta = hitInitialSize;
    }

    private void ShowInteractableCrosshair()
    {
        if (crosshair != null)
            crosshair.SetActive(false);

        if (crosshairHit != null)
            crosshairHit.SetActive(true);
    }

    private void SetNormalCrosshairInstant()
    {
        if (crosshair != null)
            crosshair.SetActive(true);

        if (crosshairHit != null)
            crosshairHit.SetActive(false);

        if (normalCrosshairRect != null)
            normalCrosshairRect.sizeDelta = normalInitialSize;

        if (hitCrosshairRect != null)
            hitCrosshairRect.sizeDelta = hitInitialSize;
    }
}