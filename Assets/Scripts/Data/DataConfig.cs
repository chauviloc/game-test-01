using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataConfig : Singleton<DataConfig>
{

    [Header("Data Config")]

    [SerializeField] private List<AxieStats> axieMasterData = new List<AxieStats>();

    public AxieStats GetAxieMasterDataByType(AxieTeam team)
    {
        for (int i = 0; i < axieMasterData.Count; i++)
        {
            if (team == axieMasterData[i].Team)
            {
                return axieMasterData[i];
            }
        }

        return null;
    }

}

public static class Extension
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    

}
