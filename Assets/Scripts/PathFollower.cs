using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    private Coroutine followRoutine;
    private Path currentPath;
    #endregion
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FollowPath(Path pathToFollow)
    {
        StopFollow();
        currentPath = pathToFollow;
        followRoutine = StartCoroutine(FollowRoutine());
    }

    private void StopFollow()
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

        for (int i = 0; i < currentPath.pathPoints.Count - 1; i++)
        {
            while ((transform.position - currentPath.pathPoints[i + 1]).sqrMagnitude > 0.001f)
            {

                targetDirection = (currentPath.pathPoints[i + 1] - transform.position).normalized;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), 1080 * Time.deltaTime);
                currentSpeed = Mathf.Max(minSpeed, currentSpeed * Mathf.Exp(-Time.deltaTime));
                transform.position = Vector3.MoveTowards(transform.position, currentPath.pathPoints[i + 1], currentSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
