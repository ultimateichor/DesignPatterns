using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages Sam the Simon's colour sequence and the player's repeat attempt.
///
/// Lives on the SimonSaysPanel in FrontView.
/// Because OfficeTurner now uses CanvasGroups instead of SetActive,
/// this component's coroutines are never interrupted by view switching.
/// </summary>
public class SimonSaysController : MonoBehaviour
{
    [Header("Buttons — assign Red, Blue, Green, Yellow in order")]
    public SimonButton[] colorButtons;

    [Header("Sequence Playback Timing")]
    [Tooltip("How long each button stays lit during Sam's demo")]
    public float flashOnDuration   = 0.65f;
    [Tooltip("Gap (dark) between each flash during demo")]
    public float flashOffDuration  = 0.20f;

    [Header("Player Input")]
    [Tooltip("Seconds the player has to repeat the WHOLE sequence — resets on each correct press")]
    public float inputTimeout = 2.5f;

    [Header("UI")]
    [Tooltip("TextMeshPro that shows Sam's colour callouts and prompts")]
    public TextMeshProUGUI samSpeechText;

    // ── Runtime state ─────────────────────────────────────────────────────
    private readonly List<int> sequence = new List<int>();
    private int   playerInputIndex;
    private bool  waitingForInput;
    private float inputDeadline;

    private Action onSuccess;
    private Action onFail;

    // ── Unity ─────────────────────────────────────────────────────────────
    void Start()
    {
        SetSpeech("");
        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].buttonIndex = i;
            colorButtons[i].SetInteractable(false);
        }
    }

    void Update()
    {
        // Timeout: player didn't finish the sequence in time
        if (waitingForInput && Time.time >= inputDeadline)
        {
            waitingForInput = false;
            DisableAllButtons();
            StartCoroutine(FailRoutine());
        }
    }

    // ── Public API ────────────────────────────────────────────────────────

    /// <summary>
    /// Called by SamTheSimon to start a new round.
    /// </summary>
    public void StartSequence(int length, Action successCb, Action failCb)
    {
        onSuccess = successCb;
        onFail    = failCb;

        // Build a new random sequence
        sequence.Clear();
        for (int i = 0; i < length; i++)
            sequence.Add(UnityEngine.Random.Range(0, colorButtons.Length));

        StopAllCoroutines();
        StartCoroutine(PlaybackRoutine());
    }

    /// <summary>
    /// Hard-stop everything (e.g. game reset).
    /// </summary>
    public void ForceStop()
    {
        StopAllCoroutines();
        waitingForInput = false;
        DisableAllButtons();
        DimAll();
        SetSpeech("");
    }

    // ── Sequence Playback ─────────────────────────────────────────────────

    private IEnumerator PlaybackRoutine()
    {
        SetSpeech("SAM IS HERE...");
        yield return new WaitForSeconds(0.9f);

        // Show each colour, calling its name aloud so the player can listen
        // without looking at the panel
        for (int i = 0; i < sequence.Count; i++)
        {
            SimonButton btn = colorButtons[sequence[i]];

            // Sam "speaks" the colour name — player doesn't need to watch
            SetSpeech(btn.colorName.ToUpper() + "!");

            yield return StartCoroutine(btn.FlashRoutine(flashOnDuration, flashOffDuration));

            SetSpeech("");
        }

        // Cue the player
        SetSpeech("NOW REPEAT!");
        yield return new WaitForSeconds(0.35f);
        SetSpeech("GO!");

        // Open input
        playerInputIndex = 0;
        waitingForInput  = true;
        inputDeadline    = Time.time + inputTimeout;

        foreach (var b in colorButtons)
            b.SetInteractable(true, HandleInput);
    }

    // ── Player Input Handling ─────────────────────────────────────────────

    private void HandleInput(int buttonIndex)
    {
        if (!waitingForInput) return;

        if (buttonIndex == sequence[playerInputIndex])
        {
            // Correct press
            playerInputIndex++;

            // Refresh the clock so the player isn't punished for thinking
            // between correct presses — but it's still tight
            inputDeadline = Time.time + inputTimeout;

            if (playerInputIndex >= sequence.Count)
            {
                // Full sequence matched!
                waitingForInput = false;
                DisableAllButtons();
                StartCoroutine(SuccessRoutine());
            }
        }
        else
        {
            // Wrong button
            waitingForInput = false;
            DisableAllButtons();
            StartCoroutine(FailRoutine());
        }
    }

    // ── Outcome Routines ──────────────────────────────────────────────────

    private IEnumerator SuccessRoutine()
    {
        SetSpeech("CORRECT!");
        yield return new WaitForSeconds(0.8f);
        SetSpeech("");
        DimAll();
        onSuccess?.Invoke();
    }

    private IEnumerator FailRoutine()
    {
        SetSpeech("WRONG!");

        // Flash all buttons red-ish to signal death
        foreach (var b in colorButtons) b.SetLit(true);
        yield return new WaitForSeconds(0.55f);
        DimAll();

        onFail?.Invoke();
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private void DisableAllButtons()
    {
        foreach (var b in colorButtons) b.SetInteractable(false);
    }

    private void DimAll()
    {
        foreach (var b in colorButtons) b.SetLit(false);
    }

    private void SetSpeech(string msg)
    {
        if (samSpeechText != null)
            samSpeechText.text = msg;
    }
}
