using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Components
    [SerializeField] private TMP_Text highScoreText, currentScoreText, roundScoreText, replaceText, welcomeText;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private CanvasGroup namePanelGroup, ingameGroup, startScreenGroup;
    #endregion

    #region Variables
    private int roundScore, currentScore, currentHighScore;
    private bool isEnteredHole = false;
    private string apiText = "", playerName = "";
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnCollidedWithBall, SetRoundScore);
        EventManager.StartListening(EventKeys.OnEnteredHole, DisableRoundScore);
        EventManager.StartListening(EventKeys.OnEnteredHole, ShowReplaceHint);
        EventManager.StartListening(EventKeys.OnWhiteBallReplaced, HideReplaceHint);
        EventManager.StartListening(EventKeys.OnFinishFollowPath, AddRoundScore);
        EventManager.StartListening(EventKeys.OnGameStarted, SetTextScoresAtStart);
        EventManager.StartListening(EventKeys.OnAPICallCompleted, GetAPIText);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnCollidedWithBall, SetRoundScore);
        EventManager.StopListening(EventKeys.OnEnteredHole, DisableRoundScore);
        EventManager.StopListening(EventKeys.OnFinishFollowPath, AddRoundScore);
        EventManager.StopListening(EventKeys.OnEnteredHole, ShowReplaceHint);
        EventManager.StopListening(EventKeys.OnWhiteBallReplaced, HideReplaceHint);
        EventManager.StopListening(EventKeys.OnGameStarted, SetTextScoresAtStart);
        EventManager.StopListening(EventKeys.OnAPICallCompleted, GetAPIText);
    }

    private void Awake()
    {
        ShowNameInput();
    }

    private void ShowNameInput()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        if (playerName != "")
        {
            namePanelGroup.gameObject.SetActive(false);
            return;
        }
        namePanelGroup.gameObject.SetActive(true);
    }

    private void GetAPIText(object[] obj = null)
    {
        apiText = (string)obj[0];
        ShowWelcomeText();
    }

    private void ShowWelcomeText()
    {
        if (apiText == "" || playerName == "") return;
        welcomeText.gameObject.SetActive(true);
        welcomeText.DOFade(1, 0.5f).SetTarget(this).From(0);
        welcomeText.text = $"Great Time {playerName} to play {apiText}";
    }

    private void SetTextScoresAtStart(object[] obj = null)
    {
        roundScore = 0;
        currentScore = 0;
        roundScoreText.gameObject.SetActive(false);
        currentScoreText.text = "Current Score\n0";
        currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"High Score\n{currentHighScore}";
    }

    private void ShowIngameScreen()
    {
        ingameGroup.DOKill();
        startScreenGroup.DOKill();
        startScreenGroup.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => startScreenGroup.gameObject.SetActive(false));
        ingameGroup.gameObject.SetActive(true);
        ingameGroup.DOFade(1, 0.5f).SetTarget(this);
    }

    private void ShowReplaceHint(object[] obj = null)
    {
        replaceText.DOKill();
        replaceText.gameObject.SetActive(true);
        replaceText.DOFade(1, 0.5f).SetTarget(this).From(0);
    }

    private void HideReplaceHint(object[] obj = null)
    {
        replaceText.DOKill();
        replaceText.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => replaceText.gameObject.SetActive(false));
    }

    private void SetRoundScore(object[] obj = null)
    {
        isEnteredHole = false;
        roundScoreText.DOKill();
        roundScoreText.transform.DOKill();
        if (roundScore == 0)
        {
            roundScoreText.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            roundScoreText.gameObject.SetActive(true);
            roundScoreText.DOFade(1, 0.25f).From(0).SetTarget(this);
        }
        roundScore += 10;
        roundScoreText.transform.DOScale(1, 0.15f).SetTarget(this).From(0.5f);
        roundScoreText.text = $"+{roundScore}";
    }

    private void AddRoundScore(object[] obj = null)
    {
        if (isEnteredHole) return;

        currentScore += roundScore;
        roundScore = 0;
        roundScoreText.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => roundScoreText.gameObject.SetActive(false));
        currentScoreText.DOFade(1, 0.5f).SetTarget(this).From(0);
        currentScoreText.text = $"Current Score\n{currentScore}";

        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", currentHighScore);
            highScoreText.DOFade(1, 0.5f).SetTarget(this).From(0);
            highScoreText.text = $"High Score\n{currentHighScore}";
        }
    }

    private void DisableRoundScore(object[] obj = null)
    {
        isEnteredHole = true;
        roundScore = 0;
        roundScoreText.DOKill();
        roundScoreText.transform.DOKill();
        roundScoreText.transform.DOShakeRotation(0.5f, 30).SetTarget(this);
        roundScoreText.transform.DOMoveY(-50, 0.5f).SetTarget(this);
        roundScoreText.DOFade(0, 0.25f).SetTarget(this).OnComplete(() => roundScoreText.gameObject.SetActive(false));
    }

    public void PlayButtonClicked()
    {
        EventManager.TriggerEvent(EventKeys.OnGameStarted);
        ShowIngameScreen();
    }

    public void SaveButtonClicked()
    {
        if (nameInput.text.Length <= 0) return;

        playerName = nameInput.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        namePanelGroup.DOKill();
        namePanelGroup.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => namePanelGroup.gameObject.SetActive(false));
        ShowWelcomeText();
    }

    public void ResetButtonClicked()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
