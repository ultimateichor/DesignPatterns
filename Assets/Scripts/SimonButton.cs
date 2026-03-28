using UnityEngine;
using UnityEngine.EventSystems;

public class SimonButton : MonoBehaviour, IPointerClickHandler
{
    public enum ButtonColor { Red, Blue, Green, Yellow }
    public ButtonColor buttonColor;
    public SimonController simonController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (simonController != null)
            simonController.ReceivePlayerInput(buttonColor);
    }
}
