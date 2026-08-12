using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMB<GameManager>
{
    [Header("Puzzle")]
    [SerializeField] private AllPuzzlesSO puzzleList;
    [SerializeField] private PuzzleSO currentPuzzle; // made it public for testing/ debug purposes to look at in the editor

    [Header("Board")]
    [SerializeField] private Tile[] tileReferences;

    [Header("Rules")]
    [SerializeField] private int maxSelections = 4;
    [SerializeField] private int maxMistakes = 4;

    private List<Tile> tileOrder = new(); // actual order the tiles should be ordered in
    private readonly List<Tile> selectedTiles = new();
    private readonly HashSet<Category> solvedCategories = new();
    private readonly List<List<Tile>> guessedTilesList = new();

    private int mistakesMade;
    private bool oneAwayGuess;

    public IReadOnlyList<Tile> TileOrder => tileOrder;
    public IReadOnlyList<Tile> TileReferences => tileReferences;
    public IReadOnlyList<Tile> SelectedTiles => selectedTiles;
    public List<List<Tile>> GuessedTilesList => guessedTilesList;
    public int SelectedTilesCount => SelectedTiles.Count;
    public int SolvedCategoriesCount => solvedCategories.Count;
    public int MistakesMade => mistakesMade;
    public int RemainingMistakes => maxMistakes - mistakesMade;
    public bool IsGameOver { get; private set; }

    public event Action OnBoardReset;
    public event Action<bool> OnBoardShuffled;
    public event Action<Tile, bool> OnTileSelectionChanged;
    public event Action<IReadOnlyList<Tile>> OnCategorySolved;
    public event Action<IReadOnlyList<Tile>> OnIncorrectGuess;
    public event Action OnOneAwayGuess;
    public event Action OnAlreadyGuessed;
    public event Action<int> OnMistakesChanged;
    public event Action OnGameWon;
    public event Action OnGameLost;

    void Start()
    {
        StartNewPuzzle();
    }

    public void StartNewPuzzle()
    {
        currentPuzzle = puzzleList.GetRandomPuzzle();

        ResetBoard();
        SetupBoard();
        ShuffleBoard(false);

        SetMistakesMade(0);
        IsGameOver = false;
    }

    private void ResetBoard()
    {
        OnBoardReset?.Invoke();

        DeselectAllTiles();
        solvedCategories.Clear();
        tileOrder.Clear();
        guessedTilesList.Clear();

        foreach (Tile tile in tileReferences)
        {
            // Defensive programming just in case 
            tile.SetSelected(false);
            tile.SetInteractable(true);
        }
    }

    private void SetupBoard()
    {
        int tileIndex = 0;

        foreach (WordGroup wordGroup in currentPuzzle.wordGroups)
        {
            foreach (string word in wordGroup.words)
            {
                Tile tile = tileReferences[tileIndex];

                tile.Setup(word, wordGroup.category, wordGroup.categoryName);
                tileOrder.Add(tile);

                tileIndex++;
            }
        }

        if (tileIndex != tileReferences.Length)
        {
            Debug.LogError(
                $"Puzzle has {tileIndex} words, " +
                $"but board has {tileReferences.Length} tiles."
            );
        }
    }

    public void ShuffleBoard(bool updateTiles)
    {
        List<int> movableIndices = new();

        // Finding unsolved tiles so only they get shuffled.
        for (int i = 0; i < tileOrder.Count; i++)
        {
            if (!solvedCategories.Contains(tileOrder[i].Category))
            {
                movableIndices.Add(i);
            }
        }
        
        for (int i = movableIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            int indexA = movableIndices[i];
            int indexB = movableIndices[randomIndex];

            (tileOrder[indexA], tileOrder[indexB]) = (tileOrder[indexB], tileOrder[indexA]);
        }

        OnBoardShuffled?.Invoke(updateTiles);
    }

    private void MarkCategorySolved(Category category)
    {
        solvedCategories.Add(category);
        MoveSolvedCategoryToFront();
    }

    private void MoveSolvedCategoryToFront()
    {
        List<Tile> newOrder = new();

        // Add solved categories in solve order.
        foreach (Category solvedCategory in solvedCategories)
        {
            foreach (Tile tile in tileOrder)
            {
                if (tile.Category == solvedCategory)
                {
                    newOrder.Add(tile);
                }
            }
        }

        // Keep all unsolved tiles after the solved tiles.
        // Their current relative order is preserved.
        foreach (Tile tile in tileOrder)
        {
            if (!solvedCategories.Contains(tile.Category))
            {
                newOrder.Add(tile);
            }
        }

        tileOrder = newOrder;
    }

    public void RevealCategoryForLoss(IReadOnlyList<Tile> tiles)
    {
        Category category = tiles[0].Category;

        if (solvedCategories.Contains(category))
            return;

        MarkCategorySolved(category);
    }

    public void SetMistakesMade(int mistakesMade)
    {
        this.mistakesMade = mistakesMade;
        OnMistakesChanged?.Invoke(RemainingMistakes);
    }

    public void MadeMistake()
    {
        mistakesMade++;
        OnMistakesChanged?.Invoke(RemainingMistakes);

        if (mistakesMade >= maxMistakes)
        {
            IsGameOver = true;
            OnGameLost?.Invoke();
            DeselectAllTiles();
        }
    }

    public void ToggleSelectTile(Tile tile)
    {
        if (tile == null || IsGameOver)
            return;

        if (selectedTiles.Contains(tile))
        {
            selectedTiles.Remove(tile);

            OnTileSelectionChanged?.Invoke(tile, false);
            return;
        }

        if (selectedTiles.Count >= maxSelections)
            return;

        selectedTiles.Add(tile);

        OnTileSelectionChanged?.Invoke(tile, true);
    }

    public void DeselectAllTiles()
    {
        foreach (Tile tile in selectedTiles)
        {
            OnTileSelectionChanged?.Invoke(tile, false);
        }

        selectedTiles.Clear();
    }

    public void SubmitGuessedTiles()
    {
        if (selectedTiles.Count != maxSelections)
            return;

        if (CheckForAlreadyGuessed())
        {
            OnAlreadyGuessed?.Invoke();
            return;
        }

        // Take a snapshot because the selection will be cleared later.
        List<Tile> guessedTiles = new(selectedTiles);
        guessedTilesList.Add(guessedTiles);

        foreach (Tile tile in guessedTiles)
        {
            tile.SetInteractable(false);
        }
        
        bool correct = IsGuessedCorrect(out bool oneAway);

        if (correct)
        {
            Category category = guessedTiles[0].Category;

            MarkCategorySolved(category);

            OnCategorySolved?.Invoke(guessedTiles);
        }
        else
        {
            // Mistake handling is in HandleTileAnimationDone => after animation of tile up and down (suspence)

            if (oneAway)
            {
                // Mark for later
                oneAwayGuess = true;
            }
            else if (mistakesMade == maxMistakes - 1)
            {
                foreach(Tile tile in tileReferences)
                {
                    tile.SetInteractable(false);
                }
            }

            OnIncorrectGuess?.Invoke(guessedTiles);
        }
    }

    private bool CheckForAlreadyGuessed()
    {
        foreach (List<Tile> guessedTileList in guessedTilesList)
        {
            bool allMatch = true;

            foreach (Tile tile in selectedTiles)
            {
                if (!guessedTileList.Contains(tile))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                return true;
            }
        }
        return false;
    }

    public void HandleTileAnimationDone(bool guessedCorrect)
    {
        if (!guessedCorrect)
        {
            MadeMistake();

            if (oneAwayGuess)
            {
                OnOneAwayGuess?.Invoke();
                oneAwayGuess = false;
            }

            if (!IsGameOver)
            {
                foreach(Tile tile in selectedTiles)
                {
                    tile.SetInteractable(true);
                }   
            }
        }
        else
        {
            DeselectAllTiles();
        }
    }

    public void HandleCategorySolved()
    {
        if (IsGameOver)
            return;

        if (SolvedCategoriesCount == 4)
        {
            OnGameWon?.Invoke();
        }
    }

    private bool IsGuessedCorrect(out bool oneAway)
    {
        Dictionary<Category, int> categoryCounts = new();

        foreach (Tile tile in selectedTiles)
        {
            categoryCounts.TryGetValue(tile.Category, out int count);
            categoryCounts[tile.Category] = count + 1;
        }

        int largestGroup = 0;

        foreach (int count in categoryCounts.Values)
        {
            if (count > largestGroup)
            {
                largestGroup = count;
            }
        }

        oneAway = largestGroup == selectedTiles.Count - 1;

        return largestGroup == selectedTiles.Count;
    }

    public HashSet<Category> GetSolvedCategories()
    {
        return solvedCategories;
    }

    public List<List<Tile>> GetUnsolvedCategoryGroups()
    {
        List<List<Tile>> groups = new();

        foreach (WordGroup wordGroup in currentPuzzle.wordGroups)
        {
            if (solvedCategories.Contains(wordGroup.category))
                continue;

            List<Tile> tiles = tileOrder.FindAll(tile => tile.Category == wordGroup.category);

            if (tiles.Count > 0)
                groups.Add(tiles);
        }

        return groups;
    }
}