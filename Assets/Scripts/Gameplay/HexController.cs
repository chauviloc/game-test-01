﻿using System.Collections;
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

    List<Vector3Int> neighbors = new List<Vector3Int>();
    
    private Hex hexData;
    private int flatIndex;
    private int hexDistance;

    private List<Vector3> hexDirection = new List<Vector3>()
    {
        new Vector3(+1, -1, 0),
        new Vector3(+1, 0, -1),
        new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0),
        new Vector3(-1, 0, +1),
        new Vector3(0, -1, +1),
    };


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

    public void MoveCharacterTo(HexController hexTarget)
    {
        axieCharacter.MoveTo(hexTarget.hexData.WorldPosition());
        hexTarget.SetCharacter(axieCharacter);

        RemoveCharacter();
    }

    public List<Vector3Int> GetNeighborHexCoordinate()
    {
        if (neighbors.Count <= 0)
        {
            for (int i = 0; i < hexDirection.Count; i++)
            {
                neighbors.Add(new Vector3Int((int)hexDirection[i].x + hexData.Q, (int)hexDirection[i].y + hexData.R, (int)hexDirection[i].z + hexData.S));
            }
        }

        return neighbors;
    }

    
    public bool IsEmpty()
    {
        return axieCharacter == null;
    }

    public AxieTeam GetAxieTeam()
    {
        if (IsEmpty())
        {
            return AxieTeam.None;
        }
        else
        {
            return axieCharacter.Team;
        }
    }

    public void SetCharacter(AxieController axie)
    {
        if (IsEmpty())
        {
            axieCharacter = axie;
            axie.SetStandingHex(this);
        }
        else
        {
            Debug.Log("This Hex Cell Not Empty!!!!");
        }
    }

    public void RemoveCharacter(bool returnToPool = false)
    {
        if (returnToPool)
        {
            EasyObjectPool.instance.ReturnObjectToPool(axieCharacter.gameObject);
        }
        axieCharacter = null;
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
