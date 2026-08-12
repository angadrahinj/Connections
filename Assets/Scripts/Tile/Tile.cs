using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.ComponentModel;

public class Tile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI wordLabel;
    [SerializeField] private Image background;
    [SerializeField] private TileAnimator tileAnimator;

    [Header("Color")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private float transitionDuration = 0.2f;

    public string Word { get; private set; }
    public Category Category { get; private set; }
    public string CategoryName { get; private set; }
    public RectTransform RectTransform => rectTransform ??= (RectTransform)transform;

    public void Setup(string word, Category difficulty, string categoryName)
    {
        Word = word;
        Category = difficulty;
        CategoryName = categoryName;
 
        SetText(word);
        SetSelected(false);
        SetInteractable(true);
 
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => GameManager.Instance.ToggleSelectTile(this));
    }

    private void SetText(string word)
    {
        wordLabel.text = word;
    }

    public void SetSelected(bool selected)
    {
        Color targetColor = selected ? selectedColor : normalColor;
        background.DOColor(targetColor, transitionDuration);

        wordLabel.color = selected ? Color.white : Color.black;
    }

    public void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    public void ResetTile()
    {
        Word = "";
        Category =  Category.None;
        CategoryName = "";

        SetSelected(false);
        SetInteractable(true);
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    public Tween GetTileUpDownAnimation()
    {
        return tileAnimator.GetTileUpDownAnimation();
    }

    public void TileShakeAnimation()
    {
        tileAnimator.TileShakeAnimation(rectTransform);
    }

    public void TileShuffledAnimation()
    {
        tileAnimator.TextBlinkAnimation();
    }
}
