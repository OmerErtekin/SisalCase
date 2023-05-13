using UnityEngine;

public class WhiteBall : MonoBehaviour
{
    #region Components
    #endregion

    #region Variables
    public float distance = 10;
    #endregion
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 45 * Time.deltaTime, 0);
        EventManager.TriggerEvent(EventKeys.OnPathCalculateRequested,new object[] {transform.position,transform.forward,distance});
    }
}
