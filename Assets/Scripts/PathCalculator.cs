using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculator : MonoBehaviour
{
    #region Variables
    public Transform testTransform;
    [SerializeField] private float pathResolution = 0.25f;
    [SerializeField] private int maxBounceCount = 6;
    [SerializeField] private LayerMask targetMasks;
    [SerializeField] private int holeLayerIndex;
    private Path calculatedPath;

    private Vector3 lastDirection, lastPosition;
    private float lastDistance;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnPathCalculateRequested, CalculatePath);
    }

    private void OnDisable()
    {

    }

    private void Awake()
    {
        calculatedPath = new();
    }

    private void CalculatePath(object[] obj = null)
    {
        Vector3 start = (Vector3)obj[0];
        Vector3 direction = (Vector3)obj[1];
        float targetDistance = (float)obj[2];

        if (IsAlreadyCalculated(start, direction, targetDistance)) return;


        ResetPath();
        lastPosition = start;
        lastDirection = direction;
        lastDistance = targetDistance;

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
            AddRemainingPoints(ray, remainingDistance);
            return 0;
        }

        float segmentLength = Vector3.Distance(ray.origin, hit.point);
        AddSegmentPoints(ray, hit.point, segmentLength);
        if (hit.transform.gameObject.layer == holeLayerIndex)
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

    private bool IsAlreadyCalculated(Vector3 position, Vector3 direction, float distance)
    {
        return lastPosition == position && lastDirection == direction && lastDistance == distance;
    }
}
[System.Serializable]
public class Path
{
    public float pathDistance;
    public List<Vector3> pathPositions = new();
}
