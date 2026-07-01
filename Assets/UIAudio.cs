using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    public AudioSource hoverSound;
    public AudioSource clickSound;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayHover()
    {
        hoverSound.Play();
    }

    public void PlayClick()
    {
        clickSound.Play();
    }
}