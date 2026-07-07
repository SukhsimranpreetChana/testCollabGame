using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Coroutine shakeCoroutine;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void StartShake(float duration, float strength, float speed)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength, speed));
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition = originalLocalPosition;
    }

    private IEnumerator ShakeRoutine(float duration, float strength, float speed)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * 2f - 1f) * strength;
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * 2f - 1f) * strength;

            float fade = 1f - Mathf.Clamp01(timer / duration);

            transform.localPosition = originalLocalPosition + new Vector3(x * fade, y * fade, 0f);

            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }

    private void OnDisable()
    {
        StopShake();
    }
}
