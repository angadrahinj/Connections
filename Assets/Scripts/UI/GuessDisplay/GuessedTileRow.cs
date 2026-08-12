using UnityEngine;
using System.Collections.Generic;

public class GuessedTileRow : MonoBehaviour
{
    [SerializeField] private GuessedTile[] guessedTiles;

    public void SetGuessedTiles(IReadOnlyList<Tile> tiles)
    {
        for (int i = 0; i < guessedTiles.Length; i++)
        {
            guessedTiles[i].SetTileColor(ColorPicker.GetCategoryColor(tiles[i].Category));
        }
    }
}
