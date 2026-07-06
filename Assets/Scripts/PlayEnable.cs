using UnityEngine;

public class PlayAudioOnEnable : MonoBehaviour
{
    public AudioSource audioSource;
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlaySound();
    }

    public void PlaySound()
    {
        if (audioSource == null || audioSource.clip == null)
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        hasPlayed = true;
        audioSource.Play();
    }
}