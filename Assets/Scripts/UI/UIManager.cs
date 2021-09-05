using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    
    [SerializeField] private UIMainMenu uiMainMenu;
    [SerializeField] private UIGameplay uiGameplay;
    [SerializeField] private UISetting uiSetting;
    [SerializeField] private UIEndGame uiEndGame;

    public UIGameplay UIGamePlay => uiGameplay;

    
    public void ShowMainMenu()
    {
        
        uiMainMenu.Show();
    }

    public void HideMainMenu()
    {
        uiMainMenu.Hide();
    }

    public void ShowGamePlay(DataPower dataDef, DataPower dataAtk, int _totalChar)
    {
        uiGameplay.Show(dataDef,dataAtk,_totalChar);
    }

    public void HideGamePlay(Action onComplete = null)
    {
        uiGameplay.Hide(onComplete);
    }

    public void ShowSetting()
    {
        uiSetting.Show();
    }

    public void HideSetting()
    {
        uiSetting.Hide();
    }

    public void ShowEndGame(AxieTeam teamWin)
    {
        uiEndGame.Show(teamWin);
    }

    public void HideEndGame()
    {
        uiEndGame.Hide();
    }

}
