using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Header("Door")]
    public Transform door;
    public Vector3 closedRotation;
    public Vector3 openRotation = new Vector3(0, 90, 0);

    public float openSpeed = 4f;
    public float closeDelay = 1f;

    [Header("Lock")]
    public bool locked = false;
    public AudioSource doorSfx;
    public AudioSource lockedSfx;

    [Tooltip("How far the door rattles when locked.")]
    public float lockedShakeAngle = 8f;

    [Tooltip("How fast the door rattles.")]
    public float lockedShakeSpeed = 12f;

    private bool playerNear = false;
    private float closeTimer = 0f;
    private bool shaking = false;

    private void Start()
    {
        closedRotation = door.eulerAngles;
    }

    private void Update()
    {
        // Don't try to animate the normal door while it's shaking.
        if (shaking)
            return;

        Quaternion targetRotation;

        if (!locked && playerNear)
        {
            targetRotation = Quaternion.Euler(openRotation);
            closeTimer = closeDelay;
        }
        else
        {
            closeTimer -= Time.deltaTime;

            targetRotation = closeTimer <= 0f
                ? Quaternion.Euler(closedRotation)
                : door.rotation;
        }

        door.rotation = Quaternion.Slerp(
            door.rotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (locked)
        {
            if (lockedSfx != null)
                lockedSfx.Play();

            if (!shaking)
                StartCoroutine(ShakeLockedDoor());
        }
        else
        {
            playerNear = true;

            if (doorSfx != null)
                doorSfx.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }

    public void ForceClose()
    {
        playerNear = false;
        closeTimer = 0f;
    }

    private IEnumerator ShakeLockedDoor()
    {
        shaking = true;

        Quaternion startRotation = Quaternion.Euler(closedRotation);
        Quaternion shakeRotation = startRotation * Quaternion.Euler(0f, lockedShakeAngle, 0f);

        // Push slightly open
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * lockedShakeSpeed;
            door.rotation = Quaternion.Slerp(startRotation, shakeRotation, t);
            yield return null;
        }

        // Return closed
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * lockedShakeSpeed;
            door.rotation = Quaternion.Slerp(shakeRotation, startRotation, t);
            yield return null;
        }

        door.rotation = startRotation;
        shaking = false;
    }
}