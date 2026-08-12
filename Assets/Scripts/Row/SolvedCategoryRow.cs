using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SolvedCategoryRow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text wordsText;

    [Header("Polish")]
    [SerializeField] private ScaleEffect scaleEffect;

    public void SolveCategoryRow(Category category, IReadOnlyList<Tile> tiles)
    {
        background.color = ColorPicker.GetCategoryColor(category);
        
        categoryText.text = tiles[0].CategoryName;
        Debug.Log(tiles[0].CategoryName);

        wordsText.text = string.Join(
            ", ",
            tiles.Select(tile => tile.Word)
        );

        canvasGroup.alpha = 1f;

        scaleEffect.ScaleUpThenDown();
    }

    public void Reset()
    {
        canvasGroup.alpha = 0f;
    }
}
