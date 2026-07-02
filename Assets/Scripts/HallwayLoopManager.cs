using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using VHS;

public class HallwayLoopManager : MonoBehaviour
{
    [Header("Loop")]
    public int loopCount = 1;

    [Header("Door")]
    public AutoDoor exitDoor;
    public ForcedLook loop3ForcedLook;

    [Header("Loop Triggers")]
    public FigureTrigger figureTrigger;
    public DoorTrigger doorTrigger;

    [Header("SFX")]
    public AudioSource rainSfx;
    public AudioSource phoneRinging;
    public AudioSource cryingSfx;
    public AudioSource chaseMusic;

    [Header("TV Audio Source")]
    public AudioSource tvAudioSource;

    [Header("TV Audio Clips")]
    public AudioClip normalReport;
    public AudioClip secondReport;
    public AudioClip behindYou;
    public AudioClip glitchyReport;
    public AudioClip questioning;
    public AudioClip allDead;

    [Header("Loop 3 TV Interruption")]
    public float lookBehindInterruptDelay = 6f;

    [Header("Loop 3 Forced Look Behind")]
    public bool forceLookBehind = true;
    public float lookBehindDuration = 5f;
    public float lookBehindSmoothSpeed = 0.125f;
    public Transform monsterLookTarget;
    public Behaviour playerMovementScript;
    public CameraController cameraController;

    [Header("Objects")]
    public GameObject drugs;
    public GameObject missingPeoplePhotos;
    public GameObject hallwayFigure;
    public GameObject monster;
    public GameObject furnitureBlockade;
    public GameObject dirtyWalls;
    public GameObject bloodWalls;
    public GameObject finalEmptyHallway;
    public GameObject teleporter;

    [Header("TV Objects")]
    public GameObject tvStatic;
    public GameObject tvNews;
    public GameObject tvOff;

    [Header("Lights")]
    public GameObject lightsOn;
    public GameObject lightsOff;
    public GameObject redLights;

    [Header("Back Light")]
    public GameObject backLightOn;
    public GameObject backLightOff;
    public float minTimeBetweenFlickers = 0f;
    public float maxTimeBetweenFlickers = 5f;
    public float minFlickerSpeed = 0.03f;
    public float maxFlickerSpeed = 0.12f;
    public int minFlickersPerBurst = 2;
    public int maxFlickersPerBurst = 6;

    [Header("Ending")]
    public string endingSceneName;

    private bool chaseStarted = false;
    private bool playerReachedFigure = false;
    private bool waitingForLookBehind = false;

    private Coroutine backLightFlickerCoroutine;
    private Coroutine loop3BroadcastCoroutine;
    private Coroutine forcedLookCoroutine;

    private void Start()
    {
        ApplyLoop();
    }

    public void NextLoop()
    {
        loopCount++;

        chaseStarted = false;
        playerReachedFigure = false;
        waitingForLookBehind = false;

        if (forcedLookCoroutine != null)
        {
            StopCoroutine(forcedLookCoroutine);
            forcedLookCoroutine = null;
        }

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        if (figureTrigger != null)
            figureTrigger.ResetTrigger();

        if (doorTrigger != null)
            doorTrigger.ResetTrigger();

        Debug.Log("Loop #" + loopCount);
        ApplyLoop();
    }

    private void ApplyLoop()
    {
        StopAllAudio();
        ResetEverything();

        if (exitDoor != null)
            exitDoor.locked = false;

        SetActive(teleporter, true);

        switch (loopCount)
        {
            case 1:
                Loop1();
                break;
            case 2:
                Loop2();
                break;
            case 3:
                Loop3();
                break;
            case 4:
                Loop4();
                break;
            case 5:
                Loop5();
                break;
            case 6:
                Loop6();
                break;
            case 7:
                Loop7();
                break;
            default:
                Ending();
                break;
        }
    }

    private void Loop1()
    {
        SetAllLights(true);
        ShowTVStatic();

        if (rainSfx != null)
            rainSfx.Play();
    }

    private void Loop2()
    {
        SetAllLights(true);
        StartBackLightFlicker();

        ShowTVNews();
        PlayTVClip(normalReport, true);

        SetActive(drugs, true);
        SetActive(missingPeoplePhotos, true);

        LockDoor();

        if (phoneRinging != null)
            phoneRinging.Play();
    }

    private void Loop3()
    {
        SetAllLights(true);

        ShowTVNews();
        PlayTVClip(secondReport, true);

        SetActive(hallwayFigure, true);
        SetActive(dirtyWalls, true);

        LockDoor();

        if (cryingSfx != null)
            cryingSfx.Play();
    }

