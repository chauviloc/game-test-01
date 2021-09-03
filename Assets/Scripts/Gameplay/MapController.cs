using System;
using System.Collections;
using System.Collections.Generic;
using MarchingBytes;
using UnityEngine;


public enum DataChangeType
{
    Atk,
    Move,
    Idle
}

public enum AxieTeam
{
    None,
    Def,
    Atk
}

public class DataChange
{
    public DataChangeType Type;
    public Vector2Int SenderHexCoord;
    public Vector2Int RecieverHexCoord;
}


public class MapController : MonoBehaviour
{

    [SerializeField] private Transform Test;

    [SerializeField] private bool showMapVisual;
    [SerializeField] private GameObject hexCellPrefab;

    
    [SerializeField] private List<HexController> listHexCreateMap = new List<HexController>();  // Use this list for create map at begin => will clear when get into game.
    [SerializeField] private List<HexController> listHexFlat = new List<HexController>(); // Main list use to update game.

    [SerializeField] private List<AxieController> axieDefs = new List<AxieController>();
    [SerializeField] private List<AxieController> axieAtks = new List<AxieController>();

    
    private Dictionary<int, List<HexController>> hexByLayer = new Dictionary<int, List<HexController>>();

    private int mapRadius = 5;
    private int mapDefRadius = 2;
    private float hexCellwidthMul = Mathf.Sqrt(3);

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
        //Debug.Log("Tick: " + tick);
        //for (int i = 0; i < listHexFlat.Count; i++)
        //{
        //    listHexFlat[i]?.OnUpdate(tick);
        //}
        //UpdateTurn();
    }


    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            //var hexCoor = ConvertWorldPosToHexCoord(Test.position);
            //var startHex = listHexFlat[GetFlatGridPosition(hexCoor.x, hexCoor.y, mapRadius)];
            //var endHex = listHexFlat[GetFlatGridPosition(0, 0, mapRadius)];
            //var line = LinearInterpolateFrom2Hex(startHex, endHex);
            //for (int i = 0; i < line.Count; i++)
            //{
            //    var coord = ConvertWorldPosToHexCoord(line[i]);
            //    listHexFlat[GetFlatGridPosition(coord.x, coord.y, mapRadius)].transform.localScale = Vector3.one * 0.5f;
            //}
            UpdateTurn();
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
        

        for (int i = 0; i <= mapRadius; i++)
        {
            hexByLayer.Add(i,GetLayerHexByLayerIndex(i));
        }

        Debug.Log(log);

        //var list = GetLayerHexByLayerIndex(3);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    list[i].transform.localScale = Vector3.one * 0.5f;
        //}

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

    public void UpdateTurn()
    {
        // Compute Data for this turn
        //List<DataChange> datas = new List<DataChange>();
        for (int i = mapRadius; i >=0; i--)
        {
            var allHexInLayer = hexByLayer[i];
            for (int j = 0; j < allHexInLayer.Count; j++)
            {
                if (allHexInLayer[j].HexData.Q == -4 && allHexInLayer[j].HexData.R == 1)
                {
                    Debug.Log("Test");
                }

                if (!allHexInLayer[j].IsEmpty())
                {
                    GetNextMove(allHexInLayer[j]);
                }
            }
        }

        Debug.Log("Test");
        // Update render with new data
        //UpdateRender(datas);



    }

    public void UpdateRender(List<DataChange> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            var hexSender = listHexFlat[GetFlatGridPosition(datas[i].SenderHexCoord.x, datas[i].SenderHexCoord.y, mapRadius)];
            if (datas[i].Type == DataChangeType.Move)
            {
                var hexTarget = listHexFlat[GetFlatGridPosition(datas[i].RecieverHexCoord.x, datas[i].RecieverHexCoord.y, mapRadius)];
                hexSender.MoveCharacterTo(hexTarget);
            }
        }
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

    public List<Vector2> LinearInterpolateFrom2Hex(HexController start, HexController end)
    {
        List<Vector2> listPoint = new List<Vector2>();
        int distance = GetDistance2Hex(start,end);

        for (int i = 0; i < distance+1; i++)
        {
            listPoint.Add(start.transform.position + (end.transform.position - start.transform.position * 1/distance *i));
        }

        return listPoint;
    }

    public Vector2Int ConvertWorldPosToHexCoord(Vector3 worldPos)
    {
        float height = 2 * GameConstants.HEX_CELL_SIZE;
        float width = hexCellwidthMul * GameConstants.HEX_CELL_SIZE;
        float verticalDistance = height * 0.75f;    // height * 3/4
        float horizontalDistance = width;

        //return new Vector3(horizontalDistance * (Q + R * 0.5f), verticalDistance * R, 0);

        float tempR = worldPos.y / verticalDistance;
        float tempQ = (worldPos.x / horizontalDistance) - (tempR * 0.5f);

        int R = (int)Math.Round(tempR, MidpointRounding.AwayFromZero);
        int Q = (int)Math.Round(tempQ, MidpointRounding.AwayFromZero);

        return new Vector2Int(Q,R);
    }

    private int GetDistance2Hex(HexController start, HexController end)
    {
        return Mathf.Max(Mathf.Abs(start.HexData.Q-end.HexData.Q), Mathf.Abs(start.HexData.R - end.HexData.R), Mathf.Abs(start.HexData.S - end.HexData.S));
    }

    private void GetNextMove(HexController hexCtr)
    {
        
        if (hexCtr.IsEmpty())
        {
            return;
        }
        List<HexController> emptyNeighbor = new List<HexController>();
        List<HexController> enemyNeighbor = new List<HexController>();
        var neighborHexCoord = hexCtr.GetNeighborHexCoordinate();
        AxieTeam currentTeam = hexCtr.GetAxieTeam();
        
        for (int i = 0; i < neighborHexCoord.Count; i++)
        {
            
            int index = Mathf.Clamp(GetFlatGridPosition(neighborHexCoord[i].x, neighborHexCoord[i].y, mapRadius),0,listHexFlat.Count-1);
            var hexNeighbor = listHexFlat[index];
            if (hexNeighbor != null)
            {
                if (hexNeighbor.IsEmpty())
                {
                    // Add to empty list
                    emptyNeighbor.Add(hexNeighbor);
                }
                else
                {
                    // had something in this hex
                    if (currentTeam != hexNeighbor.GetAxieTeam())
                    {
                        enemyNeighbor.Add(hexNeighbor);
                    }
                }
            }
        }

        if (emptyNeighbor.Count <= 0)
        {
            //DataChange data = new DataChange();
            //data.SenderHexCoord = new Vector2Int(hexCtr.HexData.Q, hexCtr.HexData.R);
            //data.Type = DataChangeType.Idle;
            return;
        }

        if (enemyNeighbor.Count > 0)
        {
            // Had enemy => Attack
            //DataChange data = new DataChange();
            //data.SenderHexCoord = new Vector2Int(hexCtr.HexData.Q, hexCtr.HexData.R);
            //data.RecieverHexCoord = new Vector2Int(enemyNeighbor[0].HexData.Q,enemyNeighbor[0].HexData.R);
            //data.Type = DataChangeType.Atk;
        }
        else
        {
            // had empty and dont have enemy => find the closet enemy then move on
            if (currentTeam == AxieTeam.Def)
            {
                return;
            }
            hexCtr.MoveCharacterTo(emptyNeighbor[0]);

            //DataChange data = new DataChange();
            //data.SenderHexCoord = new Vector2Int(hexCtr.HexData.Q, hexCtr.HexData.R);
            //data.RecieverHexCoord = new Vector2Int(emptyNeighbor[0].HexData.Q, emptyNeighbor[0].HexData.R);
            //data.Type = DataChangeType.Move;
            //return data;
        }

    }

}
