using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Components
    [SerializeField] private GameObject ballmodel;
    private Collider ballCollider;
    #endregion

    #region Variables
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnCollidedWithBall, DisableBallWithCheck);
        EventManager.StartListening(EventKeys.OnEnteredHole, DisableBallWithCheck);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnCollidedWithBall, DisableBallWithCheck);
        EventManager.StopListening(EventKeys.OnEnteredHole, DisableBallWithCheck);
    }

    private void Awake()
    {
        ballCollider = GetComponent<Collider>();
    }

    private void DisableBallWithCheck(object[] obj = null)
    {
        if ((Ball)obj[0] != this) return;
        DisableBall();
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
        ballCollider.enabled = true;
        transform.position = newPosition;
        ballmodel.SetActive(true);
        ballmodel.transform.DOScale(1, 0.5f).From(0).SetTarget(this);
    }
}
