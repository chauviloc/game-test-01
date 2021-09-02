﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : Singleton<GameManager>
{


    [HideInInspector] public StateMachine _stateMachine;


    public string CurrentState
    {
        get { return _stateMachine.CurrentState.name; }
    }


    private string previousState;

    private void Awake()
    {
        Initialized();
    }

    private void Initialized()
    {
        DOTween.Init();
        Application.targetFrameRate = 60;

        _stateMachine = new StateMachine();

        _stateMachine.CreateState(GameConstants.MAINMENU, OnInit);
        _stateMachine.CreateState(GameConstants.PLAYING, OnPlaying);
        _stateMachine.CreateState(GameConstants.PAUSE, OnPausing);
        _stateMachine.CreateState(GameConstants.ENDING, OnEnding);

        _stateMachine.CreateTransition(GameConstants.MAINMENU, GameConstants.PLAYING, GameConstants.MAINMENU_TO_PLAYING,
            OnPlayingIn);
        _stateMachine.CreateTransition(GameConstants.PLAYING, GameConstants.ENDING, GameConstants.PLAYING_TO_ENDING,
            OnEndingIn);
        _stateMachine.CreateTransition(GameConstants.ENDING, GameConstants.MAINMENU, GameConstants.ENDING_TO_MAINMENU,
            OnMainMenuIn);
        _stateMachine.CreateTransition(GameConstants.ENDING, GameConstants.PLAYING, GameConstants.ENDING_TO_PLAYING,
            OnPlayingIn);
    }

    void Start()
    {
        // Set first state at Main Menu.
        _stateMachine.SetCurrentState(GameConstants.MAINMENU);

        StartCoroutine(RunAfterOneFrame(() =>
        {
            OnMainMenuIn();
            
        }));

    }

    public void TapToPlay()
    {
        _stateMachine.ProcessTriggerEvent(GameConstants.MAINMENU_TO_PLAYING);
        
    }


    public void ShowSetting()
    {
        previousState = _stateMachine.CurrentState.name;
        _stateMachine.SetCurrentState(GameConstants.PAUSE);
        Action onCloseSetting = HideSetting;
        
    }

    public void HideSetting()
    {
        _stateMachine.SetCurrentState(previousState);
      
    }
    

    private void OnMainMenuIn()
    {
        
        
    }

    private IEnumerator RunAfterOneFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        action?.Invoke();
    }

    private void OnInit()
    {
       
    }

    private void OnPlayingIn()
    {
       
       
    }

    private void OnPausing()
    {

    }

    private void OnPlaying()
    {
        
    }

    private void OnEndingIn()
    {
        
    }

    private void HomeCallBack()
    {
        ReturnToMainMenu();
    }

    private void OnEnding()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _stateMachine.ProcessTriggerEvent(GameConstants.ENDING_TO_MAINMENU);
        }
    }

    private void ReturnToMainMenu()
    {
        
    }

    public void EndGame()
    {
        _stateMachine.ProcessTriggerEvent(GameConstants.PLAYING_TO_ENDING);
    }

    // Update is called once per frame
    void Update()
    {
        // Update state
        _stateMachine.Update();
    }

    public bool IsPauseGame()
    {
        return _stateMachine.CurrentState.name == GameConstants.PAUSE;
    }


}
