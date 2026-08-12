using System;
using UnityEngine.UI;
using UnityEngine;

public class MistakesUI : MonoBehaviour
{
    [SerializeField] private Image[] mistakeImages;

    void Start()
    {
        GameManager.Instance.OnMistakesChanged += GameManager_OnMistakesMadeChanged;
    }

    void OnDisable()
    {
        GameManager.Instance.OnMistakesChanged -= GameManager_OnMistakesMadeChanged;
    }

    private void GameManager_OnMistakesMadeChanged(int remainingMistakes)
    {
        for (int i = 0; i < mistakeImages.Length; i++)
        {
            mistakeImages[i].enabled = (i < remainingMistakes);
        }
    }
}
