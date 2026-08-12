using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A full puzzle: exactly 4 categories of 4 words each (16 words total).
/// Create instances via Assets > Create > Connections > Puzzle.
/// </summary>
[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Connections/Puzzle")]
public class PuzzleSO : ScriptableObject
{
    public WordGroup[] wordGroups;

    private void Reset()
    {
        wordGroups = new WordGroup[]
        {
            new WordGroup(Category.Yellow),
            new WordGroup(Category.Green),
            new WordGroup(Category.Blue),
            new WordGroup(Category.Purple),
        };
    }
 
    /// <summary>Basic validation to catch authoring mistakes in the Editor.</summary>
    private void OnValidate()
    {
        if (wordGroups.Length != 4)
        {
            Debug.LogWarning($"{name}: Connections puzzles must have exactly 4 categories.", this);
        }

        foreach (var category in wordGroups)
        {
            if (category.words == null || category.words.Length != 4)
            {
                Debug.LogWarning($"{name}: category '{category.categoryName}' must have exactly 4 words.", this);
            }
        }

        if (wordGroups.Length == 4)
        {
            int distinctDifficulties = wordGroups
                .Select(c => c.category)
                .Distinct()
                .Count();
                
            if (distinctDifficulties != 4)
            {
                Debug.LogWarning($"{name}: each of the 4 categories should have a distinct Difficulty (Yellow/Green/Blue/Purple).", this);
            }
        }
    }
}
