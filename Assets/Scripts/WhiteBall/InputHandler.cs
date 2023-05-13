using DG.Tweening;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    private PathFollower pathFollower;
    #endregion

    #region Variables
    [SerializeField] private Transform stickModel,stickCenter;
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
        SetStickPositionAndRotation();
    }

    private void OnMouseUp()
    {
        if (!isOnTheBall) return;
        isOnTheBall = false;
        HitWithStick();
    }

    private void SetStickPositionAndRotation()
    {
        stickModel.gameObject.SetActive(true);
        if (currentDirection != Vector3.zero)
            stickCenter.rotation = Quaternion.LookRotation(currentDirection);
        stickCenter.Rotate(0, -90, 0);
        stickModel.transform.localPosition = new Vector3(-powerMagnitude / 2, 0, 0);
    }

    private void HitWithStick()
    {
        stickModel.DOKill();
        stickModel.DOLocalMoveX(0f, 0.5f).SetTarget(this).SetEase(Ease.InBack).OnComplete(()=>
        {
            EventManager.TriggerEvent(EventKeys.OnStartFollowPath, new object[] { powerMagnitude });
            stickModel.gameObject.SetActive(false);
        });
    }
}
