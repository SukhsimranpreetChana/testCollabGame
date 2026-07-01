using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public Transform door;          
    public Vector3 closedRotation;
    public Vector3 openRotation = new Vector3(0, 90, 0);

    public float openSpeed = 4f;
    public float closeDelay = 1f;

    private bool playerNear = false;
    private float closeTimer = 0f;

    public AudioSource doorSfx;

    private void Start()
    {
        closedRotation = door.eulerAngles;
    }

    private void Update()
    {
        Quaternion targetRotation;

        if (playerNear)
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
        if (other.CompareTag("Player"))
        {
            playerNear = true;
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
}