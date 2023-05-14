using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Components
    [SerializeField] private GameObject ballmodel;
    [SerializeField] private AudioSource ballSound;
    private SphereCollider ballCollider;
    #endregion

    #region Variables
    [SerializeField] private float radiusAtReplacing, normalRadius;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnCollidedWithBall, DisableBallWithCheck);
        EventManager.StartListening(EventKeys.OnEnteredHole, DisableBallWithCheck);
        EventManager.StartListening(EventKeys.OnEnteredHole, ExpandColliderForReplacing);
        EventManager.StartListening(EventKeys.OnWhiteBallReplaced, ShrinkColliderAfterReplace);
        EventManager.StartListening(EventKeys.OnWhiteBallReplaced, EnableBallWithCheck);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnCollidedWithBall, DisableBallWithCheck);
        EventManager.StopListening(EventKeys.OnEnteredHole, DisableBallWithCheck);
        EventManager.StopListening(EventKeys.OnWhiteBallReplaced, EnableBallWithCheck);
        EventManager.StopListening(EventKeys.OnEnteredHole, ExpandColliderForReplacing);
        EventManager.StopListening(EventKeys.OnWhiteBallReplaced, ShrinkColliderAfterReplace);
    }

    private void Awake()
    {
        ballCollider = GetComponent<SphereCollider>();
    }

    private void DisableBallWithCheck(object[] obj = null)
    {
        if ((Ball)obj[0] != this) return;
        ballSound.Play();
        DisableBall();
    }

    private void ExpandColliderForReplacing(object[] obj = null)
    {
        //We are expanding other balls colliders to prevent white ball to collide.
        if ((Ball)obj[0] == this) return;
        ballCollider.radius = radiusAtReplacing;
    }

    private void ShrinkColliderAfterReplace(object[] obj = null)
    {
        //We reset our colliders
        if ((Ball)obj[0] == this) return;
        ballCollider.radius = normalRadius;
    }

    private void EnableBallWithCheck(object[] obj = null)
    {
        if ((Ball)obj[0] != this) return;
        EnableBall((Vector3)obj[1]);
    }

    private void DisableBall()
    {
        ballCollider.enabled = false;
        ballmodel.transform.DOKill();
        ballmodel.transform.DOScale(0, 0.25f).SetTarget(this).OnComplete(() =>
        {
            ballmodel.SetActive(false);
            transform.position = Vector3.one * 100;
        });
    }

    public void EnableBall(Vector3 newPosition)
    {
        ballmodel.transform.DOKill();
        transform.position = newPosition;
        ballmodel.SetActive(true);
        ballmodel.transform.DOScale(1, 0.5f).From(0).SetTarget(this).OnComplete(() => ballCollider.enabled = true);
    }
}
