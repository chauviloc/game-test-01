using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    
    [SerializeField] private UIMainMenu uiMainMenu;
    [SerializeField] private UIGameplay uiGameplay;

    public UIGameplay UIGamePlay => uiGameplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void HideGamePlay()
    {
        uiGameplay.Hide();
    }

}
