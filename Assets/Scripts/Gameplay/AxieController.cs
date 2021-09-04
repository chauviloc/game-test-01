using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MarchingBytes;
using Spine;
using Spine.Unity;
using UnityEngine;


public class AxieController : MonoBehaviour
{

    [SerializeField] private HPBarController hpBar;
    [SerializeField] private SkeletonAnimation skeletonAnim;

    public int HP => hp;
    public AxieTeam Team => team;
    public int RandomNumber => randomNumber;

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
        hp -= damage;
        hpBar.UpdateHPBar(hp,cacheAxieMasterData.HP);
        if (team == AxieTeam.Def)
        {
            skeletonAnim.AnimationState.SetAnimation(0, GameConstants.ANIMATION_HIT, false);
            skeletonAnim.AnimationState.SetAnimation(1, GameConstants.ANIMATION_IDLE, true);
        }

        if (hp <= 0)
        {
            return true;
        }

        return false;
    }

    public int CalculateDamageDeal(int targetRandomNumber)
    {
        int result = (3 + randomNumber - targetRandomNumber) % 3;
        
        return damages[result];
    }

    public void FaceToEnemy(float direction)
    {
        skeletonAnim.skeleton.flipX = direction >= 0;
    }

}
