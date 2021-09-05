using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    public const string MAINMENU = "MainMenu";
    public const string PLAYING = "Playing";
    public const string PAUSE = "Pause";
    public const string ENDING = "Ending";

    public const string MAINMENU_TO_PLAYING = "MainMenuToPlaying";
    public const string PLAYING_TO_ENDING = "PlayingToEnding";
    public const string ENDING_TO_MAINMENU = "EndingToMainMenu";
    public const string ENDING_TO_PLAYING = "EndingToPlaying";
    public const string PLAYING_TO_PAUSE = "PlayingToPause";

    public const float DEFAULT_SECOND_PER_TICK = 1;
    public const int DEFAULT_MAP_RADIUS = 5;
    public const int DEFAULT_MAP_DEF_RADIUS = 2;
    public const int HEX_CELL_SIZE = 1;
    public const int DEFAULT_LAYER_NUMBER = 5;
    public const int STEP_INCREASE_HEX = 2;

    public const string POOL_AXIE_DEF = "CharDef";
    public const string POOL_AXIE_ATK = "CharAtk";

    public const string ANIMATION_APPEAR = "action/appear";
    public const string ANIMATION_IDLE = "action/idle";
    public const string ANIMATION_MOVE = "action/move-forward";
    public const string ANIMATION_ATTACK = "attack/melee/horn-gore";
    public const string ANIMATION_HIT = "defense/hit-by-horn-attack";
    public const string ANIMATION_DEAD = "defense/hit-by-ranged-attack";

}
