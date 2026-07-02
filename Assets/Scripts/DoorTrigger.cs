using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public HallwayLoopManager loopManager;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        if (loopManager.loopCount == 3)
            loopManager.PlayerReachedDoorLoop3();

        if (loopManager.loopCount == 6)
            loopManager.ReachedDoorInLoop6();
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}