using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonController : MonoBehaviour
{
    [Header("Button Images")]
    public Image redImage;
    public Image blueImage;
    public Image greenImage;
    public Image yellowImage;

    [Header("Normal Sprites")]
    public Sprite redNormal;
    public Sprite blueNormal;
    public Sprite greenNormal;
    public Sprite yellowNormal;

    [Header("Lit Sprites (can leave empty for now)")]
    public Sprite redLit;
    public Sprite blueLit;
    public Sprite greenLit;
    public Sprite yellowLit;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip redClip;
    public AudioClip blueClip;
    public AudioClip greenClip;
    public AudioClip yellowClip;

    [Header("Player Button Sounds")]
    public AudioClip redPlayerClip;
    public AudioClip bluePlayerClip;
    public AudioClip greenPlayerClip;
    public AudioClip yellowPlayerClip;

    [Header("Timing")]
    public float flashDuration = 0.6f;
    public float gapDuration = 0.15f;
    public float inputTimeout = 1.5f;

    private List<SimonButton.ButtonColor> pattern = new List<SimonButton.ButtonColor>();
    private int playerInputIndex = 0;
    private bool acceptingInput = false;
    private float inputTimer = 0f;

    public System.Action OnSuccess;
    public System.Action OnFailure;

    private void Update()
    {
        if (acceptingInput)
        {
            inputTimer -= Time.deltaTime;
            if (inputTimer <= 0f)
            {
                acceptingInput = false;
                OnFailure?.Invoke();
            }
        }
    }

    public void StartSequence(List<SimonButton.ButtonColor> newPattern)
    {
        pattern = new List<SimonButton.ButtonColor>(newPattern);
        playerInputIndex = 0;
        acceptingInput = false;
        StopAllCoroutines();
        StartCoroutine(PlayPattern());
    }

    private IEnumerator PlayPattern()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (SimonButton.ButtonColor color in pattern)
        {
            yield return StartCoroutine(FlashButton(color));
            yield return new WaitForSeconds(gapDuration);
        }

        acceptingInput = true;
        inputTimer = inputTimeout;
    }

    private IEnumerator FlashButton(SimonButton.ButtonColor color)
    {
        SetLit(color, true);
        PlayAudio(color);
        yield return new WaitForSeconds(flashDuration);
        SetLit(color, false);
    }

    private void SetLit(SimonButton.ButtonColor color, bool lit)
    {
        switch (color)
        {
            case SimonButton.ButtonColor.Red:
                if (redImage != null)
                    redImage.sprite = (lit && redLit != null) ? redLit : redNormal;
                break;
            case SimonButton.ButtonColor.Blue:
                if (blueImage != null)
                    blueImage.sprite = (lit && blueLit != null) ? blueLit : blueNormal;
                break;
            case SimonButton.ButtonColor.Green:
                if (greenImage != null)
                    greenImage.sprite = (lit && greenLit != null) ? greenLit : greenNormal;
                break;
            case SimonButton.ButtonColor.Yellow:
                if (yellowImage != null)
                    yellowImage.sprite = (lit && yellowLit != null) ? yellowLit : yellowNormal;
                break;
        }
    }

    private void PlayAudio(SimonButton.ButtonColor color)
    {
        if (audioSource == null) return;

        AudioClip clip = null;
        switch (color)
        {
            case SimonButton.ButtonColor.Red: clip = redClip; break;
            case SimonButton.ButtonColor.Blue: clip = blueClip; break;
            case SimonButton.ButtonColor.Green: clip = greenClip; break;
            case SimonButton.ButtonColor.Yellow: clip = yellowClip; break;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
    private IEnumerator FlashButtonPlayer(SimonButton.ButtonColor color)
    {
        SetLit(color, true);
        PlayPlayerAudio(color);
        yield return new WaitForSeconds(flashDuration);
        SetLit(color, false);
    }

    private void PlayPlayerAudio(SimonButton.ButtonColor color)
    {
        if (audioSource == null) return;

        AudioClip clip = null;
        switch (color)
        {
            case SimonButton.ButtonColor.Red: clip = redPlayerClip; break;
            case SimonButton.ButtonColor.Blue: clip = bluePlayerClip; break;
            case SimonButton.ButtonColor.Green: clip = greenPlayerClip; break;
            case SimonButton.ButtonColor.Yellow: clip = yellowPlayerClip; break;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
    public void ReceivePlayerInput(SimonButton.ButtonColor color)
    {
        if (!acceptingInput) return;

        StartCoroutine(FlashButtonPlayer(color));

        if (color != pattern[playerInputIndex])
        {
            acceptingInput = false;
            OnFailure?.Invoke();
            return;
        }

        playerInputIndex++;
        inputTimer = inputTimeout;

        if (playerInputIndex >= pattern.Count)
        {
            acceptingInput = false;
            OnSuccess?.Invoke();
        }
    }
}