using UnityEngine;

public class OfficeTurner : MonoBehaviour
{
    public GameObject frontView;
    public GameObject rightView;
    public GameObject backView;
    public GameObject leftView;

    public CameraMonitorController monitorController;

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
    }
}
