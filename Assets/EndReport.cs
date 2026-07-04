using UnityEngine;
using System.Collections;

public class EndReport : MonoBehaviour
{
    [Header("TV Audio Source")]
    public AudioSource tvAudioSource;

    [Header("Report Audio")]
    public AudioClip normalReport;
    public AudioClip secondReport;
    public AudioClip callReport;
    public AudioClip end;

    [Header("Timing")]
    public float startDelay = 10f;

    private Coroutine reportCoroutine;

    private void OnEnable()
    {
        reportCoroutine = StartCoroutine(PlayReports());
    }

    private void OnDisable()
    {
        if (reportCoroutine != null)
        {
            StopCoroutine(reportCoroutine);
            reportCoroutine = null;
        }

        if (tvAudioSource != null)
            tvAudioSource.Stop();
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