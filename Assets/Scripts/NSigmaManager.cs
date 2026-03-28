using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// N. Sigma slowly approaches either the front gate or the left door.
//
// Front path (Gate):
//   N. Sigma appears in cameras, then reaches the front window.
//   Player must HOLD the gate down for gateHoldRequired seconds
//   within the attackWindow to repel it.
//
// Left path (Door):
//   N. Sigma appears in cameras, then reaches the left door.
//   Player must close the door before the attackWindow expires.
//
// Camera config: assign frontPathCamIndices and leftPathCamIndices to
// match your cam button indices (0-based). Default: front uses cams 0,1
// and left uses cams 2,3.
//
// Unity Setup:
//   - Add NSigmaManager script to a new NSigmaManager object (always active)
//   - Add NSigmaFrontVisual and NSigmaLeftVisual UI Images inside the Canvas
//     (alongside SamVisual), each with a CanvasGroup component
//   - Assign those CanvasGroups to OfficeTurner's nSigmaFrontCanvasGroup / nSigmaLeftCanvasGroup
//   - Add a NSigmaFeedOverlay Image inside CameraMonitor (above FeedImage in hierarchy)
//   - Add NSigmaJumpscare Image inside Canvas (must be last, like SamJumpscare)
//   - Assign all references in Inspector

public class NSigmaManager : MonoBehaviour
{
    public enum Approach { Front, Left }

    [Header("References")]
    public DragGate dragGate;
    public ToggleDoor toggleDoor;
    public CameraFeedController cameraFeedController;
    public CameraMonitorController monitorController;

    [Header("Camera Feed Overlay")]
    // Image shown on the camera monitor when player views N. Sigma's current cam.
    // Place this inside CameraMonitor in the Canvas, on top of FeedImage.
    public Image cameraOverlayImage;

    [Header("Entrance Visuals")]
    // Shown at front window when N. Sigma reaches the front entrance (stage 2, front path).
    // Needs a CanvasGroup — assign to OfficeTurner.nSigmaFrontCanvasGroup.
    public Image frontEntranceImage;
    // Shown at left door when N. Sigma reaches the left entrance (stage 2, left path).
    // Needs a CanvasGroup — assign to OfficeTurner.nSigmaLeftCanvasGroup.
    public Image leftEntranceImage;

    [Header("Jumpscare")]
    public Image jumpscareImage;
    public AudioClip jumpscareClip;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip approachClip; // plays when N. Sigma moves to the next stage

    [Header("Spawn Timing")]
    public float minSpawnDelay = 20f;
    public float maxSpawnDelay = 40f;

    [Header("Stage Durations")]
    // How long N. Sigma lingers at each camera stage before moving closer.
    // Two entries: [0] = first cam, [1] = second cam (hallway).
    public float[] stageDurations = { 10f, 8f };

    [Header("Attack Window")]
    public float attackWindow = 6f;       // seconds N. Sigma waits at the entrance
    public float gateHoldRequired = 3f;   // seconds of gate-held-down needed to repel (front path)

    [Header("Camera Path Config (0-based)")]
    public int[] frontPathCamIndices = { 0, 1 };
    public int[] leftPathCamIndices  = { 2, 3 };

    private bool nSigmaActive = false;
    private int currentStage = 0;  // 0,1 = camera stages; 2 = at entrance
    private Approach currentApproach;

    private void Start()
    {
        HideAll();
        StartCoroutine(NSigmaLoop());
    }

    private void Update()
    {
        // Show camera overlay only while N. Sigma is in a camera stage
        // and the player is viewing the correct camera with monitor open.
        if (cameraOverlayImage == null)
            return;

        bool shouldShow = nSigmaActive
            && currentStage < 2
            && monitorController != null && monitorController.IsMonitorOpen()
            && cameraFeedController != null
            && cameraFeedController.GetCurrentCameraIndex() == GetCurrentCameraIndex();

        cameraOverlayImage.enabled = shouldShow;
    }

    private void HideAll()
    {
        if (cameraOverlayImage != null)  cameraOverlayImage.enabled  = false;
        if (frontEntranceImage != null)  frontEntranceImage.enabled  = false;
        if (leftEntranceImage != null)   leftEntranceImage.enabled   = false;
        if (jumpscareImage != null)      jumpscareImage.enabled       = false;
    }

    private IEnumerator NSigmaLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            if (!nSigmaActive)
                yield return StartCoroutine(NSigmaAttack());
        }
    }

    private IEnumerator NSigmaAttack()
    {
        nSigmaActive = true;
        currentApproach = Random.value < 0.5f ? Approach.Front : Approach.Left;

        // Progress through camera stages
        for (int stage = 0; stage < stageDurations.Length; stage++)
        {
            currentStage = stage;
            yield return new WaitForSeconds(stageDurations[stage]);

            if (audioSource != null && approachClip != null)
                audioSource.PlayOneShot(approachClip);
        }

        // Reached the entrance
        currentStage = 2;
        SetEntranceVisual(true);

        bool blocked;

        if (currentApproach == Approach.Left)
        {
            // Left door: player just needs to close the door before timer runs out
            float timer = attackWindow;
            while (timer > 0f && !toggleDoor.IsDoorClosed())
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            blocked = toggleDoor.IsDoorClosed();
        }
        else
        {
            // Front gate: player must hold the gate down for gateHoldRequired seconds
            float heldTime = 0f;
            float timer = attackWindow;
            while (timer > 0f && heldTime < gateHoldRequired)
            {
                if (dragGate.IsGateDown())
                    heldTime += Time.deltaTime;
                timer -= Time.deltaTime;
                yield return null;
            }
            blocked = heldTime >= gateHoldRequired;
        }

        SetEntranceVisual(false);
        nSigmaActive = false;

        if (!blocked)
            yield return StartCoroutine(DeathRoutine());
    }

    private void SetEntranceVisual(bool show)
    {
        if (currentApproach == Approach.Front && frontEntranceImage != null)
            frontEntranceImage.enabled = show;
        if (currentApproach == Approach.Left && leftEntranceImage != null)
            leftEntranceImage.enabled = show;
    }

    private int GetCurrentCameraIndex()
    {
        int[] path = currentApproach == Approach.Front ? frontPathCamIndices : leftPathCamIndices;
        if (currentStage >= 0 && currentStage < path.Length)
            return path[currentStage];
        return -1;
    }

    private IEnumerator DeathRoutine()
    {
        if (jumpscareImage != null)
            jumpscareImage.enabled = true;

        if (audioSource != null && jumpscareClip != null)
            audioSource.PlayOneShot(jumpscareClip);

        float waitTime = jumpscareClip != null ? jumpscareClip.length : 2f;
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(2);
    }
}
