using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class TileAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Ease movementEase = Ease.OutQuad;
    [SerializeField] private float movementDuration = 0.2f;
    [SerializeField] private float upwardsOffset = 30f;

    [Header("Shaking Animation")]
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeStrength = 12f;

    [Header("Text Animation")]
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private float textAnimationDuration = 1f;

    private Tween animationTween;

    public Tween GetTileUpDownAnimation()
    {
        animationTween?.Kill();

        Vector3 startPosition = transform.localPosition;

        return animationTween = transform
            .DOLocalMoveY(startPosition.y + upwardsOffset, movementDuration)
            .SetEase(movementEase)
            .OnComplete(() =>
            {
                animationTween = transform
                    .DOLocalMoveY(startPosition.y, movementDuration)
                    .SetEase(movementEase);
            });
    }

    public void TileShakeAnimation(RectTransform rectTransform)
    {
        animationTween?.Kill();

        animationTween = rectTransform.DOPunchAnchorPos(
            punch: new Vector2(shakeStrength, 0f),
            duration: shakeDuration,
            vibrato: (int)shakeStrength,
            elasticity: 1f
        );
    }

    public void TextBlinkAnimation()
    {
        textCanvasGroup.DOFade(0f, textAnimationDuration / 2).OnComplete(() =>
        {
            textCanvasGroup.DOFade(1f, textAnimationDuration / 2);
        });
    }
}
