using UnityEngine;

public class BallTriggerHandler : MonoBehaviour
{
    #region Components
    private SphereCollider ballCollider;
    #endregion

    #region Variables
    [SerializeField] private float movingSize = 0.35f, stopSize = 0.75f;
    #endregion

    //ball should be selected easily while player try to hit. And the collider should be
    //a little bit smaller to avoid holes
    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnStartFollowPath, SetColliderSmaller);
        EventManager.StartListening(EventKeys.OnFinishFollowPath, SetColliderBigger);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnStartFollowPath, SetColliderSmaller);
        EventManager.StopListening(EventKeys.OnFinishFollowPath, SetColliderBigger);
    }

    private void Awake()
    {
        ballCollider = GetComponent<SphereCollider>();
    }

    private void SetColliderBigger(object[] obj = null)
    {
        ballCollider.radius = stopSize;
    }

    private void SetColliderSmaller(object[] obj = null)
    {
        ballCollider.radius = movingSize;
    }

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
