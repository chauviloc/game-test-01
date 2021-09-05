using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class UIEndGame : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private SkeletonAnimation defTeamIcon;
    [SerializeField] private SkeletonAnimation atkTeamIcon;
    [SerializeField] private TextMeshProUGUI txtTeamWin;

    private string teamWinFormat = "{0} Team Win";

    public void Show(AxieTeam teamWin)
    {
        txtTeamWin.text = string.Format(teamWinFormat, teamWin);
        defTeamIcon.gameObject.SetActive(teamWin == AxieTeam.Def);
        atkTeamIcon.gameObject.SetActive(teamWin == AxieTeam.Atk);

        float alpha = 0;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;
            defTeamIcon.skeleton.a = alpha;
            atkTeamIcon.skeleton.a = alpha;
        }).OnComplete(() =>
        {
            canvas.blocksRaycasts = true;

        });
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;
            defTeamIcon.skeleton.a = alpha;
            atkTeamIcon.skeleton.a = alpha;
        }).OnComplete(() =>
        {
            canvas.blocksRaycasts = false;
            defTeamIcon.gameObject.SetActive(false);
            atkTeamIcon.gameObject.SetActive(false);
            onComplete?.Invoke();
        });

    }

    public void OnResetPress()
    {
        GameManager.Instance.ResetGame();
        Hide();
    }

    public void OnHomePress()
    {
        GameManager.Instance.GoHome();
        Hide();
    }
}
