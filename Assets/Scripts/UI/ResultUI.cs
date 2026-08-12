using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI resultText;

    void Start()
    {
        GameManager.Instance.OnGameLost += HandleLoss;
        GameManager.Instance.OnGameWon += HandleWin;

        SetPanelEnable(false);
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameLost -= HandleLoss;
        GameManager.Instance.OnGameWon -= HandleWin;
    }

    private void HandleWin()
    {
        resultText.text = "You Win!";
    }

    private void HandleLoss()
    {
        resultText.text = "You Lose!";
    }

    public void SetPanelEnable(bool enable)
    {
        canvasGroup.alpha = enable ? 1f : 0f;
        canvasGroup.blocksRaycasts = enable;
        canvasGroup.interactable = enable;

        // Stops animations => Reveal catgory when you lose
        Time.timeScale = enable ? 0f : 1f;
    }
}
