using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Shows short on-screen notifications. Lives on its own nested Canvas so showing/hiding
/// toggles Canvas.enabled (no mesh rebuild) and never dirties the rest of the HUD.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class NotificationsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notificationText;
    [SerializeField] float displayDuration = 3f;

    [Header("Components")]
    [SerializeField] Canvas notificationCanvas;

    Coroutine hideRoutine;
    WaitForSeconds hideDelay;

    void OnValidate()
    {
        if (notificationCanvas == null)
        {
            notificationCanvas = GetComponent<Canvas>();
        }
    }

    void Awake()
    {
        // Objects set up before this script required a Canvas won't have one yet.
        if (notificationCanvas == null)
        {
            notificationCanvas = GetComponent<Canvas>();
            if (notificationCanvas == null)
            {
                notificationCanvas = gameObject.AddComponent<Canvas>();
            }
        }

        hideDelay = new WaitForSeconds(displayDuration);
    }

    void OnEnable()
    {
        Trailcam.OnMotionDetected += HandleMotionDetected;
        notificationCanvas.enabled = false;
    }

    void OnDisable()
    {
        Trailcam.OnMotionDetected -= HandleMotionDetected;
    }

    void HandleMotionDetected(Trailcam trailcam)
    {
        ShowNotification($"Motion detected at {trailcam.CameraName}");
    }

    public void ShowNotification(string message)
    {
        // Only touch the text when it changes; assigning it always regenerates the mesh.
        if (notificationText.text != message)
        {
            notificationText.text = message;
        }
        notificationCanvas.enabled = true;

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return hideDelay;
        notificationCanvas.enabled = false;
        hideRoutine = null;
    }
}
