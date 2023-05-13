using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculator : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float pathResolution = 0.25f;
    [SerializeField] private float maxBounceCount = 6;
    [SerializeField] private LayerMask targetMasks;
    private Path calculatedPath;
    private List<GameObject> points = new();
    #endregion

    private void Awake()
    {
        calculatedPath = new();
    }

    private Path CalculatePath(Vector3 start, Vector3 direction, float targetDistance)
    {
        ResetPath();

        Ray ray = new(start, direction);
        float remainingDistance = targetDistance;

        for (int i = 0; i < maxBounceCount; i++)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, remainingDistance, targetMasks))
            {
                remainingDistance = HandleRaycastHit(hit, ray, remainingDistance);
                if (remainingDistance <= 0) break;

                direction = Vector3.Reflect(ray.direction, hit.normal);
                ray = new(hit.point, direction);
            }
            else
            {
                AddRemainingPoints(ray, remainingDistance);
                calculatedPath.pathDistance += remainingDistance;
                break;
            }
        }

        return calculatedPath;
    }

    private void ResetPath()
    {
        calculatedPath.pathDistance = 0;
        calculatedPath.pathPoints.Clear();

        foreach (var point in points)
        {
            Destroy(point);
        }
        points.Clear();
    }

    private float HandleRaycastHit(RaycastHit hit, Ray ray, float remainingDistance)
    {
        if (hit.distance > remainingDistance)
        {
            AddRemainingPoints(ray, remainingDistance);
            return 0;
        }

        float segmentLength = Vector3.Distance(ray.origin, hit.point);
        AddSegmentPoints(ray, hit.point, segmentLength);

        if (hit.transform.gameObject.layer == LayerMask.GetMask("Hole"))
        {
            return 0;
        }

        calculatedPath.pathDistance += segmentLength;
        return remainingDistance - segmentLength;
    }

    private void AddSegmentPoints(Ray ray, Vector3 hitPoint, float segmentLength)
    {
        int segmentPoints = Mathf.FloorToInt(segmentLength / pathResolution);
        for (int j = 1; j <= segmentPoints; j++)
        {
            Vector3 point = Vector3.Lerp(ray.origin, hitPoint, (float)j / segmentPoints);
            AddPointToPath(point);
        }
    }

    private void AddRemainingPoints(Ray ray, float remainingDistance)
    {
        int remainingPoints = Mathf.FloorToInt(remainingDistance / pathResolution);
        for (int j = 1; j <= remainingPoints - 2; j++)
        {
            Vector3 point = ray.GetPoint(j * pathResolution);
            AddPointToPath(point);
        }
    }

    private void AddPointToPath(Vector3 point)
    {
        calculatedPath.pathPoints.Add(point);
        points.Add(Instantiate(pointPrefab, point, Quaternion.identity));
    }


}
[System.Serializable]
public class Path
{
    public float pathDistance;
    public List<Vector3> pathPoints = new();
}
