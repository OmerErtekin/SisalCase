using System.Collections.Generic;
using UnityEngine;

public class PointPool : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private int initialPoolSize = 50;
    private Queue<VisiblePoint> pointPool = new();
    #endregion

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPoint();
        }
    }

    public VisiblePoint GetPoint()
    {
        if (pointPool.Count <= 0)
        {
            CreateNewPoint();
        }
        return pointPool.Dequeue();
    }

    public void AddPointToPool(VisiblePoint block)
    {
        pointPool.Enqueue(block);
    }

    private void CreateNewPoint()
    {
        VisiblePoint pointScript = Instantiate(pointPrefab, parentTransform).GetComponent<VisiblePoint>();
        pointScript.SetPointPool(this);
        pointScript.gameObject.SetActive(false);
        pointPool.Enqueue(pointScript);
    }
}
