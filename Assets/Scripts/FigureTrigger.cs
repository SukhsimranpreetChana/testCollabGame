using UnityEngine;

public class FigureTrigger : MonoBehaviour
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

        loopManager.PlayerReachedFigure();
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}