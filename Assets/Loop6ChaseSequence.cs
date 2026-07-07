using UnityEngine;

public class Loop6ChaseSequence : MonoBehaviour
{
    public HallwayLoopManager loopManager;

    [Header("Monster")]
    public GameObject monsterChase;
    public Animator monsterAnimator;
    public string chaseTrigger = "StartChase";

    [Header("Door")]
    public AutoDoor exitDoor;
    public GameObject teleporter;

    [Header("Audio")]
    public AudioSource tvAudioSource;
    public AudioSource chaseMusic;

    [Header("Lights")]
    public GameObject normalLights;
    public GameObject chaseLights;

    private bool chaseStarted = false;

    public void StartLoop6Chase()
    {
        if (chaseStarted)
            return;

        if (loopManager == null || loopManager.loopCount != 6)
            return;

        chaseStarted = true;

        if (tvAudioSource != null)
            tvAudioSource.Stop();

        if (chaseMusic != null)
            chaseMusic.Play();

        if (normalLights != null)
            normalLights.SetActive(false);

        if (chaseLights != null)
            chaseLights.SetActive(true);

        if (monsterChase != null)
            monsterChase.SetActive(true);

        if (monsterAnimator != null)
            monsterAnimator.SetTrigger(chaseTrigger);

        if (exitDoor != null)
            exitDoor.locked = false;

        if (teleporter != null)
            teleporter.SetActive(true);
    }

    public void ResetChase()
    {
        chaseStarted = false;

        if (monsterChase != null)
            monsterChase.SetActive(false);

        if (chaseMusic != null)
            chaseMusic.Stop();

        if (chaseLights != null)
            chaseLights.SetActive(false);
    }
}