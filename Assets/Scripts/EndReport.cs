using UnityEngine;
using System.Collections;

public class EndReport : MonoBehaviour
{
    [Header("TV Audio Source")]
    public AudioSource tvAudioSource;

    [Header("Player Audio Source")]
    public AudioSource playerAudioSource;

    [Header("Report Audio")]
    public AudioClip normalReport;
    public AudioClip secondReport;
    public AudioClip callReport;
    public AudioClip end;

    [Header("Player Audio")]
    public AudioClip whyDidntYouAnswer;

    [Header("Timing")]
    public float startDelay = 10f;
    public float whyDidntYouAnswerDelay = 73f;

    private Coroutine reportCoroutine;
    private Coroutine playerAudioCoroutine;

    private void OnEnable()
    {
        reportCoroutine = StartCoroutine(PlayReports());
        playerAudioCoroutine = StartCoroutine(PlayDelayedPlayerAudio());
    }

    private void OnDisable()
    {
        if (reportCoroutine != null)
        {
            StopCoroutine(reportCoroutine);
            reportCoroutine = null;
        }

        if (playerAudioCoroutine != null)
        {
            StopCoroutine(playerAudioCoroutine);
            playerAudioCoroutine = null;
        }

        if (tvAudioSource != null)
            tvAudioSource.Stop();

        if (playerAudioSource != null)
            playerAudioSource.Stop();
    }

    private IEnumerator PlayReports()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(PlayClip(normalReport));
        yield return StartCoroutine(PlayClip(secondReport));
        yield return StartCoroutine(PlayClip(callReport));
        yield return StartCoroutine(PlayClip(end));

        reportCoroutine = null;
    }

    private IEnumerator PlayDelayedPlayerAudio()
    {
        yield return new WaitForSeconds(whyDidntYouAnswerDelay);

        if (playerAudioSource != null && whyDidntYouAnswer != null)
        {
            playerAudioSource.Stop();
            playerAudioSource.clip = whyDidntYouAnswer;
            playerAudioSource.loop = false;
            playerAudioSource.time = 0f;
            playerAudioSource.Play();
        }

        playerAudioCoroutine = null;
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        if (tvAudioSource == null || clip == null)
            yield break;

        tvAudioSource.Stop();
        tvAudioSource.clip = clip;
        tvAudioSource.loop = false;
        tvAudioSource.time = 0f;
        tvAudioSource.Play();

        yield return new WaitWhile(() =>
            tvAudioSource != null &&
            tvAudioSource.isPlaying
        );
    }
}