    private void Loop4()
    {
        SetAllLights(false);

        ShowTVOff();
        StopTVAudio();

        SetActive(furnitureBlockade, true);

        LockDoor();

        if (phoneRinging != null)
            phoneRinging.Play();
    }

    private void Loop5()
    {
        SetAllLights(false);

        ShowTVNews();
        PlayTVClip(glitchyReport, true);
    }

    private void Loop6()
    {
        SetAllLights(true);

        ShowTVStatic();
        PlayTVClip(questioning, true);

        LockDoor();
    }

    private void Loop7()
    {
        SetAllLights(false);

        ShowTVOff();
        PlayTVClip(allDead, true);

        SetActive(finalEmptyHallway, true);
    }

    public void AnswerPhone()
    {
        if (phoneRinging != null)
            phoneRinging.Stop();

        if (loopCount == 2)
        {
            UnlockDoor();
        }

        if (loopCount == 4)
        {
            SetAllLights(true);
            SetActive(bloodWalls, true);
            UnlockDoor();
        }
    }

    public void PlayerReachedFigure()
    {
        if (loopCount != 3 || playerReachedFigure)
            return;

        playerReachedFigure = true;

        StopLoop3BroadcastInterruptions();

        ShowTVOff();
        StopTVAudio();
        SetAllLights(false);

        Debug.Log("Loop 3: Player got too close to the figure.");
    }

    public void PlayerReachedDoorLoop3()
    {
        if (loopCount != 3 || !playerReachedFigure)
            return;

        SetAllLights(true);

        ShowTVNews();
        LockDoor();

        waitingForLookBehind = true;

        StartLoop3BroadcastInterruptions();

        if (loop3ForcedLook != null)
            loop3ForcedLook.StartForcedLook();

        Debug.Log("Loop 3: Lights back on. Broadcast now gets interrupted by look behind you.");
    }

    public void LookedBehind()
    {
        if (loopCount == 3)
        {
            if (!waitingForLookBehind)
                return;

            StopForcedLookBehind();

            if (playerMovementScript != null)
                playerMovementScript.enabled = true;

            if (cameraController != null)
                cameraController.SetForceLooking(false);

            StopLoop3BroadcastInterruptions();
            StopTVAudio();
            UnlockDoor();

            waitingForLookBehind = false;

            Debug.Log("Loop 3 complete. Door unlocked.");
        }

        if (loopCount == 6 && !chaseStarted)
        {
            StartChase();
        }
    }

    public void ReachedDoorInLoop6()
    {
        if (loopCount != 6)
            return;

        StopTVAudio();
        LockDoor();

        Debug.Log("Loop 6: Player reached locked door. Waiting for look behind/chase trigger.");
    }

    private void StartChase()
    {
        chaseStarted = true;

        SetActive(monster, true);

        if (chaseMusic != null)
            chaseMusic.Play();

        UnlockDoor();

        Debug.Log("Loop 6: Chase started. Door unlocked.");
    }

    private void StartForcedLookBehind()
    {
        StopForcedLookBehind();
        forcedLookCoroutine = StartCoroutine(ForceLookBehindRoutine());
    }

    private void StopForcedLookBehind()
    {
        if (forcedLookCoroutine != null)
        {
            StopCoroutine(forcedLookCoroutine);
            forcedLookCoroutine = null;
        }
    }

