using UnityEngine;


public static class Core
{
    public static class Constants
    {
        public const string TAG_BALL = "Ball";
        public const string TAG_HOLE = "Hole";
    }
}


public enum EventKeys
{
    OnWhiteBallClicked,
    OnPathCalculateRequested,
    OnPathCalculateCompleted,
    OnStartFollowPath,
    OnFinishFollowPath,
    OnEnteredHole,
    OnCollidedWithBall,
    OnStickReleased,
}