using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private SkeletonAnimation characterMainMenu;
    [SerializeField] private Transform uiMainMenu;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TapToPlay()
    {
        characterMainMenu.AnimationState.SetAnimation(0, GameConstants.ANIMATION_APPEAR, false).Complete +=
            entry =>
            {
                uiMainMenu.gameObject.SetActive(false);
                GameManager.Instance.StartGame();
            };
        
    }

}
