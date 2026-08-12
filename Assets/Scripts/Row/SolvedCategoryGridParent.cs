using System.Collections.Generic;
using UnityEngine;
public class SolvedCategoryGridParent : MonoBehaviour
{
    [SerializeField] private SolvedCategoryRow[] solvedCategoryRows;

    void Start()
    {
        GameManager.Instance.OnBoardReset += ResetSolvedCategoryRows;
    }

    void OnDisable()
    {
        GameManager.Instance.OnBoardReset -= ResetSolvedCategoryRows;
    }

    public void ResetSolvedCategoryRows()
    {
        foreach(SolvedCategoryRow solvedRow in solvedCategoryRows)
        {
            solvedRow.Reset();
        }
    }

    public void SolveCategoryRow(Category category, IReadOnlyList<Tile> tiles, int numberOfCategoriesSolved)
    {
        solvedCategoryRows[numberOfCategoriesSolved - 1].SolveCategoryRow(category, tiles);
    }
}
