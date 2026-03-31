using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// N. Bababooey sits on Cam 1 and idles. Before moving, he plays a warning sound.
// Once he leaves the camera the player has a very short window to close the left door.
// Halfway through that window his sprite appears at the door as a visual cue.
//
// Unity Setup:
//   - Add NBababooeyManager to a new always-active GameObject
//   - Add a NBababooeyFeedOverlay Image inside CameraMonitor (above FeedImage)
//   - Add a NBababooeyDoorVisual Image in the left view Canvas group
//   - Add a NBababooeyJumpscare Image in Canvas (last in hierarchy)
//   - Assign all references in Inspector

public class NBababooeyManager : MonoBehaviour
{
    [Header("References")]
    public ToggleDoor toggleDoor;
    public CameraFeedController cameraFeedController;
    public CameraMonitorController monitorController;

    [Header("Camera")]
    // Which camera index Bababooey sits on (0-based). Default: 1.
    public int camIndex = 1;
    // Shown on the camera feed while Bababooey is sitting there.
    public Image cameraOverlayImage;

    [Header("Door Visual")]
    // Shown at the left door halfway through the kill timer.
    public Image doorEntranceImage;

    [Header("Jumpscare")]
    public Image jumpscareImage;
    public AudioClip jumpscareClip;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip warningClip; // plays just before he moves

    [Header("Spawn Timing")]
    public float minIdleTime = 15f;
    public float maxIdleTime = 35f;

    [Header("Warning")]
    // How long after the warning sound before he actually leaves the camera.
    public float warningDuration = 1.5f;

    [Header("Kill Timer")]
    // Total seconds the player has to close the door after he leaves the cam.
    public float killTimer = 3f;

    private bool onCamera = false;

    private void Start()
    {
        HideAll();
        StartCoroutine(BababooeyLoop());
    }

    private void Update()
    {
        if (cameraOverlayImage == null) return;

        bool show = onCamera
            && monitorController != null && monitorController.IsMonitorOpen()
            && cameraFeedController != null
            && cameraFeedController.GetCurrentCameraIndex() == camIndex;

        cameraOverlayImage.enabled = show;
    }

    private IEnumerator BababooeyLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
            yield return StartCoroutine(BababooeyAttack());
        }
    }

    private IEnumerator BababooeyAttack()
    {
        // He's now sitting on the camera
        onCamera = true;

        // Play warning sound, then wait before he departs
        if (audioSource != null && warningClip != null)
            audioSource.PlayOneShot(warningClip);

        yield return new WaitForSeconds(warningDuration);

        // He leaves the camera
        onCamera = false;
        if (cameraOverlayImage != null)
            cameraOverlayImage.enabled = false;

        // Short kill timer — door must be closed in time
        float elapsed = 0f;
        bool doorShown = false;
        bool survived = false;

        while (elapsed < killTimer)
        {
            elapsed += Time.deltaTime;

            // Sprite appears at the door halfway through the timer
            if (!doorShown && elapsed >= killTimer * 0.5f)
            {
                doorShown = true;
                if (doorEntranceImage != null)
                    doorEntranceImage.enabled = true;
            }

            if (toggleDoor != null && toggleDoor.IsDoorClosed())
            {
                survived = true;
                break;
            }

            yield return null;
        }

        if (doorEntranceImage != null)
            doorEntranceImage.enabled = false;

        if (!survived)
            yield return StartCoroutine(DeathRoutine());
    }

    private void HideAll()
    {
        if (cameraOverlayImage != null)  cameraOverlayImage.enabled = false;
        if (doorEntranceImage != null)   doorEntranceImage.enabled  = false;
        if (jumpscareImage != null)      jumpscareImage.enabled      = false;
    }

    private IEnumerator DeathRoutine()
    {
        if (jumpscareImage != null)
            jumpscareImage.enabled = true;

        if (audioSource != null && jumpscareClip != null)
            audioSource.PlayOneShot(jumpscareClip);

        float wait = jumpscareClip != null ? jumpscareClip.length : 2f;
        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene(2);
    }
}
