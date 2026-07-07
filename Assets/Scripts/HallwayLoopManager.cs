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
    public AudioSource scaryMoanSfx;
    public AudioSource thuddingSfx;
    public AudioClip thuddingClip;
    public float thuddingDelayAfterMoan = -1f;

    [Header("Loop 6 Chase Effects")]
    public GameObject runText;
    public CameraShake cameraShake;

    [Tooltip("Delay after the Loop 6 chase trigger before red lights turn on.")]
    public float redLightsDelay = 3.5f;

    [Tooltip("Delay after the Loop 6 chase trigger before camera shake starts.")]
    public float cameraShakeDelay = 3.5f;

    [Tooltip("Chase music starts this many seconds before the red lights/camera shake moment.")]
    public float musicLeadTime = 1f;

    public float cameraShakeDuration = 1.5f;
    public float cameraShakeStrength = 0.1f;
    public float cameraShakeSpeed = 30f;

    [Header("Phone Audio Source")]
    public AudioSource phoneVoiceSource;

    [Header("Player Phone Audio")]
    public AudioSource playerPhoneAudioSource;
    public AudioClip pickUpPhone;
    public AudioClip noAnswer;
    public AudioClip hangUpPhone;
    public AudioClip phoneLineHangUpSfx;

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
    public FirstPersonController firstPersonController;
    public Behaviour[] extraMovementScriptsToDisable;
    public CharacterController playerCharacterController;
    public Rigidbody playerRigidbody;
    public Transform playerRoot;
    public CameraController cameraController;

    [Header("Loop 3 Movement Lock")]
    public bool autoFindPlayerReferences = true;
    public Transform headBobTransformToFreeze;

    [Header("Loop 3 Monster Animation")]
    public Animator loop3MonsterAnimator;
    public string loop3BackwardWalkStateName = "BackwardWalk";
    public float loop3MonsterDisappearDelay = 3f;

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
    public GameObject tvRoot;
    public GameObject tvStatic;
    public GameObject tvNews;
    public GameObject tvOff;

    [Header("Phone Objects")]
    public GameObject phoneRoot;
    public GameObject phoneRoot2;

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

    private bool loop3PlayerMovementLocked = false;
    private Vector3 loop3LockedPlayerPosition;
    private Quaternion loop3LockedPlayerRotation;
    private bool playerCharacterControllerWasEnabled = false;
    private bool playerRigidbodyWasKinematic = false;
    private bool playerMovementScriptWasEnabled = false;
    private bool firstPersonControllerWasEnabled = false;
    private bool[] extraMovementScriptsWereEnabled;
    private Vector3 lockedHeadBobLocalPosition;

    private Coroutine backLightFlickerCoroutine;
    private Coroutine loop3BroadcastCoroutine;
    private Coroutine loop4RandomPhoneCoroutine;
    private Coroutine phoneVoiceCoroutine;
    private Coroutine loop6CameraShakeCoroutine;
    private Coroutine loop6RedLightsCoroutine;
    private Coroutine loop6ChaseMusicCoroutine;
    private Coroutine loop6ThuddingCoroutine;
    private Coroutine loop2ReportCoroutine;
    private Coroutine loop3MonsterDisappearCoroutine;
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
        MaintainLoop3PlayerLock();
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
        StopLoop2ReportThenPhone();
        StopLoop3MonsterDisappear();
        StopLoop6CameraShake();
        StopLoop6RedLights();
        StopLoop6ChaseMusic();
        StopLoop6MoanThenThudding();

        UnlockPlayerMovement();

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
        SetActive(tvRoot, true);
        SetActive(phoneRoot, true);

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

        SetActive(drugs, true);
        SetActive(missingPeoplePhotos, true);

        LockDoor();

        // The phone does not ring until the player has heard
        // the full normal news report once.
        StartLoop2ReportThenPhone();
    }

    private void StartLoop2ReportThenPhone()
    {
        StopLoop2ReportThenPhone();
        loop2ReportCoroutine = StartCoroutine(Loop2ReportThenPhoneRoutine());
    }

    private void StopLoop2ReportThenPhone()
    {
        if (loop2ReportCoroutine != null)
        {
            StopCoroutine(loop2ReportCoroutine);
            loop2ReportCoroutine = null;
        }
    }

    private IEnumerator Loop2ReportThenPhoneRoutine()
    {
        if (tvAudioSource != null && normalReport != null)
        {
            PlayTVClip(normalReport, false);

            yield return new WaitWhile(() =>
                loopCount == 2 &&
                tvAudioSource != null &&
                tvAudioSource.isPlaying &&
                tvAudioSource.clip == normalReport
            );
        }

        if (loopCount == 2)
        {
            // Continue looping the report after the first full play.
            PlayTVClip(normalReport, true);

            // Now the phone can start ringing.
            PlayPhoneRinging();

            Debug.Log("Loop 2: Normal report heard once. Phone started ringing.");
        }

        loop2ReportCoroutine = null;
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

        StopTVAudio();
        StopPhoneRinging();
        StopAudio(phoneVoiceSource);

        SetActive(tvStatic, false);
        SetActive(tvNews, false);
        SetActive(tvOff, false);

        SetActive(tvRoot, false);
        SetActive(phoneRoot, false);
        SetActive(phoneRoot2, false);

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
            PlayPhoneClipWithoutHangUp(phoneLineHangUpSfx);
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

    private void CachePlayerReferences()
    {
        if (!autoFindPlayerReferences)
            return;

        GameObject playerObject = null;

        if (playerRoot != null)
            playerObject = playerRoot.gameObject;
        else
            playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        if (playerRoot == null)
            playerRoot = playerObject.transform;

        if (playerCharacterController == null)
            playerCharacterController = playerObject.GetComponent<CharacterController>();

        if (firstPersonController == null)
            firstPersonController = playerObject.GetComponent<FirstPersonController>();

        if (playerMovementScript == null && firstPersonController != null)
            playerMovementScript = firstPersonController;

        if (playerRigidbody == null)
            playerRigidbody = playerObject.GetComponent<Rigidbody>();

        if (cameraController == null)
            cameraController = playerObject.GetComponentInChildren<CameraController>();

        if (headBobTransformToFreeze == null && cameraController != null)
            headBobTransformToFreeze = cameraController.transform;
    }

    private void LockPlayerMovement()
    {
        if (loop3PlayerMovementLocked)
            return;

        CachePlayerReferences();

        loop3PlayerMovementLocked = true;

        if (playerRoot != null)
        {
            loop3LockedPlayerPosition = playerRoot.position;
            loop3LockedPlayerRotation = playerRoot.rotation;
        }

        if (headBobTransformToFreeze != null)
            lockedHeadBobLocalPosition = headBobTransformToFreeze.localPosition;

        if (playerMovementScript != null)
        {
            playerMovementScriptWasEnabled = playerMovementScript.enabled;
            playerMovementScript.enabled = false;
        }

        if (firstPersonController != null)
        {
            firstPersonControllerWasEnabled = firstPersonController.enabled;
            firstPersonController.enabled = false;
        }

        if (extraMovementScriptsToDisable != null)
        {
            extraMovementScriptsWereEnabled = new bool[extraMovementScriptsToDisable.Length];

            for (int i = 0; i < extraMovementScriptsToDisable.Length; i++)
            {
                if (extraMovementScriptsToDisable[i] != null)
                {
                    extraMovementScriptsWereEnabled[i] = extraMovementScriptsToDisable[i].enabled;
                    extraMovementScriptsToDisable[i].enabled = false;
                }
            }
        }

        if (playerCharacterController != null)
        {
            playerCharacterControllerWasEnabled = playerCharacterController.enabled;
            playerCharacterController.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbodyWasKinematic = playerRigidbody.isKinematic;

#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector3.zero;
#else
            playerRigidbody.velocity = Vector3.zero;
#endif

            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        Debug.Log("Loop 3: Player movement locked.");
    }

    private void MaintainLoop3PlayerLock()
    {
        if (!loop3PlayerMovementLocked)
            return;

        if (playerRoot != null)
        {
            playerRoot.position = loop3LockedPlayerPosition;
            playerRoot.rotation = loop3LockedPlayerRotation;
        }

        // This prevents headbob/sway from still showing while the player is locked.
        // Rotation is still controlled by ForcedLook, but local position is frozen.
        if (headBobTransformToFreeze != null)
            headBobTransformToFreeze.localPosition = lockedHeadBobLocalPosition;
    }

    private void UnlockPlayerMovement()
    {
        if (!loop3PlayerMovementLocked)
            return;

        loop3PlayerMovementLocked = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = playerRigidbodyWasKinematic;

#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector3.zero;
#else
            playerRigidbody.velocity = Vector3.zero;
#endif

            playerRigidbody.angularVelocity = Vector3.zero;
        }

        if (playerCharacterController != null)
            playerCharacterController.enabled = playerCharacterControllerWasEnabled;

        if (extraMovementScriptsToDisable != null && extraMovementScriptsWereEnabled != null)
        {
            int count = Mathf.Min(extraMovementScriptsToDisable.Length, extraMovementScriptsWereEnabled.Length);

            for (int i = 0; i < count; i++)
            {
                if (extraMovementScriptsToDisable[i] != null)
                    extraMovementScriptsToDisable[i].enabled = extraMovementScriptsWereEnabled[i];
            }
        }

        if (firstPersonController != null)
            firstPersonController.enabled = firstPersonControllerWasEnabled;

        if (playerMovementScript != null)
            playerMovementScript.enabled = playerMovementScriptWasEnabled;

        Debug.Log("Loop 3: Player movement unlocked.");
    }

    private void FinishLoop3MonsterForcedLook()
    {
        if (cameraController != null)
            cameraController.SetForceLooking(false);

        UnlockPlayerMovement();
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

        if (loop3MonsterAnimator == null && loop3Monster != null)
            loop3MonsterAnimator = loop3Monster.GetComponent<Animator>();

        if (loop3MonsterAnimator != null && !string.IsNullOrEmpty(loop3BackwardWalkStateName))
            loop3MonsterAnimator.Play(loop3BackwardWalkStateName, 0, 0f);

        StartLoop3MonsterDisappear();

        ShowTVNews();
        LockDoor();

        waitingForLookBehind = true;

        StartLoop3BroadcastInterruptions();

        LockPlayerMovement();

        if (forceLookBehind && loop3ForcedLook != null)
            loop3ForcedLook.StartForcedLook();

        Debug.Log("Loop 3: Lights back on. Player locked, monster animation started, and broadcast is interrupted by look behind you.");
    }

    private void StartLoop3MonsterDisappear()
    {
        StopLoop3MonsterDisappear();
        loop3MonsterDisappearCoroutine = StartCoroutine(Loop3MonsterDisappearRoutine());
    }

    private void StopLoop3MonsterDisappear()
    {
        if (loop3MonsterDisappearCoroutine != null)
        {
            StopCoroutine(loop3MonsterDisappearCoroutine);
            loop3MonsterDisappearCoroutine = null;
        }
    }

    private IEnumerator Loop3MonsterDisappearRoutine()
    {
        yield return new WaitForSeconds(loop3MonsterDisappearDelay);

        SetActive(loop3Monster, false);

        // Let the player move again after the monster's backward-walk moment ends.
        FinishLoop3MonsterForcedLook();

        loop3MonsterDisappearCoroutine = null;
    }

    public void LookedBehind()
    {
        if (loopCount == 3)
        {
            if (!waitingForLookBehind)
                return;

            // Movement is unlocked by Loop3MonsterDisappearRoutine after loop3MonsterDisappearDelay.
            // This prevents the player from moving during the forced-look / monster animation.
            if (!loop3PlayerMovementLocked && cameraController != null)
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

        if (monsterChaseAnimator != null &&
            !string.IsNullOrEmpty(monsterChaseAnimationTrigger))
        {
            monsterChaseAnimator.SetTrigger(monsterChaseAnimationTrigger);
        }

        // Show "RUN" text immediately.
        SetActive(runText, true);

        // Monster scary moan immediately, then thudding after the moan finishes.
        StartLoop6MoanThenThudding();

        // Delay the chase music so it plays 1 second before the red lights/camera shake moment.
        StartLoop6ChaseMusicDelay();

        // Delay the red lights.
        StartLoop6RedLightsDelay();

        // Delay camera shake.
        StartLoop6CameraShakeDelay();

        StartChase();

        Debug.Log("Loop 6: Chase animation started. Music, red lights, and camera shake are delayed.");
    }

    private void StartChase()
    {
        chaseStarted = true;

        SetActive(monster, true);
        SetActive(monsterChase, true);

        PlayPlayerLoopClip(finalLoop, true);

        UnlockDoor();

        Debug.Log("Loop 6: Chase started. Door unlocked.");
    }

    private void StartLoop6MoanThenThudding()
    {
        StopLoop6MoanThenThudding();
        loop6ThuddingCoroutine = StartCoroutine(Loop6MoanThenThuddingRoutine());
    }

    private void StopLoop6MoanThenThudding()
    {
        if (loop6ThuddingCoroutine != null)
        {
            StopCoroutine(loop6ThuddingCoroutine);
            loop6ThuddingCoroutine = null;
        }
    }

    private IEnumerator Loop6MoanThenThuddingRoutine()
    {
        float delayBeforeThud = 0f;

        if (scaryMoanSfx != null)
        {
            scaryMoanSfx.Stop();
            scaryMoanSfx.loop = false;
            scaryMoanSfx.time = 0f;
            scaryMoanSfx.Play();

            if (scaryMoanSfx.clip != null)
                delayBeforeThud = scaryMoanSfx.clip.length;
        }

        // If thuddingDelayAfterMoan is 0 or higher, use that custom delay.
        // If it is -1, wait for the full scaryMoanSfx clip length.
        if (thuddingDelayAfterMoan >= 0f)
            delayBeforeThud = thuddingDelayAfterMoan;

        if (delayBeforeThud > 0f)
            yield return new WaitForSeconds(delayBeforeThud);

        PlayThuddingSfx();

        loop6ThuddingCoroutine = null;
    }

    private void PlayThuddingSfx()
    {
        AudioSource source = thuddingSfx != null ? thuddingSfx : scaryMoanSfx;

        if (source == null)
        {
            Debug.LogWarning("Loop 6 thudding SFX could not play because no AudioSource is assigned.");
            return;
        }

        source.loop = false;
        source.time = 0f;

        if (thuddingClip != null)
        {
            source.PlayOneShot(thuddingClip);
            return;
        }

        if (source.clip != null)
        {
            source.Stop();
            source.Play();
            return;
        }

        Debug.LogWarning("Loop 6 thudding SFX could not play because there is no thuddingClip and the AudioSource has no clip.");
    }

    private void StartLoop6ChaseMusicDelay()
    {
        StopLoop6ChaseMusic();

        float musicDelay = Mathf.Max(0f, redLightsDelay - musicLeadTime);
        loop6ChaseMusicCoroutine = StartCoroutine(Loop6ChaseMusicDelayRoutine(musicDelay));
    }

    private void StopLoop6ChaseMusic()
    {
        if (loop6ChaseMusicCoroutine != null)
        {
            StopCoroutine(loop6ChaseMusicCoroutine);
            loop6ChaseMusicCoroutine = null;
        }
    }

    private IEnumerator Loop6ChaseMusicDelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (chaseMusic != null)
        {
            chaseMusic.Stop();
            chaseMusic.loop = true;
            chaseMusic.time = 0f;
            chaseMusic.Play();
        }

        loop6ChaseMusicCoroutine = null;
    }

    private void StartLoop6RedLightsDelay()
    {
        StopLoop6RedLights();
        loop6RedLightsCoroutine = StartCoroutine(Loop6RedLightsDelayRoutine());
    }

    private void StopLoop6RedLights()
    {
        if (loop6RedLightsCoroutine != null)
        {
            StopCoroutine(loop6RedLightsCoroutine);
            loop6RedLightsCoroutine = null;
        }
    }

    private IEnumerator Loop6RedLightsDelayRoutine()
    {
        yield return new WaitForSeconds(redLightsDelay);

        SetActive(lightsOn, false);
        SetActive(lightsOff, false);
        SetActive(redLights, true);
        SetActive(backLightOn, false);
        SetActive(backLightOff, false);

        loop6RedLightsCoroutine = null;
    }

    private void StartLoop6CameraShakeDelay()
    {
        StopLoop6CameraShake();
        loop6CameraShakeCoroutine = StartCoroutine(Loop6CameraShakeDelayRoutine());
    }

    private void StopLoop6CameraShake()
    {
        if (loop6CameraShakeCoroutine != null)
        {
            StopCoroutine(loop6CameraShakeCoroutine);
            loop6CameraShakeCoroutine = null;
        }
    }

    private IEnumerator Loop6CameraShakeDelayRoutine()
    {
        yield return new WaitForSeconds(cameraShakeDelay);

        if (cameraShake != null)
            cameraShake.StartShake(cameraShakeDuration, cameraShakeStrength, cameraShakeSpeed);

        loop6CameraShakeCoroutine = null;
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
        StopLoop2ReportThenPhone();
        StopLoop3MonsterDisappear();
        StopLoop6CameraShake();
        StopLoop6RedLights();
        StopLoop6ChaseMusic();

        UnlockPlayerMovement();

        if (cameraController != null)
            cameraController.SetForceLooking(false);

        SetActive(baseScene, true);
        SetActive(tvRoot, true);
        SetActive(phoneRoot, true);
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
        SetActive(runText, false);

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
        StopLoop2ReportThenPhone();
        StopLoop6CameraShake();
        StopLoop6RedLights();
        StopLoop6ChaseMusic();
        StopLoop6MoanThenThudding();

        StopAudio(rainSfx);
        StopPhoneRinging();
        StopAudio(phoneVoiceSource);
        StopAudio(cryingSfx);
        StopAudio(chaseMusic);
        StopAudio(scaryMoanSfx);
        StopAudio(thuddingSfx);
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
