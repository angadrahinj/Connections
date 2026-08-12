using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuessDisplayUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> guessedTileRowGOs;
    [SerializeField] private GameObject guessedTileRowPrefab;
    [SerializeField] private RectTransform parentRect;

    void Start()
    {
        GameManager.Instance.OnGameLost += UpdateDisplayUI;
        GameManager.Instance.OnGameWon += UpdateDisplayUI;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameLost -= UpdateDisplayUI;
        GameManager.Instance.OnGameWon -= UpdateDisplayUI;
    }

    private void UpdateDisplayUI()
    {
        Reset();
        DisplayGuessedTiles();
    }

    public void Reset()
    {
        foreach (GameObject guessedTileRow in guessedTileRowGOs)
        {
            Destroy(guessedTileRow);
        }
        guessedTileRowGOs.Clear();
    }

    public void DisplayGuessedTiles()
    {
        List<List<Tile>> guessedTilesList = GameManager.Instance.GuessedTilesList;

        foreach (List<Tile> guessedTiles in guessedTilesList)
        {
            GameObject guessedTileRow = Instantiate(guessedTileRowPrefab, transform);
            guessedTileRow.GetComponent<GuessedTileRow>().SetGuessedTiles(guessedTiles);

            guessedTileRowGOs.Add(guessedTileRow);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }
}
