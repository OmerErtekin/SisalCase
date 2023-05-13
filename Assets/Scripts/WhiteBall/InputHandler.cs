using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region Components
    private PathFollower pathFollower;
    #endregion

    #region Variables
    [SerializeField] private Vector3 rotationPerSecond = new();
    [SerializeField] private float minPower = 1, maxPower = 10;
    private bool isOnTheBall = false;
    private float powerMagnitude, distance;
    private Vector3 touchPosition;
    #endregion

    #region Properties
    private bool IsMoving => pathFollower.IsFollowing;
    public float PowerPercantage => distance / maxPower;
    #endregion
    private void Awake()
    {
        pathFollower = GetComponent<PathFollower>();
    }

    private void FixedUpdate()
    {
        if (IsMoving || !isOnTheBall) return;
        RotateBallWhileDragging();
    }

    private void RotateBallWhileDragging()
    {
        transform.Rotate(rotationPerSecond * Time.deltaTime);
        EventManager.TriggerEvent(EventKeys.OnPathCalculateRequested, new object[] { distance });
    }

    private void OnMouseDown()
    {
        isOnTheBall = true;
        touchPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!isOnTheBall) return;
        powerMagnitude = (Input.mousePosition - touchPosition).magnitude / 50;
        distance = Mathf.Clamp(powerMagnitude, minPower, maxPower);
    }

    private void OnMouseUp()
    {
        if (!isOnTheBall) return;

        isOnTheBall = false;
        EventManager.TriggerEvent(EventKeys.OnStartFollowPath);
    }
}
