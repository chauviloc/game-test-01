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
        float alpha = 0;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;

        }).OnComplete(() => { canvas.blocksRaycasts = true; });
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;
        }).OnComplete(() =>
        {
            canvas.blocksRaycasts = false;
            onComplete?.Invoke();
        });

    }

   

    public void OnResumePress()
    {
        SoundManager.Instance.PlayButtonClickSFX();
        Hide(() =>
        {
            GameManager.Instance.UnPauseGame();
        });
    }

    public void OnResetPress()
    {
        SoundManager.Instance.PlayButtonClickSFX();
        GameManager.Instance.UnPauseGame();
        GameManager.Instance.ResetGame();
        Hide();
    }

    public void OnHomePress()
    {
        SoundManager.Instance.PlayButtonClickSFX();
        GameManager.Instance.UnPauseGame();
        GameManager.Instance.GoHome();
        Hide();
    }

}
