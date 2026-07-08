using UnityEngine;
using System.Collections;

public class MainMenuLightFlicker : MonoBehaviour
{
    [Header("Light Objects")]
    public GameObject lightOn;
    public GameObject lightOff;

    [Header("Random Timing")]
    public float minTimeBetweenFlickers = 0f;
    public float maxTimeBetweenFlickers = 5f;

    [Header("Flicker Burst")]
    public float minFlickerSpeed = 0.03f;
    public float maxFlickerSpeed = 0.12f;
    public int minFlickersPerBurst = 2;
    public int maxFlickersPerBurst = 6;

    private Coroutine flickerCoroutine;

    private void OnEnable()
    {
        StartFlicker();
    }

    private void OnDisable()
    {
        StopFlicker();
    }

    public void StartFlicker()
    {
        StopFlicker();
        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    public void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        SetLight(true);
    }

    private IEnumerator FlickerRoutine()
    {
        SetLight(true);

        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers)
            );

            int flickerCount = Random.Range(
                minFlickersPerBurst,
                maxFlickersPerBurst + 1
            );

            for (int i = 0; i < flickerCount; i++)
            {
                SetLight(false);

                yield return new WaitForSeconds(
                    Random.Range(minFlickerSpeed, maxFlickerSpeed)
                );

                SetLight(true);

                yield return new WaitForSeconds(
                    Random.Range(minFlickerSpeed, maxFlickerSpeed)
                );
            }
        }
    }

    private void SetLight(bool on)
    {
        if (lightOn != null)
            lightOn.SetActive(on);

        if (lightOff != null)
            lightOff.SetActive(!on);
    }
}