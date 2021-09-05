using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIGameplay : MonoBehaviour
{
    [SerializeField] private Transform statPanel;
    [SerializeField] private TextMeshProUGUI txtStatTitle;
    [SerializeField] private TextMeshProUGUI txtStatHP;
    [SerializeField] private TextMeshProUGUI txtStatRandom;
    [SerializeField] private TextMeshProUGUI txtStatDamage;
    [SerializeField] private TextMeshProUGUI txtDefPower;
    [SerializeField] private TextMeshProUGUI txtAtkPower;
    [SerializeField] private TextMeshProUGUI txtTotalChar;
    [SerializeField] private Slider sldDefPower;
    [SerializeField] private Slider sldAtkPower;
    [SerializeField] private Image changeSpeedFillImg;
    [SerializeField] private TextMeshProUGUI changeSpeedText;

    [SerializeField] private CanvasGroup canvas;

    private int[] speed = new[] {1, 2, 3, 4};
    private int totalChar = 0;
    private int atkPower = 0;
    private int defPower = 0;
    private int numberSpeedPress = -1;
    private string speedFormat = "x{0}";
    private string statTitleFormat = "{0} Team";
    private string statHPFormat = "HP: {0}/{1}";
    private string statRandomFormat = "Random: {0}";
    private string statDamageFormat = "Damage: {0}";
    private AxieController cacheAxieTouch;

    public void Show(DataPower dataDef, DataPower dataAtk, int _totalChar)
    {
        //canvas.alpha = 1;
        float alpha = 0;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;
        }).OnComplete(() =>
        {
            canvas.blocksRaycasts = true;
        });
        totalChar = _totalChar;
        atkPower = dataAtk.Power;
        defPower = dataDef.Power;
        txtTotalChar.text = totalChar.ToString();
        txtDefPower.text = defPower.ToString();
        txtAtkPower.text = atkPower.ToString();
        sldDefPower.value = 1;
        sldAtkPower.value = 1;
        numberSpeedPress = -1;

        OnChangeSpeedPress();
    }

    public void Hide(Action onComplete = null)
    {
        float alpha = 1;
        DOTween.To(() => alpha, x => alpha = x, 0, 0.25f).OnUpdate(() =>
        {
            canvas.alpha = alpha;

        }).OnComplete(
        () =>
        {
            canvas.blocksRaycasts = false;
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

    public void SettingPress()
    {
        GameManager.Instance.PauseGame();
        UIManager.Instance.ShowSetting();
    }


    public void OnChangeSpeedPress()
    {
        numberSpeedPress++;
        int index = numberSpeedPress % speed.Length;
        float progress = (index+1) * 1.0f / speed.Length;
        changeSpeedFillImg.fillAmount = progress;
        changeSpeedText.text = string.Format(speedFormat,speed[index].ToString());

        GameManager.Instance.ChangeSpeed(speed[index]);
    }

    public void SetSelectCharacter(AxieController axie)
    {
        if (axie == null)
        {
            statPanel.gameObject.SetActive(false);
        }
        else
        {
            statPanel.gameObject.SetActive(true);
        }
        
        cacheAxieTouch = axie;
        OnUpdateStatSelectChar();
    }

    public void OnUpdateStatSelectChar()
    {
        if (cacheAxieTouch != null)
        {
            txtStatTitle.text = string.Format(statTitleFormat, cacheAxieTouch.Team);
            txtStatHP.text = string.Format(statHPFormat, cacheAxieTouch.HP, cacheAxieTouch.MaxHp);
            txtStatRandom.text = string.Format(statRandomFormat, cacheAxieTouch.RandomNumber);
            txtStatDamage.text = string.Format(statDamageFormat, cacheAxieTouch.Damage);
        }
    }

}
