using UnityEngine;
using UnityEngine.UI;

public class GuessedTile : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetTileColor(Color color)
    {
        image.color = color;
    }
}
