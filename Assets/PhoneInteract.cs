using UnityEngine;

public class PhoneInteract : MonoBehaviour, IInteractable
{
    public HallwayLoopManager loopManager;

    public void Interact()
    {
        loopManager.AnswerPhone();
    }
}