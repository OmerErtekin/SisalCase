using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    #region Components
    private PointPool pool;
    #endregion

    #region Variables
    private Path path;
    private List<VisiblePoint> points = new();
    #endregion
    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnPathCalculateCompleted, DrawPath);
        EventManager.StartListening(EventKeys.OnStartFollowPath, HidePath);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnPathCalculateCompleted, DrawPath);
        EventManager.StopListening(EventKeys.OnStartFollowPath, HidePath);
    }

    private void Awake()
    {
        pool = GetComponent<PointPool>();
    }

    private void DrawPath(object[] obj = null)
    {
        HidePath();
        path = (Path)obj[0];
        VisiblePoint point;
        for (int i = 0; i < path.pathPositions.Count; i++)
        {
            point = pool.GetPoint();
            point.ShowPoint(path.pathPositions[i]);
            points.Add(point);
        }
    }

    private void HidePath(object[] obj = null)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].HidePoint();
        }
        points.Clear();
    }
}
