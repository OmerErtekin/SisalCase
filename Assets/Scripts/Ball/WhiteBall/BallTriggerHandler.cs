using UnityEngine;

public class BallTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Core.Constants.TAG_BALL))
        {
            EventManager.TriggerEvent(EventKeys.OnCollidedWithBall, new object[] { other.gameObject.GetComponent<Ball>() });
        }

        if (other.gameObject.CompareTag(Core.Constants.TAG_HOLE))
        {
            EventManager.TriggerEvent(EventKeys.OnEnteredHole, new object[] { GetComponent<Ball>() });
        }
    }
}
