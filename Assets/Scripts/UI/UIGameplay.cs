using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIGameplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI txtDefPower;
    [SerializeField] private TextMeshProUGUI txtAtkPower;
    [SerializeField] private TextMeshProUGUI txtTotalChar;
    [SerializeField] private Slider sldDefPower;
    [SerializeField] private Slider sldAtkPower;
    [SerializeField] private CanvasGroup canvas;

    private int totalChar = 0;
    private int atkPower = 0;
    private int defPower = 0;

    public void Show(DataPower dataDef, DataPower dataAtk, int _totalChar)
    {
        canvas.alpha = 1;
        totalChar = _totalChar;
        atkPower = dataAtk.Power;
        defPower = dataDef.Power;
        txtTotalChar.text = totalChar.ToString();
        txtDefPower.text = defPower.ToString();
        txtAtkPower.text = atkPower.ToString();
        sldDefPower.value = 1;
        sldAtkPower.value = 1;
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {

        });

        canvas.DOFade(0, 0.25f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }


    public void UpdateUI(DataPower dataDef, DataPower dataAtk)
    {
        float defValue = dataDef.Power * 1.0f / defPower;
        float atkValue = dataAtk.Power * 1.0f / atkPower;
        txtDefPower.text = dataDef.Power.ToString();
        txtAtkPower.text = dataAtk.Power.ToString();
        sldDefPower.value = defValue;
        sldAtkPower.value = atkValue;

    }

}
