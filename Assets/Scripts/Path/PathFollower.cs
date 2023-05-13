using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    #region Components
    private PathCalculator pathCalculator;
    #endregion

    #region Variables
    private Coroutine followRoutine;
    private bool isFollowing;
    private BallPath currentPath;
    #endregion

    #region Properties
    public bool IsFollowing => isFollowing;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnStartFollowPath, FollowPath);
        EventManager.StartListening(EventKeys.OnEnteredHole, StopFollow);
        EventManager.StartListening(EventKeys.OnPathCalculateCompleted, SetCurrentPath);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnStartFollowPath, FollowPath);
        EventManager.StopListening(EventKeys.OnEnteredHole, StopFollow);
        EventManager.StopListening(EventKeys.OnPathCalculateCompleted, SetCurrentPath);
    }

    private void Awake()
    {
        pathCalculator = GetComponent<PathCalculator>();
    }

    private void FollowPath(object[] obj = null)
    {
        StopFollow();
        followRoutine = StartCoroutine(FollowRoutine((float)obj[0]));
    }

    private void StopFollow(object[] obj = null)
    {
        if (followRoutine != null)
            StopCoroutine(followRoutine);
        isFollowing = false;
    }

    private void SetCurrentPath(object[] obj = null) => currentPath = (BallPath)obj[0];

    private IEnumerator FollowRoutine(float hitMagnitude)
    {
        float baseSpeedFactor = hitMagnitude / 4;

        float currentSpeed = 4 * baseSpeedFactor;
        float minSpeed = baseSpeedFactor / 6;

        Vector3 targetDirection;

        isFollowing = true;

        for (int i = 0; i < currentPath.pathPositions.Count - 1; i++)
        {
            while ((transform.position - currentPath.pathPositions[i + 1]).sqrMagnitude > 0.001f)
            {

                targetDirection = (currentPath.pathPositions[i + 1] - transform.position).normalized;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), 1080 * Time.deltaTime);
                currentSpeed = Mathf.Max(minSpeed, currentSpeed * Mathf.Exp(-Time.deltaTime));
                transform.position = Vector3.MoveTowards(transform.position, currentPath.pathPositions[i + 1], currentSpeed * Time.deltaTime);
                yield return null;
            }
        }
        StopFollow();
    }
}
