using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisiblePoint : MonoBehaviour
{
    #region Components
    private PointPool pool;
    #endregion

    #region Variables
    #endregion

    public void ShowPoint(Vector3 showPosition)
    {
        gameObject.SetActive(true);
        transform.position = showPosition;
    }

    public void HidePoint()
    {
        pool.AddPointToPool(this);
        gameObject.SetActive(false);
    }

    public void SetPointPool(PointPool pointPool)
    {
        pool = pointPool;
    }
}
