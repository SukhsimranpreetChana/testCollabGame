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

    [Header("Phone Audio Source")]
    public AudioSource phoneVoiceSource;

    [Header("Player Phone Audio")]
    public AudioSource playerPhoneAudioSource;
    public AudioClip pickUpPhone;
    public AudioClip noAnswer;
    public AudioClip hangUpPhone;

    [Header("Player Loop Audio Source")]
    public AudioSource playerLoopAudioSource;
    public AudioClip beginningLoop;
    public AudioClip addedLoop;
    public AudioClip finalLoop;
    public AudioClip endingLoop;

    [Header("TV Audio Source")]
    public AudioSource tvAudioSource;

    [Header("TV Audio Clips")]
    public AudioClip normalReport;
    public AudioClip secondReport;
    public AudioClip behindYou;
    public AudioClip answerThePhone;
    public AudioClip glitchyReport;
    public AudioClip questioning;
    public AudioClip allDead;

    [Header("Loop 4 Red Room Phone Repeats")]
    public AudioClip whyNoAnswer1;
    public AudioClip whyNoAnswer2;
    public AudioClip whyNoAnswer3;
    public float timeBetweenRandomPhoneClips = 0.25f;

    [Header("Loop 3 TV Interruption")]
    public float lookBehindInterruptDelay = 6f;

    [Header("Loop 3 Forced Look Behind")]
    public bool forceLookBehind = true;
    public Transform monsterLookTarget;
    public Behaviour playerMovementScript;
    public CameraController cameraController;

    [Header("Loop 6 Looked At Monster Trigger")]
    public Camera playerCamera;
    public GameObject lookedAtMonster;
    public string lookedAtMonsterName = "LookedAtMonster";
    public float lookRayDistance = 100f;
    public GameObject monsterChase;
    public Animator monsterChaseAnimator;
    public string monsterChaseAnimationTrigger = "StartChase";

    [Header("Objects")]
    public GameObject baseScene;
    public GameObject drugs;
    public GameObject missingPeoplePhotos;
    public GameObject hallwayFigure;
    public GameObject loop3Monster;
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
    private bool loop6DoorChecked = false;
    private bool loop6ChaseAnimationStarted = false;
    private bool endingStarted = false;

    private bool phoneIsRinging = false;
    private bool noAnswerPlayedThisLoop = false;

    private Coroutine backLightFlickerCoroutine;
    private Coroutine loop3BroadcastCoroutine;
    private Coroutine loop4RandomPhoneCoroutine;
    private Coroutine phoneVoiceCoroutine;
    private AudioClip originalPhoneRingingClip;

    private void Start()
    {
        if (phoneRinging != null)
            originalPhoneRingingClip = phoneRinging.clip;

        ApplyLoop();
    }

    private void Update()
    {
        CheckLoop6LookedAtMonsterRaycast();
    }

    public void NextLoop()
    {
        loopCount++;

        chaseStarted = false;
        playerReachedFigure = false;
        waitingForLookBehind = false;
        loop6DoorChecked = false;
        loop6ChaseAnimationStarted = false;
        noAnswerPlayedThisLoop = false;

        StopLoop4RandomPhoneClips();
        StopPhoneVoiceCoroutine();

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
        SetActive(baseScene, true);

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

        PlayPhoneRinging();
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

        ShowTVNews();
        PlayTVClip(answerThePhone, false);

        SetActive(furnitureBlockade, true);

        LockDoor();

        PlayPhoneRinging();
    }

    private void Loop5()
    {
        SetAllLights(false);

        ShowTVNews();
        PlayTVClip(glitchyReport, true);

        PlayPlayerLoopClip(addedLoop, true);
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
        SetAllLights(true);

        ShowTVOff();
        StopTVAudio();

        SetActive(baseScene, false);
        SetActive(finalEmptyHallway, true);

        PlayPlayerLoopClip(allDead, false);
    }

    public void AnswerPhone()
    {
        PlayPlayerPhoneClip(pickUpPhone);

        if (!phoneIsRinging)
        {
            TryPlayNoAnswer();
            return;
        }

        StopPhoneRinging();

        if (phoneVoiceSource != null)
            phoneVoiceSource.Stop();

        if (loopCount == 2)
        {
            UnlockDoor();
        }

        if (loopCount == 4)
        {
            SetActive(lightsOn, false);
            SetActive(lightsOff, false);
            SetActive(redLights, true);
            SetActive(backLightOff, false);

            ShowTVNews();
            StopTVAudio();

            PlayPhoneClipWithoutHangUp(questioning, StartLoop4RandomPhoneClipsAfterQuestioning);

            PlayPlayerLoopClip(beginningLoop, true);

            SetActive(bloodWalls, true);
            UnlockDoor();
        }
    }

    private void TryPlayNoAnswer()
    {
        if (noAnswerPlayedThisLoop)
            return;

        noAnswerPlayedThisLoop = true;
        PlayPhoneClipThenHangUp(noAnswer);
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

        SetActive(hallwayFigure, false);

        Debug.Log("Loop 3: Player got too close to the figure. Lights out, figure hidden.");
    }

    public void PlayerReachedDoorLoop3()
    {
        if (loopCount != 3 || !playerReachedFigure)
            return;

        SetAllLights(true);
        SetActive(loop3Monster, true);

        ShowTVNews();
        LockDoor();

        waitingForLookBehind = true;

        StartLoop3BroadcastInterruptions();

        if (forceLookBehind && loop3ForcedLook != null)
            loop3ForcedLook.StartForcedLook();

        Debug.Log("Loop 3: Lights back on. Broadcast now gets interrupted by look behind you.");
    }

    public void LookedBehind()
    {
        if (loopCount == 3)
        {
            if (!waitingForLookBehind)
                return;

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
    }

    public void ReachedDoorInLoop6()
    {
        if (loopCount != 6)
            return;

        StopTVAudio();
        LockDoor();

        loop6DoorChecked = true;
        SetActive(monsterChase, true);

        Debug.Log("Loop 6: Player reached locked door. monsterChase enabled. Waiting for player to look at LookedAtMonster.");
    }

    private void CheckLoop6LookedAtMonsterRaycast()
    {
        if (loopCount != 6)
            return;

        if (!loop6DoorChecked || chaseStarted || loop6ChaseAnimationStarted)
            return;

        Camera cam = playerCamera != null ? playerCamera : Camera.main;

        if (cam == null)
            return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookRayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            bool hitAssignedObject = lookedAtMonster != null && hitObject == lookedAtMonster;
            bool hitNamedObject = !string.IsNullOrEmpty(lookedAtMonsterName) && hitObject.name == lookedAtMonsterName;

            if (hitAssignedObject || hitNamedObject)
            {
                StartLoop6ChaseFromLook();
            }
        }
    }

    private void StartLoop6ChaseFromLook()
    {
        loop6ChaseAnimationStarted = true;

        SetActive(monsterChase, true);

        if (monsterChaseAnimator == null && monsterChase != null)
            monsterChaseAnimator = monsterChase.GetComponent<Animator>();

        if (monsterChaseAnimator != null && !string.IsNullOrEmpty(monsterChaseAnimationTrigger))
            monsterChaseAnimator.SetTrigger(monsterChaseAnimationTrigger);

        StartChase();

        Debug.Log("Loop 6: LookedAtMonster raycast hit. Chase animation triggered.");
    }

    private void StartChase()
    {
        chaseStarted = true;

        SetActive(monster, true);
        SetActive(monsterChase, true);

        if (chaseMusic != null)
            chaseMusic.Play();

        PlayPlayerLoopClip(finalLoop, true);

        UnlockDoor();

        Debug.Log("Loop 6: Chase started. Door unlocked.");
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

    private void StartLoop4RandomPhoneClipsAfterQuestioning()
    {
        StopLoop4RandomPhoneClips();
        loop4RandomPhoneCoroutine = StartCoroutine(Loop4RandomPhoneClips());
    }

    private void StopLoop4RandomPhoneClips()
    {
        if (loop4RandomPhoneCoroutine != null)
        {
            StopCoroutine(loop4RandomPhoneCoroutine);
            loop4RandomPhoneCoroutine = null;
        }
    }

    private IEnumerator Loop4RandomPhoneClips()
    {
        AudioClip lastClip = null;

        while (loopCount == 4)
        {
            AudioClip randomClip = GetRandomLoop4PhoneClip(lastClip);

            if (randomClip == null)
                yield break;

            lastClip = randomClip;

            yield return StartCoroutine(PlayPhoneClipWithoutHangUpRoutine(randomClip));

            yield return new WaitForSeconds(timeBetweenRandomPhoneClips);
        }

        loop4RandomPhoneCoroutine = null;
    }

    private AudioClip GetRandomLoop4PhoneClip(AudioClip lastClip)
    {
        AudioClip[] clips = new AudioClip[] { whyNoAnswer1, whyNoAnswer2, whyNoAnswer3 };
        int validCount = 0;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null)
                validCount++;
        }

        if (validCount == 0)
            return null;

        if (validCount == 1)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                    return clips[i];
            }
        }

        AudioClip chosenClip = null;
        int safety = 0;

        while (chosenClip == null && safety < 20)
        {
            AudioClip candidate = clips[Random.Range(0, clips.Length)];

            if (candidate != null && candidate != lastClip)
                chosenClip = candidate;

            safety++;
        }

        return chosenClip;
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
        SetActive(furnitureBlockade, false);
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

    private void PlayPhoneRinging()
    {
        if (phoneRinging == null)
            return;

        if (originalPhoneRingingClip != null)
            phoneRinging.clip = originalPhoneRingingClip;

        phoneRinging.loop = true;
        phoneRinging.time = 0f;
        phoneRinging.Play();

        phoneIsRinging = true;
    }

    private void StopPhoneRinging()
    {
        if (phoneRinging != null)
            phoneRinging.Stop();

        phoneIsRinging = false;
    }

    private void PlayPlayerPhoneClip(AudioClip clip)
    {
        if (playerPhoneAudioSource == null || clip == null)
            return;

        playerPhoneAudioSource.Stop();
        playerPhoneAudioSource.clip = clip;
        playerPhoneAudioSource.loop = false;
        playerPhoneAudioSource.time = 0f;
        playerPhoneAudioSource.Play();
    }

    private void PlayPhoneClipWithoutHangUp(AudioClip clip, System.Action onComplete = null)
    {
        StopPhoneVoiceCoroutine();
        phoneVoiceCoroutine = StartCoroutine(PlayPhoneClipWithoutHangUpRoutine(clip, onComplete));
    }

    private IEnumerator PlayPhoneClipWithoutHangUpRoutine(AudioClip clip, System.Action onComplete = null)
    {
        AudioSource source = phoneVoiceSource != null ? phoneVoiceSource : phoneRinging;

        if (source == null)
            yield break;

        if (clip != null)
        {
            source.Stop();
            source.clip = clip;
            source.loop = false;
            source.time = 0f;
            source.Play();

            yield return new WaitForSeconds(clip.length);
        }

        onComplete?.Invoke();

        if (phoneVoiceCoroutine != null)
            phoneVoiceCoroutine = null;
    }

    private void PlayPhoneClipThenHangUp(AudioClip clip, System.Action onComplete = null)
    {
        StopPhoneVoiceCoroutine();
        phoneVoiceCoroutine = StartCoroutine(PlayPhoneClipThenHangUpRoutine(clip, onComplete));
    }

    private IEnumerator PlayPhoneClipThenHangUpRoutine(AudioClip clip, System.Action onComplete = null)
    {
        AudioSource source = phoneVoiceSource != null ? phoneVoiceSource : phoneRinging;

        if (source == null)
            yield break;

        if (clip != null)
        {
            source.Stop();
            source.clip = clip;
            source.loop = false;
            source.time = 0f;
            source.Play();

            yield return new WaitForSeconds(clip.length);
        }

        if (hangUpPhone != null)
        {
            source.Stop();
            source.clip = hangUpPhone;
            source.loop = false;
            source.time = 0f;
            source.Play();

            yield return new WaitForSeconds(hangUpPhone.length);
        }

        onComplete?.Invoke();

        if (phoneVoiceCoroutine != null)
            phoneVoiceCoroutine = null;
    }

    private void StopPhoneVoiceCoroutine()
    {
        if (phoneVoiceCoroutine != null)
        {
            StopCoroutine(phoneVoiceCoroutine);
            phoneVoiceCoroutine = null;
        }
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

    private void PlayPlayerLoopClip(AudioClip clip, bool loop)
    {
        if (playerLoopAudioSource == null || clip == null)
            return;

        if (playerLoopAudioSource.clip == clip && playerLoopAudioSource.isPlaying)
            return;

        playerLoopAudioSource.Stop();
        playerLoopAudioSource.clip = clip;
        playerLoopAudioSource.loop = loop;
        playerLoopAudioSource.time = 0f;
        playerLoopAudioSource.Play();
    }

    private void StopPlayerLoopAudio()
    {
        if (playerLoopAudioSource != null)
            playerLoopAudioSource.Stop();
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
        StopLoop4RandomPhoneClips();
        StopPhoneVoiceCoroutine();

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        SetActive(baseScene, true);
        SetActive(drugs, false);
        SetActive(missingPeoplePhotos, false);
        SetActive(hallwayFigure, false);
        SetActive(monster, false);
        SetActive(loop3Monster, false);
        SetActive(monsterChase, false);
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
        StopLoop4RandomPhoneClips();
        StopPhoneVoiceCoroutine();

        StopAudio(rainSfx);
        StopPhoneRinging();
        StopAudio(phoneVoiceSource);
        StopAudio(cryingSfx);
        StopAudio(chaseMusic);
        StopTVAudio();

        if (loopCount < 4 || loopCount > 7)
            StopPlayerLoopAudio();
    }

    private void StopAudio(AudioSource audio)
    {
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }

    private void Ending()
    {
        if (endingStarted)
            return;

        endingStarted = true;

        if (endingLoop != null && playerLoopAudioSource != null)
        {
            StartCoroutine(PlayEndingLoopThenLoad());
        }
        else
        {
            LoadEndingScene();
        }
    }

    private IEnumerator PlayEndingLoopThenLoad()
    {
        PlayPlayerLoopClip(endingLoop, false);

        if (endingLoop != null)
            yield return new WaitForSeconds(endingLoop.length);

        LoadEndingScene();
    }

    private void LoadEndingScene()
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
