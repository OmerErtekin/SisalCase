using System.Collections;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    #region Components
    private PathCalculator pathCalculator;
    #endregion

    #region Variables
    private Coroutine followRoutine;
    private bool isFollowing;
    #endregion

    #region Properties
    public bool IsFollowing => isFollowing;
    private Path CurrentPath => pathCalculator.CurentPath;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnStartFollowPath, FollowPath);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnStartFollowPath, FollowPath);
    }

    private void Awake()
    {
        pathCalculator = GetComponent<PathCalculator>();
    }

    private void FollowPath(object[] obj = null)
    {
        StopFollow();
        followRoutine = StartCoroutine(FollowRoutine());
    }

    private void StopFollow(object[] obj = null)
    {
        if(followRoutine != null)
            StopCoroutine(followRoutine);
        isFollowing = false;
    }

    private IEnumerator FollowRoutine()
    {
        float baseSpeedFactor = CurrentPath.pathDistance / 4;

        float currentSpeed = 4 * baseSpeedFactor;
        float minSpeed = baseSpeedFactor / 6;

        Vector3 targetDirection;

        isFollowing = true;

        for (int i = 0; i < CurrentPath.pathPositions.Count - 1; i++)
        {
            while ((transform.position - CurrentPath.pathPositions[i + 1]).sqrMagnitude > 0.001f)
            {

                targetDirection = (CurrentPath.pathPositions[i + 1] - transform.position).normalized;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), 1080 * Time.deltaTime);
                currentSpeed = Mathf.Max(minSpeed, currentSpeed * Mathf.Exp(-Time.deltaTime));
                transform.position = Vector3.MoveTowards(transform.position, CurrentPath.pathPositions[i + 1], currentSpeed * Time.deltaTime);
                yield return null;
            }
        }
        StopFollow();
    }
}
