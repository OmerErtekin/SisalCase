using UnityEngine;

public class WhiteBallPlacer : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    #endregion

    #region Variables
    [SerializeField] private GameObject ghostBall;
    private Ball whiteBallReference;
    private bool canPlace = false, isShowingGhost = false;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnEnteredHole, EnableGhostBall);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnEnteredHole, EnableGhostBall);
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (!isShowingGhost) return;
        PlaceTheGhostBall();
    }

    private void Update()
    {
        if (!isShowingGhost) return;
        PlaceTheBall();
    }

    private void EnableGhostBall(object[] obj = null)
    {
        whiteBallReference = (Ball)obj[0];
        isShowingGhost = true;
    }

    private void PlaceTheBall()
    {
        if (!Input.GetMouseButtonUp(0) || !canPlace) return;

        canPlace = false;
        isShowingGhost = false;
        ghostBall.SetActive(false);
        EventManager.TriggerEvent(EventKeys.OnWhiteBallReplaced, new object[] { whiteBallReference, ghostBall.transform.position });
    }

    private void PlaceTheGhostBall()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag(Core.Constants.TAG_TABLE))
        {
            canPlace = true;
            ghostBall.transform.position = new Vector3(hit.point.x, 0.075f, hit.point.z);
        }
        else
        {
            canPlace = false;
        }
        ghostBall.SetActive(canPlace);
    }
}
