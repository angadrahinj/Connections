using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A single group of 4 related words.
/// </summary>
[System.Serializable]
public class WordGroup : ISerializationCallbackReceiver
{
    public string categoryName;
    public Category category;
    public string[] words = new string[4];

    // Constructors 
    public WordGroup() { }
 
    public WordGroup(Category difficulty)
    {
        this.category = difficulty;
        this.words = new string[4];
    }

    // Validation
    private void OnValidate() 
    {
        
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize () => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize () {}
}
