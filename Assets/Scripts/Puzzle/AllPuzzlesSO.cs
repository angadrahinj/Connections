using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Connections/All Puzzles")]
public class AllPuzzlesSO : ScriptableObject
{
    [SerializeField] private List<PuzzleSO> listOfPuzzles;

    public List<PuzzleSO> ListOfPuzzles => listOfPuzzles;

    public PuzzleSO GetRandomPuzzle()
    {
        return listOfPuzzles.ChooseRandom();
    }
}
