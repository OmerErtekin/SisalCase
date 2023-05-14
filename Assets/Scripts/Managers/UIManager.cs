using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Components
    [SerializeField] private TMP_Text highScoreText, currentScoreText, roundScoreText, replaceText;
    #endregion

    #region Variables
    private int roundScore, currentScore, currentHighScore;
    private bool isEnteredHole = false;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnCollidedWithBall, SetRoundScore);
        EventManager.StartListening(EventKeys.OnEnteredHole, DisableRoundScore);
        EventManager.StartListening(EventKeys.OnEnteredHole, ShowReplaceHint);
        EventManager.StartListening(EventKeys.OnWhiteBallReplaced, HideReplaceHint);
        EventManager.StartListening(EventKeys.OnFinishFollowPath, AddRoundScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnCollidedWithBall, SetRoundScore);
        EventManager.StopListening(EventKeys.OnEnteredHole, DisableRoundScore);
        EventManager.StopListening(EventKeys.OnFinishFollowPath, AddRoundScore);
        EventManager.StartListening(EventKeys.OnEnteredHole, ShowReplaceHint);
        EventManager.StartListening(EventKeys.OnWhiteBallReplaced, HideReplaceHint);
    }

    private void Start()
    {
        SetTextScoresAtStart();
    }

    private void SetTextScoresAtStart()
    {
        roundScore = 0;
        currentScore = 0;
        roundScoreText.gameObject.SetActive(false);
        currentScoreText.text = "Current Score\n0";
        currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"High Score\n{currentHighScore}";
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
}
