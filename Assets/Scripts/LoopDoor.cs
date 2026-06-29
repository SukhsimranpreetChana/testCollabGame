using UnityEngine;

public class LoopDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        resetBack sequence = other.GetComponent<resetBack>();

        if (sequence != null)
        {
            sequence.StartDoorSequence();
        }


    }
}