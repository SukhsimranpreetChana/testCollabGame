using UnityEngine;

public class Loop3MonsterTrigger : MonoBehaviour
{
    [Header("Loop Manager")]
    public HallwayLoopManager loopManager;

    [Header("Monster")]
    public Animator monsterAnimator;
    public AudioSource monsterAudioSource;

    [Header("Animation")]
    public string monsterAnimation = "tryANim";

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only work during Loop 3
        if (loopManager == null || loopManager.loopCount != 3)
            return;

        // Make sure the Player entered
        if (!other.CompareTag(playerTag))
            return;

        // Don't trigger again
        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        PlayMonsterAnimation();
    }

    private void PlayMonsterAnimation()
    {
        // Play tryANim
        if (monsterAnimator != null)
        {
            monsterAnimator.Play(
                monsterAnimation,
                0,
                0f
            );
        }

        // Stop monster audio
        if (monsterAudioSource != null)
        {
            monsterAudioSource.Stop();

            // Disable the AudioSource component
            monsterAudioSource.enabled = false;
        }

        Debug.Log(
            "Loop 3: Monster triggered. Playing "
            + monsterAnimation
            + " and disabling monster audio."
        );
    }

    public void ResetTrigger()
    {
        hasTriggered = false;

        if (monsterAudioSource != null)
            monsterAudioSource.enabled = true;
    }
}