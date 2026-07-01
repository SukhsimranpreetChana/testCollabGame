using UnityEngine;
using UnityEngine.UI;

public class CrosshairInteract : MonoBehaviour
{
    public RawImage crosshairImage;
    public string interactableTag = "Interactable";
    public float hoverSizeMultiplier = 1.5f; // Multiplier for crosshair size when hovering
    public float hoverOpacity = 1.0f; // Opacity when hovering
    public float lerpSpeed = 5f; // Speed of interpolation

    private Color initialColor;
    private float initialSize;

    void Start()
    {
        initialColor = crosshairImage.color;
        initialSize = crosshairImage.rectTransform.sizeDelta.x; // Assuming the crosshair is square
    }

    void Update()
    {
        // Get the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Cast a ray from the camera's center through the screen point
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                // Object with interactable tag is hit, interpolate to hover state
                InterpolateToHoverState();
            }
            else
            {
                // No interactable object is hit, interpolate to idle state
                InterpolateToIdleState();
            }
        }
        else
        {
            // No object is hit, interpolate to idle state
            InterpolateToIdleState();
        }
    }

    void InterpolateToHoverState()
    {
        float targetOpacity = hoverOpacity;
        float currentOpacity = Mathf.Lerp(crosshairImage.color.a, targetOpacity, Time.deltaTime * lerpSpeed);
        crosshairImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentOpacity);

        float targetSize = initialSize * hoverSizeMultiplier;
        float currentSize = Mathf.Lerp(crosshairImage.rectTransform.sizeDelta.x, targetSize, Time.deltaTime * lerpSpeed);
        crosshairImage.rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    void InterpolateToIdleState()
    {
        float targetOpacity = 0.5f; // 50% opacity when not hovering
        float currentOpacity = Mathf.Lerp(crosshairImage.color.a, targetOpacity, Time.deltaTime * lerpSpeed);
        crosshairImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentOpacity);

        float targetSize = initialSize;
        float currentSize = Mathf.Lerp(crosshairImage.rectTransform.sizeDelta.x, targetSize, Time.deltaTime * lerpSpeed);
        crosshairImage.rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
