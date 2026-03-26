using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// One coloured panel on the Simon Says wall.
/// Handles its own flash animation and click detection.
/// </summary>
public class SimonButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Colour")]
    public Color litColor = Color.red;

    [Header("Label (set in Inspector)")]
    public string colorName = "Red";

    // Set by SimonSaysController at runtime
    [HideInInspector] public int buttonIndex;

    // ── Private ──────────────────────────────────────────────────────────
    private Image img;
    private Color dimColor;

    private bool interactable;
    private Action<int> clickCallback;

    // ── Unity ─────────────────────────────────────────────────────────────
    void Awake()
    {
        img = GetComponent<Image>();

        // Dim version = 25 % brightness of the lit colour, fully opaque
        dimColor = new Color(litColor.r * 0.25f,
                             litColor.g * 0.25f,
                             litColor.b * 0.25f,
                             1f);
        img.color = dimColor;
    }

    // ── Public API ────────────────────────────────────────────────────────

    /// <summary>
    /// Plays one flash step (lit → wait → dim → wait).
    /// Yielded by SimonSaysController during sequence playback.
    /// </summary>
    public IEnumerator FlashRoutine(float onDuration, float offDuration)
    {
        img.color = litColor;
        yield return new WaitForSeconds(onDuration);
        img.color = dimColor;
        yield return new WaitForSeconds(offDuration);
    }

    /// <summary>
    /// Immediately set the button lit or dim (used for success/fail flashes).
    /// </summary>
    public void SetLit(bool on)
    {
        img.color = on ? litColor : dimColor;
    }

    /// <summary>
    /// Enable or disable player click input on this button.
    /// </summary>
    public void SetInteractable(bool state, Action<int> callback = null)
    {
        interactable   = state;
        clickCallback  = callback;
    }

    // ── IPointerClickHandler ──────────────────────────────────────────────
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;

        // Give brief visual feedback on click
        StartCoroutine(ClickFlash());

        clickCallback?.Invoke(buttonIndex);
    }

    private IEnumerator ClickFlash()
    {
        img.color = litColor;
        yield return new WaitForSeconds(0.12f);
        img.color = dimColor;
    }
}
