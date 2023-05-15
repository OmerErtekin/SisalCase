using UnityEngine;

public class BallTriggerHandler : MonoBehaviour
{
    #region Components
    [SerializeField] private AudioSource wallHitSound;
    #endregion

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Core.Constants.TAG_WALL))
        {
            wallHitSound.Play();
        }
    }
}
