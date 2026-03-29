using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SixSevenManager : MonoBehaviour
{
    [Header("References")]
    public CameraFeedController cameraFeedController;
    public DragGate gate;

    [Header("Gate Camera Sprites")]
    public int gateCamIndex = 0;
    public Sprite defaultGateSprite;
    public Sprite sixSprite;
    public Sprite sevenSprite;

    [Header("Jumpscares")]
    public Image sixJumpscareImage;
    public Image sevenJumpscareImage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioCue;
    public AudioClip bonkClip;
    public AudioClip sixLeaveClip;
    public AudioClip deathClip;

    [Header("Settings")]
    public float minIdleTime = 15f;
    public float maxIdleTime = 35f;
    public float encounterWindow = 7f;

    private enum State { Idle, Active, Dead }
    private State state = State.Idle;
    private float timer = 0f;
    private bool isSix = false;
    private bool wasDownLastFrame = false;

    void Start()
    {
        if (sixJumpscareImage != null) sixJumpscareImage.enabled = false;
        if (sevenJumpscareImage != null) sevenJumpscareImage.enabled = false;
        StartIdleTimer();
    }

    void Update()
    {
        if (state == State.Dead) return;

        bool gateDown = gate != null && gate.IsGateDown();
        bool gateJustPulledDown = gateDown && !wasDownLastFrame;

        if (state == State.Idle)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                TriggerEncounter();
        }
        else if (state == State.Active)
        {
            timer -= Time.deltaTime;

            if (isSix)
            {
                if (gateJustPulledDown)
                    StartCoroutine(DeathRoutine());
                else if (timer <= 0f)
                    ResolveEncounter();
            }
            else
            {
                if (gateJustPulledDown)
                    ResolveEncounter();
                else if (timer <= 0f)
                    StartCoroutine(DeathRoutine());
            }
        }

        wasDownLastFrame = gateDown;
    }

    void TriggerEncounter()
    {
        isSix = Random.value < 0.5f;

        cameraFeedController.cameraFeeds[gateCamIndex].feedSprite = isSix ? sixSprite : sevenSprite;
        if (cameraFeedController.GetCurrentCameraIndex() == gateCamIndex)
            cameraFeedController.ShowCamera(gateCamIndex);

        if (audioSource != null && audioCue != null)
            audioSource.PlayOneShot(audioCue);

        if (!isSix && gate.IsGateDown())
        {
            ResolveEncounter();
            return;
        }

        timer = encounterWindow;
        state = State.Active;
    }

    void ResolveEncounter()
    {
        cameraFeedController.cameraFeeds[gateCamIndex].feedSprite = defaultGateSprite;
        if (cameraFeedController.GetCurrentCameraIndex() == gateCamIndex)
            cameraFeedController.ShowCamera(gateCamIndex);

        AudioClip leaveClip = isSix ? sixLeaveClip : bonkClip;
        if (audioSource != null && leaveClip != null)
            audioSource.PlayOneShot(leaveClip);

        StartIdleTimer();
    }

    void StartIdleTimer()
    {
        state = State.Idle;
        timer = Random.Range(minIdleTime, maxIdleTime);
    }

    IEnumerator DeathRoutine()
    {
        state = State.Dead;

        cameraFeedController.cameraFeeds[gateCamIndex].feedSprite = defaultGateSprite;

        Image jumpscareImage = isSix ? sixJumpscareImage : sevenJumpscareImage;
        if (jumpscareImage != null) jumpscareImage.enabled = true;

        if (audioSource != null && deathClip != null)
            audioSource.PlayOneShot(deathClip);

        yield return new WaitForSeconds(deathClip != null ? deathClip.length : 1.5f);

        SceneManager.LoadScene(2);
    }
}