using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BoardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SolvedCategoryGridParent solvedCategoryGridParent;

    [Header("Buttons")]
    [SerializeField] private Button shuffleButton;
    [SerializeField] private Button deselectAllButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button viewResultsButton;

    [Header("Solving Animation")]
    [SerializeField] private float initialDelayBeforeSolve = 1f;
    [SerializeField] private float delayBetweenCategorySolves = 0.75f;

    [Header("Tile Animation")]
    [SerializeField] private float animationDelayBetweenTiles = 0.01f;
    [SerializeField] private float waitDelayBeforeAnimation = 0.4f;

    [Header("Reflow Animation")]
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private float tileReorderDuration = 0.4f;
    [SerializeField] private AnimationCurve tileReorderEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isAnimatingTileReorder;

    private void OnEnable()
    {
        gameManager.OnBoardReset += GameManager_OnBoardReset;
        gameManager.OnBoardShuffled += GameManager_OnBoardShuffled;
        gameManager.OnTileSelectionChanged += GameManager_OnTileSelectionChanged;
        gameManager.OnCategorySolved += GameManager_OnCategorySolved;
        gameManager.OnIncorrectGuess += GameManager_OnIncorrectGuess;

        gameManager.OnGameWon += GameManager_OnGameWon;
        gameManager.OnGameLost += GameManager_OnGameLost;
    }

    private void OnDisable()
    {
        gameManager.OnBoardReset -= GameManager_OnBoardReset;
        gameManager.OnBoardShuffled -= GameManager_OnBoardShuffled;
        gameManager.OnTileSelectionChanged -= GameManager_OnTileSelectionChanged;
        gameManager.OnCategorySolved -= GameManager_OnCategorySolved;
        gameManager.OnIncorrectGuess -= GameManager_OnIncorrectGuess;
    }

    private void GameManager_OnGameLost()
    {
        SetViewResultsButton(true);

        StartCoroutine(RevealRemainingCategoriesOnLoss());
    }

    private void GameManager_OnGameWon()
    {
        SetViewResultsButton(true);
    }

    public void SetViewResultsButton(bool gameOver)
    {
        viewResultsButton.gameObject.SetActive(gameOver);

        submitButton.gameObject.SetActive(!gameOver);
        shuffleButton.gameObject.SetActive(!gameOver);
        deselectAllButton.gameObject.SetActive(!gameOver);
    }

    private void GameManager_OnBoardReset()
    {
        IReadOnlyList<Tile> tiles = gameManager.TileReferences;

        foreach (Tile tile in tiles)
        {
            tile.SetVisible(true);

            // Resets its selected state, word, category, category name
            tile.ResetTile();
        }

        SetViewResultsButton(false);
    }

    private void GameManager_OnBoardShuffled(bool updateTiles)
    {
        IReadOnlyList<Tile> tiles = gameManager.TileOrder;
        HashSet<Category> solvedCategories = gameManager.GetSolvedCategories();

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].transform.SetSiblingIndex(i);

            if (updateTiles && !solvedCategories.Contains(tiles[i].Category))
            {
                tiles[i].TileShuffledAnimation();
            }
        }
    }

    private void GameManager_OnTileSelectionChanged(Tile tile, bool selected)
    {
        tile.SetSelected(selected);

        StartCoroutine(CheckDeselectAllButtonInteractable());
        StartCoroutine(CheckSubmitButtonInteractable());
    }

    private void GameManager_OnIncorrectGuess(IReadOnlyList<Tile> guessedTiles)
    {
        submitButton.interactable = false;
        TileUpDownAnimationSequence(guessedTiles, false, () =>
            {
                gameManager.HandleTileAnimationDone(false);
            },
            () =>
            {
                submitButton.interactable = true;
            }
        );
    }

    private void GameManager_OnCategorySolved(IReadOnlyList<Tile> guessedTiles)
    {
        TileUpDownAnimationSequence(guessedTiles, true, () =>
        {
            gameManager.HandleTileAnimationDone(true);
        });
    }

    public void TileUpDownAnimationSequence(IReadOnlyList<Tile> guessedTiles, bool guessedCorrect, Action onComplete, Action onFullyComplete = null)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (Tile tile in guessedTiles)
        {
            Tween tween = tile.GetTileUpDownAnimation();
            sequence.Append(tween);
            sequence.AppendInterval(animationDelayBetweenTiles);
        }

        // Wait a bit before playing correct or incorrect animation
        sequence.AppendInterval(waitDelayBeforeAnimation);

        sequence.Play().OnComplete(() =>
        {
            onComplete.Invoke();

            if (guessedCorrect)
            {
                int solvedCategoriesCount = gameManager.SolvedCategoriesCount;


                StartCoroutine(AnimateCategoryTilePositions(() =>
                {
                    SolveCategoryRow(guessedTiles, solvedCategoriesCount);
                    onFullyComplete?.Invoke();
                }));

            }
            else
            {
                AnimateGuessWrongTileShakeAnimation(guessedTiles);
                onFullyComplete?.Invoke();
            }
        });
    }

    private void SolveCategoryRow(IReadOnlyList<Tile> guessedTiles, int solvedCategoriesCount)
    {
        foreach (Tile tile in guessedTiles)
        {
            tile.SetVisible(false);
        }
        Category solvedCategory = guessedTiles[0].Category;
        solvedCategoryGridParent.SolveCategoryRow(solvedCategory, guessedTiles, solvedCategoriesCount);

        gameManager.HandleCategorySolved();
    }

    private IEnumerator RevealRemainingCategoriesOnLoss()
    {
        yield return new WaitForSeconds(initialDelayBeforeSolve);

        List<List<Tile>> unsolvedGroups = gameManager.GetUnsolvedCategoryGroups();

        foreach (List<Tile> group in unsolvedGroups)
        {
            gameManager.RevealCategoryForLoss(group);

            bool categoryRevealDone = false;

            StartCoroutine(AnimateCategoryTilePositions(() =>
            {
                SolveCategoryRow(group, gameManager.SolvedCategoriesCount);

                DOVirtual.DelayedCall(delayBetweenCategorySolves, () =>
                {
                    categoryRevealDone = true;

                });
            }));

            yield return new WaitUntil(() => categoryRevealDone);
        }

        SetViewResultsButton(true);
    }

    ///<summary>
    /// Animating the solved category tiles
    /// </summary>
    private IEnumerator AnimateCategoryTilePositions(Action onComplete)
    {
        IReadOnlyList<Tile> tiles = gameManager.TileOrder;

        // Before everything moves
        var before = new Dictionary<Tile, Vector2>();
        foreach (Tile tile in tiles)
        {
            before[tile] = tile.RectTransform.anchoredPosition;
        }

        // Apply the order already determined by GameManager. Note that this has already been called before this function.
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].transform.SetSiblingIndex(i);
        }
        // Rebuild it and decide the order right now
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridContainer);

        // Note the after UI rebuild positions, but only keep tiles that actually moved
        var after = new Dictionary<Tile, Vector2>();
        var movingTiles = new List<Tile>();
        foreach (Tile tile in tiles)
        {
            Vector2 newPos = tile.RectTransform.anchoredPosition;

            if (newPos != before[tile])
            {
                after[tile] = newPos;
                movingTiles.Add(tile);
            }
        }

        // Nothing moved - skip the animation entirely
        if (movingTiles.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        isAnimatingTileReorder = true;
        gridLayoutGroup.enabled = false;

        // Resetting tiles to their initial positions
        foreach (Tile tile in movingTiles)
        {
            tile.RectTransform.anchoredPosition = before[tile];
        }

        Sequence reorderSequence = DOTween.Sequence();
        foreach (Tile tile in movingTiles)
        {
            reorderSequence.Insert(
                0,
                tile.RectTransform
                    .DOAnchorPos(after[tile], tileReorderDuration)
                    .SetEase(tileReorderEase)
            );
        }

        yield return reorderSequence.WaitForCompletion();

        foreach (Tile tile in movingTiles)
        {
            tile.RectTransform.anchoredPosition = after[tile];
        }

        gridLayoutGroup.enabled = true;
        isAnimatingTileReorder = false;

        onComplete?.Invoke();
    }

    private void AnimateGuessWrongTileShakeAnimation(IReadOnlyList<Tile> guessedTiles)
    {
        foreach (Tile tile in guessedTiles)
        {
            tile.TileShakeAnimation();
        }
    }

    private IEnumerator CheckDeselectAllButtonInteractable()
    {
        yield return null;

        if (gameManager.SelectedTilesCount >= 1)
        {
            deselectAllButton.interactable = true;
        }
        else
        {
            deselectAllButton.interactable = false;
        }
    }

    private IEnumerator CheckSubmitButtonInteractable()
    {
        yield return null;

        if (gameManager.SelectedTilesCount == 4)
        {
            submitButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
        }
    }
}