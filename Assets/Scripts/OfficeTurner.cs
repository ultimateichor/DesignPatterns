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
    public CanvasGroup nSigmaFrontCanvasGroup;
    public CanvasGroup nSigmaLeftCanvasGroup;
    public CanvasGroup burtlePanelCanvasGroup;


    private int currentDirection = 0;

    void Start()
    {
        ShowCurrentDirection();
    }

    void Update()
    {
        // Keep BurtlePanel hidden when monitor is open, even if facing back
        if (burtlePanelCanvasGroup != null)
        {
            bool monitorOpen = monitorController != null && monitorController.IsMonitorOpen();
            bool showBurtle = (currentDirection == 2) && !monitorOpen;
            burtlePanelCanvasGroup.alpha = showBurtle ? 1f : 0f;
            burtlePanelCanvasGroup.blocksRaycasts = showBurtle;
        }

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

        // N. Sigma front visual only visible facing front (0)
        if (nSigmaFrontCanvasGroup != null)
        {
            nSigmaFrontCanvasGroup.alpha = (currentDirection == 0) ? 1f : 0f;
            nSigmaFrontCanvasGroup.blocksRaycasts = false;
        }

        // N. Sigma left visual only visible facing left (3)
        if (nSigmaLeftCanvasGroup != null)
        {
            nSigmaLeftCanvasGroup.alpha = (currentDirection == 3) ? 1f : 0f;
            nSigmaLeftCanvasGroup.blocksRaycasts = false;
        }

        // Burtle panel only visible facing back (2)
        if (burtlePanelCanvasGroup != null)
        {
            burtlePanelCanvasGroup.alpha = (currentDirection == 2) ? 1f : 0f;
            burtlePanelCanvasGroup.blocksRaycasts = (currentDirection == 2);
        }

    }
}