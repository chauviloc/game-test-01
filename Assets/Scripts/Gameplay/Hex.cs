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

    private const float Size = 1.0f;
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
        float height = 2 * Size;
        float width = widthMul * Size;
        float verticalDistance = height * 0.75f;    // height * 3/4
        float horizontalDistance = width;

        return new Vector3(horizontalDistance * (Q + R*0.5f), verticalDistance * R, 0);

    }

}
