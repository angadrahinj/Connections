using UnityEngine;
using DG.Tweening;
using System;

public class ScaleEffect : MonoBehaviour
{
    public float scaleUpFactor = 1.2f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutBack;

    public Vector3 originalScale;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void ScaleUp()
    {
        PlayScale(originalScale * scaleUpFactor);
    }

    public void ScaleDown()
    {
        PlayScale(originalScale);
    }

    public void ScaleUpThenDown()
    {
        currentTween?.Kill();

        currentTween = DOTween.Sequence()
            .Append(transform.DOScale(originalScale * scaleUpFactor, duration)
                .SetEase(ease))
            .Append(transform.DOScale(originalScale, duration)
                .SetEase(ease));
    }

    private void PlayScale(Vector3 targetScale)
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(targetScale, duration).SetEase(ease);
    }
}