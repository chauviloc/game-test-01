using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AxieController : MonoBehaviour
{

    public int HP => hp;

    private int hp;
    private AxieTeam team;
    private Vector2 rangeRandom;
    private AxieStats cacheAxieMasterData;

    public void Init(AxieStats masterData)
    {
        cacheAxieMasterData = masterData;
        hp = cacheAxieMasterData.HP;
        team = cacheAxieMasterData.Team;
        rangeRandom = cacheAxieMasterData.RandomRange;

    }


    

}
