using System.Collections;
using System.Collections.Generic;
using MarchingBytes;
using UnityEngine;


public enum DataChangeType
{
    Atk,
    Move
}

public enum AxieTeam
{
    Def,
    Atk
}

public class DataChange
{
    public DataChangeType Type;
    public Vector3 Sender;
    public Vector3 Reciever;
}


public class MapController : MonoBehaviour
{
    [SerializeField] private bool showMapVisual;
    [SerializeField] private GameObject hexCellPrefab;

    
    [SerializeField] private List<HexController> listHexCreateMap = new List<HexController>();  // Use this list for create map at begin => will clear when get into game.
    [SerializeField] private List<HexController> listHexFlat = new List<HexController>(); // Main list use to update game.

    [SerializeField] private List<AxieController> axieDefs = new List<AxieController>();
    [SerializeField] private List<AxieController> axieAtks = new List<AxieController>();
    //[SerializeField] private List<>

    private int mapRadius = 5;
    private int mapDefRadius = 2;
    
    public void CreateMap(int radius)
    {
        mapRadius = radius;

        int x = 0; // row
        int y = 0; // col

       
        for (int col = -mapRadius; col <= mapRadius; col++)
        {
            //x = mapRadius + col;
            for (int row = -mapRadius; row <= mapRadius; row++)
            {
                //y = mapRadius + row;
                for (int s = -mapRadius; s <= mapRadius; s++)
                {
                    if ((col + row + s) == 0)
                    {
                        Hex hex = new Hex(col, row, s);

                        var hexGO = Instantiate(hexCellPrefab, hex.WorldPosition(), Quaternion.identity).GetComponent<HexController>();
                        hexGO.Init(hex,showMapVisual);
                        listHexCreateMap.Add(hexGO);
                        CreateAxie(hexGO, mapDefRadius);
                       
                    }
                    
                }
                
            }
        }

    }

    private void CreateAxie(HexController hexGO, int radius)
    {
        if (hexGO.HexDistance <= radius)
        {
            hexGO.SetCharacter(CreateAxie(AxieTeam.Def, hexGO.HexData.WorldPosition()));
        }
        else if (hexGO.HexDistance > radius + 1)
        {
            hexGO.SetCharacter(CreateAxie(AxieTeam.Atk, hexGO.HexData.WorldPosition()));
        }
    }

    private AxieController CreateAxie(AxieTeam axieTeam, Vector3 pos)
    {
        var axieMasterData = DataConfig.Instance.GetAxieMasterDataByType(axieTeam);
        var axie = EasyObjectPool.instance.GetObjectFromPool(axieTeam == AxieTeam.Def? GameConstants.POOL_AXIE_DEF : GameConstants.POOL_AXIE_ATK, pos, Quaternion.identity).GetComponent<AxieController>(); 
        axie.Init(axieMasterData);
        if (axieTeam == AxieTeam.Atk)
        {
            axieAtks.Add(axie);
        }
        else
        {
            axieDefs.Add(axie);
        }
        return axie;
    }

    public void OnUpdate(float tick)
    {
        Debug.Log("Tick: " + tick);
        for (int i = 0; i < listHexFlat.Count; i++)
        {
            listHexFlat[i]?.OnUpdate(tick);
        }
    }


