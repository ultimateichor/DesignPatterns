using UnityEngine;

/// <summary>
/// Handles A/D office-rotation and view locking.
///
/// FIX: Views are now hidden via CanvasGroup (alpha/raycasts) instead of
/// SetActive, so MonoBehaviours (doors, gates, Simon Says, etc.) keep
/// running while a view is not visible — no more action-freeze on view swap.
/// </summary>
public class OfficeTurner : MonoBehaviour
{
    [Header("Office Views")]
    public GameObject frontView;
    public GameObject rightView;
    public GameObject backView;
    public GameObject leftView;

    [Header("References")]
    public CameraMonitorController monitorController;

    // ── Runtime state ─────────────────────────────────────────────────────
    private int  currentDirection = 0;   // 0 Front · 1 Right · 2 Back · 3 Left
    private bool viewLocked       = false;

    // One CanvasGroup per view (auto-created in Awake if missing)
    private CanvasGroup frontGroup;
    private CanvasGroup rightGroup;
    private CanvasGroup backGroup;
    private CanvasGroup leftGroup;

    // ── Unity ─────────────────────────────────────────────────────────────
    void Awake()
    {
        // Guarantee each view has a CanvasGroup so we can hide without
        // disabling the GameObject (which would freeze coroutines)
        frontGroup = EnsureCanvasGroup(frontView);
        rightGroup = EnsureCanvasGroup(rightView);
        backGroup  = EnsureCanvasGroup(backView);
        leftGroup  = EnsureCanvasGroup(leftView);
    }

    void Start()
    {
        RefreshViews();
    }

    void Update()
    {
        // No input allowed if view is locked (animatronic visit) or monitor is open
        if (viewLocked) return;
        if (monitorController != null && monitorController.IsMonitorOpen()) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentDirection = (currentDirection - 1 + 4) % 4;
            RefreshViews();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentDirection = (currentDirection + 1) % 4;
            RefreshViews();
        }
    }

    // ── Public API ────────────────────────────────────────────────────────

    /// <summary>Force the player to a specific view (0–3).</summary>
    public void SetView(int direction)
    {
        currentDirection = Mathf.Clamp(direction, 0, 3);
        RefreshViews();
    }

    /// <summary>
    /// Lock or unlock view rotation.
    /// When locked the player cannot press A/D to rotate.
    /// </summary>
    public void LockView(bool locked)
    {
        viewLocked = locked;
    }

    public int GetCurrentDirection() => currentDirection;

    // ── Internals ─────────────────────────────────────────────────────────

    void RefreshViews()
    {
        SetGroupVisible(frontGroup, currentDirection == 0);
        SetGroupVisible(rightGroup, currentDirection == 1);
        SetGroupVisible(backGroup,  currentDirection == 2);
        SetGroupVisible(leftGroup,  currentDirection == 3);
    }

    static void SetGroupVisible(CanvasGroup group, bool visible)
    {
        if (group == null) return;
        group.alpha          = visible ? 1f : 0f;
        group.blocksRaycasts = visible;
        group.interactable   = visible;
    }

    static CanvasGroup EnsureCanvasGroup(GameObject go)
    {
        if (go == null) return null;
        var cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}
