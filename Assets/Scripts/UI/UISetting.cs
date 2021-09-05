using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UISetting : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;



    public void Show()
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.25f).OnUpdate(() => { canvas.alpha = alpha; });
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;
        }).OnComplete(() =>
        {
            onComplete?.Invoke();
        });

    }

    public void OnClosePress()
    {
        Hide(() =>
        {
            GameManager.Instance.UnPauseGame();
        });
    }

}
