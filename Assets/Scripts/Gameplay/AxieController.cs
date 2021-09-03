using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AxieController : MonoBehaviour
{

    public int HP => hp;
    public int RandomNumber => randomNumber;

    private int hp;
    private AxieTeam team;
    private Vector2Int rangeRandom;
    private AxieStats cacheAxieMasterData;
    private HexController currentHexStanding;

    private int randomNumber;
    private int[] damages = new[] { 4,5,3};

    public void Init(AxieStats masterData)
    {
        cacheAxieMasterData = masterData;
        hp = cacheAxieMasterData.HP;
        team = cacheAxieMasterData.Team;
        rangeRandom = cacheAxieMasterData.RandomRange;
        randomNumber = Random.Range(rangeRandom.x, rangeRandom.y + 1);
    }

    public void SetStandingHex(HexController hex)
    {
        currentHexStanding = hex;
    }

    public bool CanMove()
    {
        if (cacheAxieMasterData.Team == AxieTeam.Def)
        {
            return false;
        }

        return true;
    }

    public void MoveTo(Vector3 pos)
    {

    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("Dead");
        }
    }

    public void OnUpdate(float tick)
    {

    }

    public int CalculateDamageDeal(int targetRandomNumber)
    {
        int result = 3 + randomNumber - targetRandomNumber;
        return damages[result];
    }

}
