using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TiltNavigation : MonoBehaviour
{
    public float tiltThreshold = 20f; // Degrees to trigger navigation
    private Selectable _currentSelectable;

    [SerializeField] private PlayerControl playerControl;

    private bool _tiltUp = false;
    private bool _tiltDown = false;
    private bool _tiltLeft = false;
    private bool _tiltRight = false;

    private void Start()
    {
        // Attempt to find the initial selectable
        _currentSelectable = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>()?? FindObjectOfType<Button>();

        if (_currentSelectable == null)
        {
            Debug.LogWarning("No selectable UI element found at Start.");
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(_currentSelectable.gameObject);
        }
    }

    private void Update()
    {
        // Guard for null playerControl or rotation
        if (playerControl == null || playerControl.playerVariables == null || playerControl.playerVariables.rotation == null)
        {
            Debug.LogWarning("PlayerControl or rotation data is null. Skipping tilt navigation.");
            return;
        }

        // Reassign currentSelectable if lost
        if (_currentSelectable == null)
        {
            Debug.LogWarning("No current selectable. Attempting to find one.");
            _currentSelectable = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (_currentSelectable == null) return; // Still null, exit early
        }

        // Calculate tilt
        Vector3 tilt = GetTiltFromQuaternion(playerControl.playerVariables.rotation);

        // Detect tilt and move if in the neutral zone previously
        if (tilt.x > tiltThreshold && !_tiltDown) // Forward tilt
        {
            MoveToSelectable(_currentSelectable.FindSelectableOnDown());
            _tiltDown = true;
        }
        else if (tilt.x < -tiltThreshold && !_tiltUp) // Backward tilt
        {
            MoveToSelectable(_currentSelectable.FindSelectableOnUp());
            _tiltUp = true;
        }
        else if (tilt.z > tiltThreshold && !_tiltRight) // Right tilt
        {
            MoveToSelectable(_currentSelectable.FindSelectableOnRight());
            _tiltRight = true;
        }
        else if (tilt.z < -tiltThreshold && !_tiltLeft) // Left tilt
        {
            MoveToSelectable(_currentSelectable.FindSelectableOnLeft());
            _tiltLeft = true;
        }

        // Reset registered states when back in neutral zone
        if (Mathf.Abs(tilt.x) < tiltThreshold / 2 && Mathf.Abs(tilt.z) < tiltThreshold / 2)
        {
            _tiltUp = false;
            _tiltDown = false;
            _tiltLeft = false;
            _tiltRight = false;
        }

        // Handle action button
        if (playerControl.playerVariables.actionThing)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected != null)
            {
                ExecuteEvents.Execute(selected, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }
    }

    private void MoveToSelectable(Selectable next)
    {
        if (next != null)
        {
            Debug.Log($"Navigating to: {next.gameObject.name}");
            _currentSelectable = next;
            next.Select();
        }
    }

    //More detailed EulerAngles for calculating the normalized Vector3 rotation
    Vector3 GetTiltFromQuaternion(Quaternion q)
    {
        // Adjust for device orientation
        Quaternion adjusted = new Quaternion(q.x, q.y, -q.z, -q.w);
        Vector3 euler = adjusted.eulerAngles;

        // Normalize angles (-180 to 180 degrees)
        euler = new Vector3(
            Mathf.DeltaAngle(euler.x, 0),
            Mathf.DeltaAngle(euler.y, 0),
            Mathf.DeltaAngle(euler.z, 0)
        );

        return euler;
    }
}
