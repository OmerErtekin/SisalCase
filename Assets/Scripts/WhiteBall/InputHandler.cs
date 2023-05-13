using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    private PathFollower pathFollower;
    #endregion

    #region Variables
    [SerializeField] private float minPower = 1, maxPower = 10;
    private bool isOnTheBall = false;
    public float powerMagnitude;
    private Vector3 realWorldFirstTouch, realWorldCurrent, currentDirection;
    #endregion

    #region Properties
    private bool IsMoving => pathFollower.IsFollowing;
    public float PowerPercantage => powerMagnitude / maxPower;
    #endregion
    private void Awake()
    {
        mainCamera = Camera.main;
        pathFollower = GetComponent<PathFollower>();
    }

    private void FixedUpdate()
    {
        if (IsMoving || !isOnTheBall) return;
        RotateBallWhileDragging();
    }

    private void RotateBallWhileDragging()
    {
        EventManager.TriggerEvent(EventKeys.OnPathCalculateRequested, new object[] { transform.position, currentDirection, powerMagnitude });
    }

    private void OnMouseDown()
    {
        isOnTheBall = true;
        realWorldFirstTouch = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        realWorldFirstTouch.y = 0;
    }

    private void OnMouseDrag()
    {
        if (!isOnTheBall) return;
        realWorldCurrent = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        realWorldCurrent.y = 0;

        powerMagnitude = (realWorldCurrent - realWorldFirstTouch).sqrMagnitude * 4;
        powerMagnitude = Mathf.Clamp(powerMagnitude, minPower, maxPower);
        currentDirection = (realWorldFirstTouch - realWorldCurrent).normalized;
    }

    private void OnMouseUp()
    {
        if (!isOnTheBall) return;

        isOnTheBall = false;
        EventManager.TriggerEvent(EventKeys.OnStartFollowPath, new object[] { powerMagnitude });
    }
}
