using UnityEngine;
using UnityEngine.EventSystems;

public class DragGate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform gateRect;
    private RectTransform parentRect;
    private Canvas parentCanvas;

    [Header("Gate Limits")]
    public float upY = 460f;
    public float downY = 142f;

    [Header("Return Speed")]
    public float returnSpeed = 600f;

    private bool isDragging = false;
    private float dragOffsetY = 0f;

    private void Awake()
    {
        gateRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        parentRect = gateRect.parent as RectTransform;
    }

    private void Update()
    {
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging)
        {
            Vector2 pos = gateRect.anchoredPosition;
            pos.y = Mathf.MoveTowards(pos.y, upY, returnSpeed * Time.deltaTime);
            gateRect.anchoredPosition = pos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos
        );

        dragOffsetY = gateRect.anchoredPosition.y - localMousePos.y;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public bool IsGateDown()
    {
        return gateRect != null && gateRect.anchoredPosition.y <= downY + 80f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos
        );

        Vector2 pos = gateRect.anchoredPosition;
        pos.y = localMousePos.y + dragOffsetY;
        pos.y = Mathf.Clamp(pos.y, downY, upY);

        gateRect.anchoredPosition = pos;
    }
}