using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EventDispatcher;
using MarchingBytes;
using Spine;
using Spine.Unity;
using UnityEngine;


public class AxieController : MonoBehaviour
{

    [SerializeField] private HPBarController hpBar;
    [SerializeField] private SkeletonAnimation skeletonAnim;

    public int HP => hp;
    public int MaxHp => cacheAxieMasterData.HP;
    public AxieTeam Team => team;
    public int RandomNumber => randomNumber;
    public int Damage => cacheDamage;

    private int cacheDamage;
    private int hp;
    private AxieTeam team;
    private Vector2Int rangeRandom;
    private AxieStats cacheAxieMasterData;

    //private HexController currentHexStanding;

    private int randomNumber;
    private int[] damages = new[] { 4,5,3};

    public void Init(AxieStats masterData)
    {
        hpBar.Init();
        cacheAxieMasterData = masterData;
        hp = cacheAxieMasterData.HP;
        team = cacheAxieMasterData.Team;
        rangeRandom = cacheAxieMasterData.RandomRange;
        randomNumber = Random.Range(rangeRandom.x, rangeRandom.y + 1);
        
        hpBar.UpdateHPBar(hp, cacheAxieMasterData.HP);

        skeletonAnim.AnimationState.End -= OnAnimationEnd;
        skeletonAnim.AnimationState.End += OnAnimationEnd;


        skeletonAnim.AnimationState.SetAnimation(0, GameConstants.ANIMATION_APPEAR, false);
        skeletonAnim.AnimationState.SetAnimation(1, GameConstants.ANIMATION_IDLE, true);

        this.PostEvent(EventID.UpdatePower,new DataPower()
        {
            Team = team,
            Power = hp,
        });
    }

   
    void OnAnimationEnd(TrackEntry trackEntry)
    {
        if (trackEntry.animation.name == GameConstants.ANIMATION_DEAD)
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        }
    }

    public void AttackTo(Vector2 targetPos, float time)
    {
        if (team == AxieTeam.Atk)
        {
            Vector2 currentPos = transform.position;
            transform.DOMove(targetPos, time).OnComplete(() => { transform.DOMove(currentPos, time); });
            skeletonAnim.AnimationState.SetAnimation(0, GameConstants.ANIMATION_ATTACK, false);
            skeletonAnim.AnimationState.SetAnimation(1, GameConstants.ANIMATION_IDLE, true);
        }

    }

    public void MoveTo(Vector3 pos, float time)
    {
        skeletonAnim.AnimationState.SetAnimation(0, GameConstants.ANIMATION_MOVE, false);
        skeletonAnim.AnimationState.SetAnimation(1, GameConstants.ANIMATION_IDLE, true);
        transform.DOMove(pos, time);
    }

    /// <summary>
    /// Take damge
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>Is Dead</returns>
    public bool TakeDamage(int damage)
    {
        int lostPower = damage < hp ? damage : hp;
        hp -= damage;
        hpBar.UpdateHPBar(hp,cacheAxieMasterData.HP);
        
        this.PostEvent(EventID.UpdatePower, new DataPower()
        {
            Team = team,
            Power = -lostPower,
        });
        if (team == AxieTeam.Def)
        {
            skeletonAnim.AnimationState.SetAnimation(0, GameConstants.ANIMATION_HIT, false);
            skeletonAnim.AnimationState.SetAnimation(1, GameConstants.ANIMATION_IDLE, true);
        }

        if (hp <= 0)
        {
            hp = 0;
            return true;
        }

        return false;
    }

    public int CalculateDamageDeal(int targetRandomNumber)
    {
        randomNumber = Random.Range(rangeRandom.x, rangeRandom.y + 1);
        int result = (3 + randomNumber - targetRandomNumber) % 3;
        cacheDamage = damages[result];
        return cacheDamage;
    }

    public void FaceToEnemy(float direction)
    {
        skeletonAnim.skeleton.flipX = direction >= 0;
    }

    public void ManualReset()
    {
        hp = 0;
        team = AxieTeam.None;

    }

    public void Touch()
    {
        skeletonAnim.skeleton.SetColor(Color.grey);
    }

    public void ReleaseTouch()
    {
        skeletonAnim.skeleton.SetColor(Color.white);
    }
}
