using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex
{
    // Col
    public readonly int Q;
    // Row
    public readonly int R;
    // S = -Q-R
    public readonly int S;

    
    private float widthMul = Mathf.Sqrt(3);

    public Hex(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        //S = -Q - R;
        //Debug.Log(Q+R+S);
    }

    public Vector3 WorldPosition()
    {
        float height = 2 * GameConstants.HEX_CELL_SIZE;
        float width = widthMul * GameConstants.HEX_CELL_SIZE;
        float verticalDistance = height * 0.75f;    // height * 3/4
        float horizontalDistance = width;

        return new Vector3(horizontalDistance * (Q + R*0.5f), verticalDistance * R, 0);

    }

}
