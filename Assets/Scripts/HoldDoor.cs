using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleDoor : MonoBehaviour, IPointerClickHandler
{
    [Header("Door")]
    public RectTransform doorRect;

    public float openX = -500f;
    public float closedX = 0f;

    public float moveSpeed = 1200f;

    [Header("Button Visuals")]
    public Image buttonImage;
    public Sprite openSprite;    // red
    public Sprite closedSprite;  // green

    [Header("Settings")]
    public float snapThreshold = 1f;

    private bool isClosed = false;

    private void Start()
    {
        UpdateButtonVisual();
    }

    private void Update()
    {
        if (doorRect == null)
            return;

        float targetX = isClosed ? closedX : openX;

        Vector2 pos = doorRect.anchoredPosition;
        pos.x = Mathf.MoveTowards(pos.x, targetX, moveSpeed * Time.deltaTime);

        // snap to target when close enough
        if (Mathf.Abs(pos.x - targetX) < snapThreshold)
        {
            pos.x = targetX;
        }

        doorRect.anchoredPosition = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsDoorIdle())
            return;

        isClosed = !isClosed;

        UpdateButtonVisual();
    }

    private bool IsDoorIdle()
    {
        float x = doorRect.anchoredPosition.x;

        bool atOpen = Mathf.Abs(x - openX) < snapThreshold;
        bool atClosed = Mathf.Abs(x - closedX) < snapThreshold;

        return atOpen || atClosed;
    }

    private void UpdateButtonVisual()
    {
        if (buttonImage == null)
            return;

        buttonImage.sprite = isClosed ? closedSprite : openSprite;
    }
}