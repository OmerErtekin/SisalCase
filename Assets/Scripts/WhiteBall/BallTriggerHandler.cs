using UnityEngine;

public class BallTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(Core.Constants.TAG_BALL))
        {
            EventManager.TriggerEvent(EventKeys.OnCollidedWithBall, new object[] { other.gameObject });
        }

        if(other.gameObject.CompareTag(Core.Constants.TAG_HOLE))
        {
            EventManager.TriggerEvent(EventKeys.OnEnteredHole);
        }
    }
}
