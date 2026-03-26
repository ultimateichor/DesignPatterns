using UnityEngine;

/// <summary>
/// Sam the Simon — animatronic AI.
///
/// Sam randomly storms the front office, displays a Simon Says colour sequence
/// (while calling out each colour name so the player can look away),
/// then demands the player repeat it immediately.
/// Fail to repeat in time → death.
///
/// At max difficulty, Sam visits frequently and uses long sequences.
/// All timing values are exposed in the Inspector so they are easy to tweak.
/// </summary>
public class SamTheSimon : MonoBehaviour
{
    [Header("Sam's Visual — UI Image in the front office")]
    [Tooltip("The GameObject containing Sam's sprite. Will be shown/hidden on visits.")]
    public GameObject samSpriteObject;

    [Header("References")]
    public SimonSaysController simonSays;
    public OfficeTurner officeTurner;

    [Header("Difficulty (Max — applied from the start)")]
    [Tooltip("Minimum seconds between Sam visits")]
    public float minTimeBetweenVisits = 8f;
    [Tooltip("Maximum seconds between Sam visits")]
    public float maxTimeBetweenVisits = 20f;
    [Tooltip("Number of colours in each sequence")]
    public int sequenceLength = 5;

    // ── Runtime state ─────────────────────────────────────────────────────
    private bool  samPresent;
    private float nextVisitTime;

    // ── Unity ─────────────────────────────────────────────────────────────
    void Start()
    {
        HideSam();
        ScheduleNextVisit();
    }

    void Update()
    {
        // Don't do anything while the game is already over
        if (GameManager.Instance != null && GameManager.Instance.IsDead) return;

        if (!samPresent && Time.time >= nextVisitTime)
            Appear();
    }

    // ── Visit Logic ───────────────────────────────────────────────────────

    void Appear()
    {
        samPresent = true;

        // Force the player to face front and lock rotation so they can't flee
        officeTurner?.SetView(0);
        officeTurner?.LockView(true);

        ShowSam();

        // Kick off the Simon Says round
        simonSays.StartSequence(sequenceLength, OnPlayerWin, OnPlayerLose);
    }

    void OnPlayerWin()
    {
        samPresent = false;
        HideSam();
        officeTurner?.LockView(false);
        ScheduleNextVisit();
    }

    void OnPlayerLose()
    {
        samPresent = false;
        HideSam();
        officeTurner?.LockView(false);

        GameManager.Instance?.PlayerDie();
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    void ShowSam()
    {
        if (samSpriteObject != null)
            samSpriteObject.SetActive(true);
    }

    void HideSam()
    {
        if (samSpriteObject != null)
            samSpriteObject.SetActive(false);
    }

    void ScheduleNextVisit()
    {
        nextVisitTime = Time.time + Random.Range(minTimeBetweenVisits, maxTimeBetweenVisits);
    }
}