    private List<HexController> GetLayerHexByLayerIndex(int layer)
    {
        List<HexController> listHexLayer = new List<HexController>();

        for (int r = 0; r <= layer; r++)
        {
            int q = -layer;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q,r,mapRadius)]);
        }

        for (int q = -layer+1; q <= 0; q++)
        {
            int r = layer;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q, r, mapRadius)]);
        }

        for (int q = 1; q <= layer; q++)
        {
            int r = layer - q;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q, r, mapRadius)]);
        }

        for (int r = -1; r >= -layer; r--)
        {
            int q = layer;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q, r, mapRadius)]);
        }

        for (int q = layer-1; q >=0 ; q--)
        {
            int r = -layer;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q, r, mapRadius)]);
        }

        for (int q = -1; q >= -layer+1; q--)
        {
            int r = -layer-q;
            listHexLayer.Add(listHexFlat[GetFlatGridPosition(q, r, mapRadius)]);
        }

        //for (int i = 0; i < listHexData.Count; i++)
        //{
        //    log = "";
        //    for (int j = 0; j < listHexData[i].Count; j++)
        //    {
        //        var hex = listHexData[i][j];

        //        if (hex != null)
        //        {
        //            Vector2 gridPos = GetGridPosition(hex.HexData.Q, hex.HexData.R);
        //            log += "(" + gridPos.x + "," + gridPos.y + ")-";
        //            int distance = Mathf.Max(Mathf.Abs(hex.HexData.Q), Mathf.Abs(hex.HexData.R), Mathf.Abs(hex.HexData.S));
        //            if (distance == layer)
        //            {
        //                hex.transform.localScale = Vector3.one * 0.5f;
        //                listHexLayer.Add(hex);
        //            }
        //        }
        //        else
        //        {
        //            log += "null-";
        //        }
        //    }
        //    Debug.Log(log);
        //}

        return listHexLayer;
    }

    public void AddMapLayer()
    {
        int previousMapRadius = mapRadius;
        mapRadius += GameConstants.STEP_INCREASE_HEX;
        
        listHexFlat.Clear();
        for (int col = -mapRadius; col <= mapRadius; col++)
        {
            //x = mapRadius + col;
            for (int row = -mapRadius; row <= mapRadius; row++)
            {
                //y = mapRadius + row;
                for (int s = -mapRadius; s <= mapRadius; s++)
                {
                    if ((col + row + s) == 0)
                    {
                        int distance = Mathf.Max(Mathf.Abs(col), Mathf.Abs(row), Mathf.Abs(s));
                        if (distance > previousMapRadius)
                        {
                            Hex hex = new Hex(col, row, s);
                            var hexGO = Instantiate(hexCellPrefab, hex.WorldPosition(), Quaternion.identity).GetComponent<HexController>();
                            hexGO.Init(hex,showMapVisual);
                            listHexCreateMap.Add(hexGO);
                            CreateAxie(hexGO, mapDefRadius);
                           
                        }
                        
                    }

                }

            }
        }

        // Update free circle
        for (int i = 0; i < listHexCreateMap.Count; i++)
        {
            int hexDistance = listHexCreateMap[i].HexDistance;
            if (hexDistance == mapDefRadius+2)
            {
                listHexCreateMap[i].ResetCharacter();
            }
            else if (hexDistance == mapDefRadius + 1)
            {
                listHexCreateMap[i].SetCharacter(CreateAxie(AxieTeam.Def, listHexCreateMap[i].HexData.WorldPosition()));
            }
        }

        mapDefRadius++;
    }

    public void GeneradeMapData()
    {

        for (int i = 0; i < mapRadius * 2 + 1; i++)
        {
            for (int j = 0; j < mapRadius * 2 + 1; j++)
            {
                listHexFlat.Add(null);
            }
        }


        for (int i = 0; i < listHexCreateMap.Count; i++)
        {
            //int index = GetFlatGridPosition(listHexCreateMap[i].HexData.Q, listHexCreateMap[i].HexData.R, mapRadius);
            listHexCreateMap[i].ComputeFlatIndex(mapRadius);
            listHexFlat[listHexCreateMap[i].FlatIndex] = listHexCreateMap[i];
        }

        string log = "";
        for (int i = 0; i < listHexFlat.Count; i++)
        {
            if (listHexFlat[i] != null)
            {
                log += "hex," + i + "-";
            }
            else
            {
                log += "null-";
            }
        }
        listHexCreateMap.Clear();
        Debug.Log(log);

        var list = GetLayerHexByLayerIndex(3);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.localScale = Vector3.one * 0.5f;
        }

        // Test Neighbor
        //int checkIndex = GetFlatGridPosition(5, 0, mapRadius);
        //var listTemp = listHexFlat[checkIndex].GetNeighborHexCoordinate();
        //for (int i = 0; i < listTemp.Count; i++)
        //{
        //    int index = GetFlatGridPosition(listTemp[i].x, listTemp[i].y, mapRadius);
        //    var hexNeighbor = listHexFlat[Mathf.Clamp(index, 0, listHexFlat.Count - 1)];
        //    if (hexNeighbor != null)
        //    {
        //        hexNeighbor.transform.localScale = Vector3.one * 0.5f;
        //    }
        //}

    }



    public int GetFlatGridPosition(int q, int r, int radius)
    {
        Vector2 gridPos = GetGridPosition(q, r, radius);
        int index = (int)(gridPos.y + gridPos.x * (radius * 2 + 1));
        return index;
    }


    public Vector2 GetGridPosition(int q, int r, int radius)
    {
        int x = radius + q;
        int y = radius + r;
        return new Vector2(x, y);
    }

  
}
