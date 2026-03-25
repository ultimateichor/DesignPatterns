using UnityEngine;

public class CameraMonitorController : MonoBehaviour
{
    [Header("References")]
    public RectTransform cameraMonitorRect;

    [Header("Positions")]
    public float openY = 0f;
    public float closedY = -900f;

    [Header("Settings")]
    public float slideSpeed = 1200f;

    [Header("State")]
    public bool monitorOpen = false;

    private void Start()
    {
        if (cameraMonitorRect != null)
        {
            Vector2 pos = cameraMonitorRect.anchoredPosition;
            pos.y = closedY;
            cameraMonitorRect.anchoredPosition = pos;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsMonitorMoving())
        {
            ToggleMonitor();
        }

        UpdateMonitorPosition();
    }

    public void ToggleMonitor()
    {
        monitorOpen = !monitorOpen;
    }

    private void UpdateMonitorPosition()
    {
        if (cameraMonitorRect == null)
            return;

        Vector2 pos = cameraMonitorRect.anchoredPosition;
        float targetY = monitorOpen ? openY : closedY;

        pos.y = Mathf.MoveTowards(pos.y, targetY, slideSpeed * Time.deltaTime);

        cameraMonitorRect.anchoredPosition = pos;
    }

    public bool IsMonitorOpen()
    {
        return monitorOpen;
    }

    public bool IsMonitorFullyOpen()
    {
        return cameraMonitorRect != null &&
               Mathf.Abs(cameraMonitorRect.anchoredPosition.y - openY) < 1f;
    }

    public bool IsMonitorMoving()
    {
        if (cameraMonitorRect == null)
            return false;

        float targetY = monitorOpen ? openY : closedY;
        return Mathf.Abs(cameraMonitorRect.anchoredPosition.y - targetY) > 1f;
    }
}
