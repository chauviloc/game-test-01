using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hpSpriteRender;
    [SerializeField] private float maxSize = 5;

    private Gradient g;

    public void Init()
    {
        g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = Color.green;
        gck[0].time = 1.0F;
        gck[1].color = Color.red;
        gck[1].time = -1.0F;

        gak[0].alpha = 1.0F;
        gak[0].time = 1.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = -1.0F;
        g.SetKeys(gck, gak);

    }

    public void UpdateHPBar(int currentValue, int maxValue)
    {
        //float previousSize = hpSpriteRender.transform.localScale.x;
        float progress = currentValue * 1.0f / maxValue;
        float currentSize = progress * maxSize;
        //hpSpriteRender.transform.localScale = new Vector3(currentSize,1,1);
        hpSpriteRender.transform.DOScaleX(currentSize,0.25f);
        hpSpriteRender.color = g.Evaluate(progress);
    }

}
