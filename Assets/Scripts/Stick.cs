using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    [SerializeField] private GameObject stickModel;
    [SerializeField] private float minLocalDistance = 1, maxLocalDistance = 6;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnPathCalculateRequested, SetStickPositionAndRotation);
        EventManager.StartListening(EventKeys.OnStartFollowPath, HideStick);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnPathCalculateRequested, SetStickPositionAndRotation);
        EventManager.StopListening(EventKeys.OnStartFollowPath, HideStick);
    }

    private void ShowStick(object obj = null)
    {
        stickModel.SetActive(true);
    }

    private void HideStick(object obj = null)
    {
        stickModel.SetActive(false);
    }

    private void ReleaseStick()
    {

    }

    private void SetStickPositionAndRotation(object[] obj = null)
    {
        ShowStick();
        Vector3 targetDirection = (Vector3)obj[1];
        if (targetDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(targetDirection);
        transform.Rotate(0, -90, 0);
        stickModel.transform.localPosition = new Vector3(-(float)obj[2]/2, 0, 0);
    }
}
