using UnityEngine;
using UnityEngine.UI;

public class CameraFeedController : MonoBehaviour
{
    [System.Serializable]
    public class CameraFeed
    {
        public string cameraName;
        public Sprite feedSprite;
    }

    [Header("UI")]
    public Image feedImage;

    [Header("Feeds")]
    public CameraFeed[] cameraFeeds;

    [Header("State")]
    public int currentCameraIndex = 0;

    private void Start()
    {
        ShowCamera(currentCameraIndex);
    }

    public void ShowCamera(int index)
    {
        if (cameraFeeds == null || cameraFeeds.Length == 0)
            return;

        if (index < 0 || index >= cameraFeeds.Length)
            return;

        currentCameraIndex = index;

        if (feedImage != null && cameraFeeds[index].feedSprite != null)
        {
            feedImage.sprite = cameraFeeds[index].feedSprite;
        }

        Debug.Log("Showing camera: " + cameraFeeds[index].cameraName);
    }

    public int GetCurrentCameraIndex()
    {
        return currentCameraIndex;
    }

    public string GetCurrentCameraName()
    {
        if (cameraFeeds == null || currentCameraIndex < 0 || currentCameraIndex >= cameraFeeds.Length)
            return "";

        return cameraFeeds[currentCameraIndex].cameraName;
    }
}