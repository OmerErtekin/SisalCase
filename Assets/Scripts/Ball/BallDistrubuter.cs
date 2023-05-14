using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDistrubuter : MonoBehaviour
{
    #region Components

    #endregion

    #region Variables
    [SerializeField] private float xRange, zRange;
    [SerializeField] private float minRequiredDistance = 0.75f;
    [SerializeField] private List<Ball> balls = new();
    private List<Ball> disabledBalls = new();
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnCollidedWithBall, AddBallToDisabled);
        EventManager.StartListening(EventKeys.OnFinishFollowPath, SetDisabledBalls);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnCollidedWithBall, AddBallToDisabled);
        EventManager.StopListening(EventKeys.OnFinishFollowPath, SetDisabledBalls);
    }

    private void Start()
    {
        DistrubuteBallsAtStart();
    }

    private void DistrubuteBallsAtStart()
    {
        for (int i = 1; i < balls.Count; i++)
        {
            StartCoroutine(SetBallToRandomPosition(balls[i]));
        }
    }

    private void AddBallToDisabled(object[] obj = null)
    {
        disabledBalls.Add((Ball)obj[0]);
    }

    private void SetDisabledBalls(object[] obj = null)
    {
        for (int i = 0; i < disabledBalls.Count; i++)
        {
            StartCoroutine(SetBallToRandomPosition(disabledBalls[i]));
        }
        disabledBalls.Clear();
    }

    private IEnumerator SetBallToRandomPosition(Ball ball)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-xRange / 2, xRange / 2), 0.075f, Random.Range(-zRange / 2, zRange / 2));

        while (IsThereAnyCloseBall(randomPosition, ball))
        {
            randomPosition = new Vector3(Random.Range(-xRange / 2, xRange / 2), 0.075f, Random.Range(-zRange / 2, zRange / 2));
            yield return null;
        }
        ball.EnableBall(randomPosition);
    }


    private bool IsThereAnyCloseBall(Vector3 position, Ball ball)
    {
        for (int i = 1; i < balls.Count; i++)
        {
            if (balls[i] == ball) continue;

            if ((position - balls[i].transform.position).sqrMagnitude <= minRequiredDistance * minRequiredDistance)
                return true;
        }

        return false;
    }
}
