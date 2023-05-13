using System.Collections;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    private Coroutine followRoutine;
    private Path currentPath;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnPathCalculateCompleted, GetCalculatedPath);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnPathCalculateCompleted, GetCalculatedPath);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void GetCalculatedPath(object[] obj = null)
    {
        currentPath = (Path)obj[0];
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
    }

    private IEnumerator FollowRoutine()
    {
        float baseSpeedFactor = currentPath.pathDistance / 4;

        float currentSpeed = 4 * baseSpeedFactor;
        float minSpeed = baseSpeedFactor / 6;

        Vector3 targetDirection;

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
    }
}
