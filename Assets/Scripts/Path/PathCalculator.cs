using System.Collections.Generic;
using UnityEngine;

public class PathCalculator : MonoBehaviour
{
    #region Variables
    [SerializeField] private float pathResolution = 0.25f,sphereCastRadius = 0.075f;
    [SerializeField] private int maxBounceCount = 6;
    [SerializeField] private LayerMask targetMasks;
    [SerializeField] private int holeLayerIndex;
    private BallPath calculatedPath = new();

    private Vector3 lastPos, lastDir;
    private float lastMagnitude;
    #endregion

    #region Properties
    public BallPath CurentPath => calculatedPath;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnPathCalculateRequested, CalculatePath);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnPathCalculateRequested, CalculatePath);
    }

    private void CalculatePath(object[] obj = null)
    {
        Vector3 start = (Vector3)obj[0];
        Vector3 direction = (Vector3)obj[1];
        float targetDistance = (float)obj[2];

        if (IsAlreadyCalculated(start, direction, targetDistance)) return;

        ResetPath();

        lastPos = start;
        lastDir = direction;
        lastMagnitude = targetDistance;

        Ray ray = new(start, direction);
        float remainingDistance = targetDistance;

        for (int i = 0; i < maxBounceCount; i++)
        {
            if (Physics.SphereCast(ray,sphereCastRadius, out RaycastHit hit, remainingDistance, targetMasks))
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

        EventManager.TriggerEvent(EventKeys.OnPathCalculateCompleted, new object[] { calculatedPath });
    }

    private void ResetPath()
    {
        calculatedPath.pathDistance = 0;
        calculatedPath.pathPositions.Clear();
    }

    private float HandleRaycastHit(RaycastHit hit, Ray ray, float remainingDistance)
    {
        if (hit.distance > remainingDistance)
        {
            calculatedPath.pathDistance += remainingDistance;
            AddRemainingPoints(ray, remainingDistance);
            return 0;
        }

        float segmentLength = Vector3.Distance(ray.origin, hit.point);
        AddSegmentPoints(ray, hit.point, segmentLength);

        calculatedPath.pathDistance += segmentLength;

        if (hit.transform.gameObject.layer == holeLayerIndex)
        {
            return 0;
        }

        return remainingDistance - segmentLength;
    }

    private void AddSegmentPoints(Ray ray, Vector3 hitPoint, float segmentLength)
    {
        int segmentPoints = Mathf.FloorToInt(segmentLength / pathResolution);
        for (int j = 1; j <= segmentPoints; j++)
        {
            Vector3 position = Vector3.Lerp(ray.origin, hitPoint, (float)j / segmentPoints);
            calculatedPath.pathPositions.Add(position);
        }
    }

    private void AddRemainingPoints(Ray ray, float remainingDistance)
    {
        int remainingPoints = Mathf.FloorToInt(remainingDistance / pathResolution);
        for (int j = 1; j <= remainingPoints - 2; j++)
        {
            Vector3 position = ray.GetPoint(j * pathResolution);
            calculatedPath.pathPositions.Add(position);
        }
    }

    private bool IsAlreadyCalculated(Vector3 pos,Vector3 dir,float magnitude)
    {
        return pos == lastPos && dir == lastDir && magnitude == lastMagnitude;
    }
}
[System.Serializable]
public class BallPath
{
    public float pathDistance;
    public List<Vector3> pathPositions = new();
}
