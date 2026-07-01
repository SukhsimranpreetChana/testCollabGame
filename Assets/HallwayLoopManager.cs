using UnityEngine;
using UnityEngine.SceneManagement;

public class HallwayLoopManager : MonoBehaviour
{
    [Header("Loop")]
    public int loopCount = 1;

    [Header("Door")]
    public AutoDoor exitDoor;

    [Header("Audio")]
    public AudioSource rainSfx;
    public AudioSource phoneRinging;
    public AudioSource cryingSfx;
    public AudioSource newsReportSfx;
    public AudioSource pickUpPhoneSfx;
    public AudioSource lookBehindYouSfx;
    public AudioSource whyDidntYouAnswerSfx;
    public AudioSource chaseMusic;

    [Header("Objects")]
    public GameObject drugs;
    public GameObject missingPeoplePhotos;
    public GameObject hallwayFigure;
    public GameObject monster;
    public GameObject furnitureBlockade;
    public GameObject dirtyWalls;
    public GameObject bloodWalls;
    public GameObject normalFurniture;
    public GameObject finalEmptyHallway;

    [Header("TV")]
    public GameObject tvStatic;
    public GameObject tvNews;
    public GameObject tvOff;

    [Header("Lights")]
    public GameObject lightsOn;
    public GameObject lightsOff;
    public GameObject redLights;
    public GameObject flickeringBackLight;

    [Header("Ending")]
    public string endingSceneName;

    private bool phoneAnswered = false;
    private bool lookedBehind = false;
    private bool chaseStarted = false;

    private void Start()
    {
        ApplyLoop();
    }

    public void NextLoop()
    {
        loopCount++;
        phoneAnswered = false;
        lookedBehind = false;
        chaseStarted = false;

        Debug.Log("Loop #" + loopCount);

        ApplyLoop();
    }

    private void ApplyLoop()
    {
        StopAllLoopAudio();
        ResetEverything();

        if (exitDoor != null)
            exitDoor.locked = false;

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
        SetActive(lightsOn, true);
        SetActive(tvStatic, true);

        if (rainSfx != null)
            rainSfx.Play();
    }

    private void Loop2()
    {
        SetActive(lightsOn, true);
        SetActive(drugs, true);
        SetActive(missingPeoplePhotos, true);
        SetActive(tvNews, true);
        SetActive(flickeringBackLight, true);

        if (exitDoor != null)
            exitDoor.locked = true;

        if (phoneRinging != null)
            phoneRinging.Play();

        if (newsReportSfx != null)
            newsReportSfx.Play();

        InvokeRepeating(nameof(PlayPickUpPhone), 4f, 4f);
    }

    private void Loop3()
    {
        SetActive(lightsOn, true);
        SetActive(hallwayFigure, true);
        SetActive(dirtyWalls, true);
        SetActive(tvNews, true);

        if (exitDoor != null)
            exitDoor.locked = true;

        if (cryingSfx != null)
            cryingSfx.Play();

        if (newsReportSfx != null)
            newsReportSfx.Play();

        InvokeRepeating(nameof(PlayLookBehindYou), 4f, 4f);
    }

    private void Loop4()
    {
        SetActive(lightsOff, true);
        SetActive(tvOff, true);
        SetActive(furnitureBlockade, true);

        if (exitDoor != null)
            exitDoor.locked = true;

        if (phoneRinging != null)
            phoneRinging.Play();
    }

    private void Loop5()
    {
        SetActive(lightsOff, true);
        SetActive(tvNews, true);

        if (whyDidntYouAnswerSfx != null)
            whyDidntYouAnswerSfx.Play();
    }

    private void Loop6()
    {
        SetActive(lightsOn, true);
        SetActive(tvStatic, true);

        if (exitDoor != null)
            exitDoor.locked = true;
    }

    private void Loop7()
    {
        SetActive(finalEmptyHallway, true);
        SetActive(tvOff, true);
        SetActive(lightsOff, true);
    }

    private void Ending()
    {
        if (!string.IsNullOrEmpty(endingSceneName))
            SceneManager.LoadScene(endingSceneName);
    }

    public void AnswerPhone()
    {
        phoneAnswered = true;

        if (phoneRinging != null)
            phoneRinging.Stop();

        CancelInvoke(nameof(PlayPickUpPhone));

        if (loopCount == 2)
        {
            if (exitDoor != null)
                exitDoor.locked = false;
        }

        if (loopCount == 4)
        {
            SetActive(lightsOff, false);
            SetActive(lightsOn, true);
            SetActive(bloodWalls, true);

            if (exitDoor != null)
                exitDoor.locked = false;
        }
    }

    public void LookedBehind()
    {
        lookedBehind = true;

        if (loopCount == 3)
        {
            CancelInvoke(nameof(PlayLookBehindYou));

            if (exitDoor != null)
                exitDoor.locked = false;
        }

        if (loopCount == 6 && !chaseStarted)
        {
            StartChase();
        }
    }

    public void ReachedDoorInLoop6()
    {
        if (loopCount == 6 && exitDoor != null)
            exitDoor.locked = true;
    }

    private void StartChase()
    {
        chaseStarted = true;

        SetActive(monster, true);

        if (chaseMusic != null)
            chaseMusic.Play();

        if (exitDoor != null)
            exitDoor.locked = false;
    }

    private void PlayPickUpPhone()
    {
        if (pickUpPhoneSfx != null)
            pickUpPhoneSfx.Play();
    }

    private void PlayLookBehindYou()
    {
        if (lookBehindYouSfx != null)
            lookBehindYouSfx.Play();
    }

    private void ResetEverything()
    {
        CancelInvoke();

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
        SetActive(flickeringBackLight, false);
    }

    private void StopAllLoopAudio()
    {
        StopAudio(rainSfx);
        StopAudio(phoneRinging);
        StopAudio(cryingSfx);
        StopAudio(newsReportSfx);
        StopAudio(pickUpPhoneSfx);
        StopAudio(lookBehindYouSfx);
        StopAudio(whyDidntYouAnswerSfx);
        StopAudio(chaseMusic);
    }

    private void StopAudio(AudioSource audio)
    {
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }
}