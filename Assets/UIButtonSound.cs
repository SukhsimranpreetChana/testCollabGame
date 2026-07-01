using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIAudio.Instance.PlayHover();
    }

    public void PlayClick()
    {
        UIAudio.Instance.PlayClick();
    }
}