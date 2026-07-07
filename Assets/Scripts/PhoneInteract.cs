using UnityEngine;

public class PhoneInteract : MonoBehaviour, IInteractable
{
    public HallwayLoopManager loopManager;
    public TutorialText tutorialText;

    private bool showedTutorial = false;

    public void ShowTutorial()
    {
        if (showedTutorial)
            return;

        showedTutorial = true;

        if (tutorialText != null)
            tutorialText.ShowInteractTutorial();
    }

    public void Interact()
    {
        if (loopManager != null)
            loopManager.AnswerPhone();
    }
}