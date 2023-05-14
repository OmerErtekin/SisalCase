
public static class Core
{
    public static class Constants
    {
        public const string TAG_BALL = "Ball";
        public const string TAG_HOLE = "Hole";
        public const string TAG_TABLE = "Table";
    }
}


public enum EventKeys
{
    OnWhiteBallReplaced,
    OnPathCalculateRequested,
    OnPathCalculateCompleted,
    OnStartFollowPath,
    OnFinishFollowPath,
    OnEnteredHole,
    OnCollidedWithBall,
    OnStickReleased,
}