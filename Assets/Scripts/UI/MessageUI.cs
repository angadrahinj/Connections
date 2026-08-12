using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.5f;

    void Start()
    {
        GameManager.Instance.OnGameLost += HandleLoss;
        GameManager.Instance.OnGameWon += HandleWin;
        GameManager.Instance.OnOneAwayGuess += HandleOneAway;
        GameManager.Instance.OnAlreadyGuessed += HandleAlreadyGuessed;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameLost -= HandleLoss;
        GameManager.Instance.OnGameWon -= HandleWin;
        GameManager.Instance.OnOneAwayGuess -= HandleOneAway;
        GameManager.Instance.OnAlreadyGuessed -= HandleAlreadyGuessed;
    }

    private void HandleWin()
    {
        AnimateTextPopup("Great!");
    }

    private void HandleLoss()
    {
        AnimateTextPopup("Nice try!");
    }

    private void HandleOneAway()
    {
        AnimateTextPopup("One away!");
    }

    private void HandleAlreadyGuessed()
    {
        AnimateTextPopup("Already guessed!");
    }

    private void AnimateTextPopup(string message)
    {
        messageText.text = message;

        textCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            textCanvasGroup.DOFade(0f, fadeDuration).SetDelay(displayDuration);
        });
    }
}
