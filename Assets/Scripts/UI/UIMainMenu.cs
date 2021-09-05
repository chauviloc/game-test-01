using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{

    [SerializeField] private SkeletonAnimation characterMainMenu;
    [SerializeField] private CanvasGroup canvas;



    public void Show()
    {
        characterMainMenu.gameObject.SetActive(true);
        characterMainMenu.skeleton.a = 1;
        canvas.alpha = 1;
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {
            characterMainMenu.skeleton.a = alpha;
            
        });
       
        canvas.DOFade(0, 0.25f).OnComplete(() =>
        {
            
            onComplete?.Invoke();
        });
    }

    public void TapToPlay()
    {
        characterMainMenu.AnimationState.SetAnimation(0, GameConstants.ANIMATION_APPEAR, false).Complete +=
            entry =>
            {
                Hide(() =>
                {
                    characterMainMenu.gameObject.SetActive(false);
                    GameManager.Instance.StartGame();
                });
                
            };
    }

}
