using UnityEngine;

public class WhiteBall : MonoBehaviour
{
    #region Components
    private PathFollower pathFollower;
    #endregion

    #region Variables
    [SerializeField] private Vector3 rotationPerSecond = new();
    public float distance = 10;
    private bool isOnTheBall = false;
    private Vector3 touchPosition;
    public float magnitude;
    #endregion

    #region Properties
    private bool IsMoving => pathFollower.IsFollowing;
    #endregion
    private void Awake()
    {
        pathFollower = GetComponent<PathFollower>();
    }

    private void FixedUpdate()
    {
        if (IsMoving) return;
        transform.Rotate(rotationPerSecond * Time.deltaTime);
        EventManager.TriggerEvent(EventKeys.OnPathCalculateRequested,new object[] {distance});
    }

    private void OnMouseDown()
    {
        isOnTheBall = true;
        touchPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!isOnTheBall) return;
        magnitude = (Input.mousePosition - touchPosition).magnitude;
    }

    private void OnMouseUp()
    {
        if (!isOnTheBall) return;

        isOnTheBall = false;
        EventManager.TriggerEvent(EventKeys.OnStartFollowPath);
    }
}
