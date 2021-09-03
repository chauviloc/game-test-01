using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Data", menuName = "AxieStats", order = 1)]
public class AxieStats : ScriptableObject
{
    public AxieTeam Team;
    public int HP;
    public Vector2Int RandomRange;
}
