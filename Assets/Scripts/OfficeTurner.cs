using UnityEngine;

public class OfficeTurner : MonoBehaviour
{
    public GameObject frontView;
    public GameObject rightView;
    public GameObject backView;
    public GameObject leftView;

    public CameraMonitorController monitorController;
    public CanvasGroup gateCanvasGroup;
    public CanvasGroup doorButtonCanvasGroup;
    public CanvasGroup simonPanelCanvasGroup;
    public CanvasGroup samVisualCanvasGroup;

    private int currentDirection = 0;

    void Start()
    {
        ShowCurrentDirection();
    }

    void Update()
    {
        if (monitorController != null && monitorController.IsMonitorOpen())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentDirection--;

            if (currentDirection < 0)
            {
                currentDirection = 3;
            }

            ShowCurrentDirection();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentDirection++;

            if (currentDirection > 3)
            {
                currentDirection = 0;
            }

            ShowCurrentDirection();
        }
    }

    void ShowCurrentDirection()
    {
        frontView.SetActive(currentDirection == 0);
        rightView.SetActive(currentDirection == 1);
        backView.SetActive(currentDirection == 2);
        leftView.SetActive(currentDirection == 3);

        // Gate only visible facing front (0)
        if (gateCanvasGroup != null)
        {
            gateCanvasGroup.alpha = (currentDirection == 0) ? 1f : 0f;
            gateCanvasGroup.blocksRaycasts = (currentDirection == 0);
        }

        // Door button only visible facing left (3)
        if (doorButtonCanvasGroup != null)
        {
            doorButtonCanvasGroup.alpha = (currentDirection == 3) ? 1f : 0f;
            doorButtonCanvasGroup.blocksRaycasts = (currentDirection == 3);
        }

        // Simon panel only visible facing front (0)
        if (simonPanelCanvasGroup != null)
        {
            simonPanelCanvasGroup.alpha = (currentDirection == 0) ? 1f : 0f;
            simonPanelCanvasGroup.blocksRaycasts = (currentDirection == 0);
        }

        // Sam only visible facing front (0)
        if (samVisualCanvasGroup != null)
        {
            samVisualCanvasGroup.alpha = (currentDirection == 0) ? 1f : 0f;
            samVisualCanvasGroup.blocksRaycasts = (currentDirection == 0);
        }

    }
}