    private IEnumerator ForceLookBehindRoutine()
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraController != null)
            cameraController.SetForceLooking(true);

        float timer = 0f;

        while (timer < lookBehindDuration)
        {
            timer += Time.deltaTime;

            if (cameraController != null && monsterLookTarget != null)
                cameraController.DriftLookToward(monsterLookTarget, lookBehindSmoothSpeed);

            yield return null;
        }

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        forcedLookCoroutine = null;
    }

    private void StartLoop3BroadcastInterruptions()
    {
        StopLoop3BroadcastInterruptions();
        loop3BroadcastCoroutine = StartCoroutine(Loop3BroadcastWithInterruptions());
    }

    private void StopLoop3BroadcastInterruptions()
    {
        if (loop3BroadcastCoroutine != null)
        {
            StopCoroutine(loop3BroadcastCoroutine);
            loop3BroadcastCoroutine = null;
        }
    }

    private IEnumerator Loop3BroadcastWithInterruptions()
    {
        PlayTVClip(behindYou, false);

        if (behindYou != null)
            yield return new WaitForSeconds(behindYou.length);

        PlayTVClip(secondReport, true);

        while (true)
        {
            yield return new WaitForSeconds(lookBehindInterruptDelay);

            if (tvAudioSource == null || behindYou == null)
                continue;

            float savedTime = 0f;

            if (tvAudioSource.clip == secondReport)
                savedTime = tvAudioSource.time;

            tvAudioSource.Stop();
            tvAudioSource.clip = behindYou;
            tvAudioSource.loop = false;
            tvAudioSource.time = 0f;
            tvAudioSource.Play();

            yield return new WaitForSeconds(behindYou.length);

            if (secondReport != null)
            {
                tvAudioSource.Stop();
                tvAudioSource.clip = secondReport;
                tvAudioSource.loop = true;

                if (secondReport.length > 0f)
                    tvAudioSource.time = savedTime % secondReport.length;

                tvAudioSource.Play();
            }
        }
    }

    private void LockDoor()
    {
        if (exitDoor != null)
            exitDoor.locked = true;

        SetActive(teleporter, false);
    }

    private void UnlockDoor()
    {
        if (exitDoor != null)
            exitDoor.locked = false;

        SetActive(teleporter, true);
    }

    private void ShowTVStatic()
    {
        SetActive(tvStatic, true);
        SetActive(tvNews, false);
        SetActive(tvOff, false);
    }

    private void ShowTVNews()
    {
        SetActive(tvStatic, false);
        SetActive(tvNews, true);
        SetActive(tvOff, false);
    }

    private void ShowTVOff()
    {
        SetActive(tvStatic, false);
        SetActive(tvNews, false);
        SetActive(tvOff, true);
    }

    private void PlayTVClip(AudioClip clip, bool loop = true)
    {
        if (tvAudioSource == null || clip == null)
            return;

        if (tvAudioSource.clip == clip && tvAudioSource.isPlaying)
            return;

        tvAudioSource.Stop();
        tvAudioSource.clip = clip;
        tvAudioSource.loop = loop;
        tvAudioSource.time = 0f;
        tvAudioSource.Play();
    }

    private void StopTVAudio()
    {
        if (tvAudioSource != null)
            tvAudioSource.Stop();
    }

    private void SetAllLights(bool on)
    {
        SetActive(lightsOn, on);
        SetActive(lightsOff, !on);
        SetBackLight(on);
    }

    private void SetBackLight(bool on)
    {
        StopBackLightFlicker();

        SetActive(backLightOn, on);
        SetActive(backLightOff, !on);
    }

    private void StartBackLightFlicker()
    {
        StopBackLightFlicker();
        backLightFlickerCoroutine = StartCoroutine(BackLightFlicker());
    }

    private void StopBackLightFlicker()
    {
        if (backLightFlickerCoroutine != null)
        {
            StopCoroutine(backLightFlickerCoroutine);
            backLightFlickerCoroutine = null;
        }
    }

    private IEnumerator BackLightFlicker()
    {
        while (true)
        {
            SetActive(backLightOn, true);
            SetActive(backLightOff, false);

            yield return new WaitForSeconds(Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers));

            int flickerCount = Random.Range(minFlickersPerBurst, maxFlickersPerBurst + 1);

            for (int i = 0; i < flickerCount; i++)
            {
                SetActive(backLightOn, false);
                SetActive(backLightOff, true);

                yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));

                SetActive(backLightOn, true);
                SetActive(backLightOff, false);

                yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
            }
        }
    }

    private void ResetEverything()
    {
        StopBackLightFlicker();
        StopLoop3BroadcastInterruptions();
        StopForcedLookBehind();

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        SetActive(drugs, false);
        SetActive(missingPeoplePhotos, false);
        SetActive(hallwayFigure, false);
        SetActive(monster, false);
        SetActive(furnitureBlockade, false);
        SetActive(dirtyWalls, false);
        SetActive(bloodWalls, false);
        SetActive(finalEmptyHallway, false);

        SetActive(tvStatic, false);
        SetActive(tvNews, false);
        SetActive(tvOff, false);

        SetActive(lightsOn, false);
        SetActive(lightsOff, false);
        SetActive(redLights, false);

        SetActive(backLightOn, false);
        SetActive(backLightOff, false);
    }

    private void StopAllAudio()
    {
        StopLoop3BroadcastInterruptions();

        StopAudio(rainSfx);
        StopAudio(phoneRinging);
        StopAudio(cryingSfx);
        StopAudio(chaseMusic);
        StopTVAudio();
    }

    private void StopAudio(AudioSource audio)
    {
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }

    private void Ending()
    {
        if (!string.IsNullOrEmpty(endingSceneName))
            SceneManager.LoadScene(endingSceneName);
    }

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }
}
