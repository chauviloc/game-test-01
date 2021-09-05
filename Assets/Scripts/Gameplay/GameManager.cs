using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : Singleton<GameManager>
{

    [SerializeField] private MapController hexMap;
    [SerializeField] private FPSDisplay fpsDisplay;
    

    [HideInInspector] public StateMachine _stateMachine;

    public float SecondPerTick => secondPerTick;
    public MapController HexMap => hexMap;
    
    public string CurrentState
    {
        get { return _stateMachine.CurrentState.name; }
    }


    private string previousState;
    private float secondPerTick;
    private float currentTickTime;
    private float tick;
    private bool generadeMapDone = false;

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

        secondPerTick = GameConstants.DEFAULT_SECOND_PER_TICK;

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



    public void StartGame()
    {
        _stateMachine.ProcessTriggerEvent(GameConstants.MAINMENU_TO_PLAYING);
        
    }


    public void PauseGame()
    {
        hexMap.PauseCamera();
        previousState = _stateMachine.CurrentState.name;
        _stateMachine.SetCurrentState(GameConstants.PAUSE);
        
    }

    public void UnPauseGame()
    {
        hexMap.UnPauseCamera();
        _stateMachine.SetCurrentState(previousState);
      
    }


    public void ChangeSpeed(int value)
    {
        secondPerTick = GameConstants.DEFAULT_SECOND_PER_TICK / value;
        //Debug.Log("Speed: x" + value);
    }

    private void OnMainMenuIn()
    {
        hexMap.Init();
    }

    private IEnumerator CreateMoreLayer(Action onComplete)
    {
        yield return new WaitForSeconds(1.0f);
        float fps = 60;
        while (fps > 30)
        {
            fps = 1 / Time.unscaledDeltaTime;
            hexMap.AddMapLayer();
            yield return new WaitForSeconds(0.25f);
        }

        // Create Done => Start Game
        onComplete?.Invoke(); 
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
       
       
        hexMap.CreateMap(GameConstants.DEFAULT_LAYER_NUMBER);
        StartCoroutine(CreateMoreLayer(() =>
        {
            hexMap.GeneradeMapData();
            generadeMapDone = true;
            //StartGame();
        }));
        
    }

    private void OnPausing()
    {
        
    }

    private void OnPlaying()
    {
       
        // Update Gameplay
        if (!generadeMapDone)
        {
            return;
        }

        currentTickTime += Time.deltaTime;
        while (currentTickTime >= secondPerTick)
        {
            tick++;
            hexMap.OnUpdate(tick);
            float timeOdd = currentTickTime - secondPerTick;
            currentTickTime = timeOdd;
            if (currentTickTime < 0)
            {
                currentTickTime = 0;
            }
        }

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
