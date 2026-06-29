using UnityEngine;

public class DoorAnimationEventRelay : MonoBehaviour
{
    public resetBack playerSequence;

    public void TeleportPlayer()
    {
        playerSequence.TeleportPlayer();
    }
}