using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    [SerializeField] private GameObject ballModel;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnEnteredHole, HideTheBall);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnEnteredHole, HideTheBall);
    }

    private void ShowTheBall(object[] obj = null)
    {
        ballModel.SetActive(true);
    }

    private void HideTheBall(object[] obj = null)
    {
        ballModel.SetActive(false);
    }

}
