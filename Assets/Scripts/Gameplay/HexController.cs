using System.Collections;
using System.Collections.Generic;
using MarchingBytes;
using UnityEngine;

//public enum HexStatus
//{
//    Empty,

//}

public class HexController : MonoBehaviour
{
    
    [SerializeField] private SpriteRenderer spriteBG;
    [SerializeField] private AxieController axieCharacter;
    
    public Hex HexData => hexData;
    public int FlatIndex => flatIndex;  // this Index will use in 1D list.
    public int HexDistance => hexDistance;
    

    //public HexStatus Status
    //{
    //    get { return status; }
    //    set
    //    {
    //        status = value;

    //    }
    //}

    private Hex hexData;
    //private HexStatus status;
    private int flatIndex;
    private int hexDistance;


    //Debug
    public int Q;
    public int R;
    public int S;

    public void Init(Hex hex, bool showHex)
    {
        hexData = hex;
        hexDistance = Mathf.Max(Mathf.Abs(hexData.Q), Mathf.Abs(hexData.R), Mathf.Abs(hexData.S));

        spriteBG.gameObject.SetActive(showHex);


        // Use for Debug => Will remove
        Q = hex.Q;
        R = hex.R;
        S = hex.S;

    }

    public bool IsEmpty()
    {
        return axieCharacter == null;
    }

    public void SetCharacter(AxieController axie)
    {
        if (IsEmpty())
        {
            axieCharacter = axie;
        }
        else
        {
            Debug.Log("This Hex Cell Not Empty!!!!");
        }
    }

    public void ResetCharacter()
    {
        if (IsEmpty())
        {
            return;
        }

        EasyObjectPool.instance.ReturnObjectToPool(axieCharacter.gameObject);
        axieCharacter = null;
    }

    public void ComputeFlatIndex(int radius)
    {
        GetFlatGridPosition(radius);
    }


    /// <summary>
    /// Convert to 1D Grid  => use with float list
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    public int GetFlatGridPosition(int radius)
    {
        Vector2 gridPos = GetGridPosition(radius);
        int index = (int)(gridPos.y + gridPos.x * (radius * 2 + 1));
        flatIndex = index;
        return index;
    }


    /// <summary>
    /// Convert to 2D Grid
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    public Vector2 GetGridPosition(int radius)
    {
        int x = radius + hexData.Q;
        int y = radius + hexData.R;
        return new Vector2(x, y);
    }


}
