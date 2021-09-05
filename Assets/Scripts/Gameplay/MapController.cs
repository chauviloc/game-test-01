using System;
using System.Collections;
using System.Collections.Generic;
using EventDispatcher;
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

public class DataPower
{
    public AxieTeam Team;
    public int Power;
}

public class DataChange
{
    public DataChangeType Type;
    public Vector2Int SenderHexCoord;
    public Vector2Int RecieverHexCoord;
    public int AtkDamage;
}


public class MapController : MonoBehaviour
{

    [SerializeField] private Transform Test;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private bool showMapVisual;
    [SerializeField] private GameObject hexCellPrefab;

    
    [SerializeField] private List<HexController> listHexCreateMap = new List<HexController>();  // Use this list for create map at begin => will clear when get into game.
    [SerializeField] private List<HexController> listHexFlat = new List<HexController>(); // Main list use to update game.

    [SerializeField] private List<AxieController> axieDefs = new List<AxieController>();
    [SerializeField] private List<AxieController> axieAtks = new List<AxieController>();

    
    private Dictionary<int, List<HexController>> hexByLayer = new Dictionary<int, List<HexController>>();
    private List<HexController> listTeamDefLeft = new List<HexController>();
    //private List<HexController> listTeamAtkLeft = new List<HexController>();
    private List<DataChange> dataChanges = new List<DataChange>();
    private DataPower atkTeamPower;
    private DataPower defTeamPower;

    private UIGameplay cacheUIGameplay;
    //public CameraController Camera => cameraController;

    private int mapRadius = 5;
    private int mapDefRadius = 2;
    private float hexCellwidthMul = Mathf.Sqrt(3);

    public void Init()
    {
        mapRadius = GameConstants.DEFAULT_MAP_RADIUS;
        mapDefRadius = GameConstants.DEFAULT_MAP_DEF_RADIUS;
        cacheUIGameplay = UIManager.Instance.UIGamePlay;
        atkTeamPower = new DataPower();
        defTeamPower = new DataPower();
        this.RegisterListener(EventID.UpdatePower,UpdatePower);
    }

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

        UpdateCameraSize();

    }

    private void UpdateCameraSize()
    {
        float height = 2 * GameConstants.HEX_CELL_SIZE;
        //float width = hexCellwidthMul * GameConstants.HEX_CELL_SIZE;
        float totalHeight = height * mapRadius * 0.75f;
        cameraController.UpdateMapSize(-totalHeight,totalHeight);
        //Debug.Log(totalHeight);
    }

    private void CreateAxie(HexController hexGO, int radius)
    {
        if (hexGO.HexDistanceToCenter <= radius)
        {
            hexGO.SetCharacter(CreateAxie(AxieTeam.Def, hexGO.HexData.WorldPosition()));
            listTeamDefLeft.Add(hexGO);
            
        }
        else if (hexGO.HexDistanceToCenter > radius + 1)
        {
            hexGO.SetCharacter(CreateAxie(AxieTeam.Atk, hexGO.HexData.WorldPosition()));
            //listTeamAtkLeft.Add(hexGO);
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
        UpdateTurn();
        cacheUIGameplay.UpdateUI(defTeamPower,atkTeamPower);
    }

    public void PauseCamera()
    {
        cameraController.Pause();
    }

    public void UnPauseCamera()
    {
        cameraController.UnPause();
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
            int hexDistance = listHexCreateMap[i].HexDistanceToCenter;
            if (hexDistance == mapDefRadius+2)
            {
                listHexCreateMap[i].ResetCharacter();
            }
            else if (hexDistance == mapDefRadius + 1)
            {
                listHexCreateMap[i].SetCharacter(CreateAxie(AxieTeam.Def, listHexCreateMap[i].HexData.WorldPosition()));
                listTeamDefLeft.Add(listHexCreateMap[i]);
            }
        }

        mapDefRadius++;
        UpdateCameraSize();
    }

    public void GeneradeMapData()
    {
        Debug.Log("Total: " + axieAtks.Count + ", " + axieDefs.Count);
        for (int i = 0; i < mapRadius * 2 + 1; i++)
        {
            for (int j = 0; j < mapRadius * 2 + 1; j++)
            {
                listHexFlat.Add(null);
            }
        }


        for (int i = 0; i < listHexCreateMap.Count; i++)
        {
            listHexCreateMap[i].ComputeFlatIndex(mapRadius);
            listHexFlat[listHexCreateMap[i].FlatIndex] = listHexCreateMap[i];
        }
        listHexCreateMap.Clear();
        
        // cache hex by layer
        for (int i = 0; i <= mapRadius; i++)
        {
            hexByLayer.Add(i,GetLayerHexByLayerIndex(i));
        }


        UIManager.Instance.ShowGamePlay(defTeamPower, atkTeamPower, (axieAtks.Count + axieDefs.Count));
    }

    public void UpdatePower(object powerData)
    {
        var data = (DataPower) powerData;
        //Debug.Log("UpdatePower: " + data.Team);
        if (data.Team == AxieTeam.Atk)
        {
            
            atkTeamPower.Power += data.Power;
        }
        else if (data.Team == AxieTeam.Def)
        {
            defTeamPower.Power += data.Power;
        }
    }

    public void UpdateTurn()
    {
        // Compute Data for this turn
        for (int i = mapRadius; i >=0; i--)
        {
            var allHexInLayer = hexByLayer[i];
            for (int j = 0; j < allHexInLayer.Count; j++)
            {
                //if (allHexInLayer[j].HexData.Q == -5 && allHexInLayer[j].HexData.R == 0)
                //{
                //    Debug.Log("Test");
                //}

                if (!allHexInLayer[j].IsEmpty())
                {
                    GetNextMove(allHexInLayer[j]);
                }
            }
        }
        // Apply compute data
        ApplyDataChange();
        dataChanges.Clear();

        // Update stat UI when select char
        cacheUIGameplay.OnUpdateStatSelectChar();
    }

    public void ApplyDataChange()
    {
        for (int i = 0; i < dataChanges.Count; i++)
        {
            var data = dataChanges[i];
            var hexSender = listHexFlat[GetFlatGridPosition(data.SenderHexCoord.x, data.SenderHexCoord.y, mapRadius)];
            if (data.Type == DataChangeType.Move)
            {
                var hexTarget = listHexFlat[GetFlatGridPosition(data.RecieverHexCoord.x, data.RecieverHexCoord.y, mapRadius)];
                hexSender.MoveCharacterTo(hexTarget);
                hexSender.MarkChangeData(false);
                hexTarget.MarkChangeData(false);
            }
            else if (data.Type == DataChangeType.Atk)
            {
                var hexTarget = listHexFlat[GetFlatGridPosition(data.RecieverHexCoord.x, data.RecieverHexCoord.y, mapRadius)];
                hexSender.AttackTo(hexTarget,data.AtkDamage);
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
            if (hexNeighbor != null && !hexNeighbor.MarkChange)
            {
                if (hexNeighbor.IsEmpty())
                {
                    // Add to empty list
                    emptyNeighbor.Add(hexNeighbor);
                }
                else
                {
                    // had something in this hex
                    var neighborTeam = hexNeighbor.GetAxieTeam();
                    //Debug.Log(neighborTeam + ", " + currentTeam);
                    if (currentTeam != neighborTeam)
                    {
                        enemyNeighbor.Add(hexNeighbor);
                    }
                }
            }
        }

        
        if (enemyNeighbor.Count > 0)
        {
            // Had enemy => Attack
            DataChange data = new DataChange();
            data.SenderHexCoord = new Vector2Int(hexCtr.HexData.Q, hexCtr.HexData.R);
            data.RecieverHexCoord = new Vector2Int(enemyNeighbor[0].HexData.Q, enemyNeighbor[0].HexData.R);
            data.Type = DataChangeType.Atk;
            data.AtkDamage = hexCtr.CalculateDamageDeal(enemyNeighbor[0].GetRandomNumber());
            dataChanges.Add(data);
        }
        else
        {
            // had empty and dont have enemy => find the closet enemy then move on
            if (emptyNeighbor.Count <= 0 || currentTeam == AxieTeam.Def)
            {
                return;
            }

            int shortestDistance = Int32.MaxValue;
            HexController nextMoveHex = null;
            for (int i = 0; i < emptyNeighbor.Count; i++)
            {
                var emptyHex = emptyNeighbor[i];
                if (!emptyHex.MarkChange)
                {
                    for (int j = 0; j < listTeamDefLeft.Count; j++)
                    {
                        var enemyHex = listTeamDefLeft[i];
                        int distance = emptyHex.DistanceTo(enemyHex);
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            nextMoveHex = emptyHex;
                        }
                    }
                }
            }

            if (nextMoveHex != null)
            {
                //hexCtr.MoveCharacterTo(nextMoveHex);
                nextMoveHex.MarkChangeData(true);
                hexCtr.MarkChangeData(true);

                DataChange data = new DataChange();
                data.SenderHexCoord = new Vector2Int(hexCtr.HexData.Q, hexCtr.HexData.R);
                data.RecieverHexCoord = new Vector2Int(nextMoveHex.HexData.Q, nextMoveHex.HexData.R);
                data.Type = DataChangeType.Move;
                dataChanges.Add(data);
            }

        }

    }

    public void RemoveAxieCache(AxieTeam team, AxieController axie)
    {
        if (team == AxieTeam.Atk)
        {
            axieAtks.Remove(axie);
            if (axieAtks.Count <= 0)
            {
                GameManager.Instance.EndGame(AxieTeam.Def);
            }
        }
        else if (team == AxieTeam.Def)
        {
            axieDefs.Remove(axie);
            if (axieDefs.Count <= 0)
            {
                GameManager.Instance.EndGame(AxieTeam.Atk);
            }
        }
        
    }

    public void ManualReset()
    {

        this.RemoveListener(EventID.UpdatePower, UpdatePower);
        for (int i = 0; i < listHexFlat.Count; i++)
        {
            if (listHexFlat[i] != null)
            {
                Destroy(listHexFlat[i].gameObject);
            }
        }
        listHexFlat.Clear();

        for (int i = 0; i < axieDefs.Count; i++)
        {
            axieDefs[i].ManualReset();
            EasyObjectPool.instance.ReturnObjectToPool(axieDefs[i].gameObject);
        }
        axieDefs.Clear();
        for (int i = 0; i < axieAtks.Count; i++)
        {
            axieAtks[i].ManualReset();
            EasyObjectPool.instance.ReturnObjectToPool(axieAtks[i].gameObject);
        }
        axieAtks.Clear();
        hexByLayer.Clear();
        listTeamDefLeft.Clear();
        dataChanges.Clear();
    }

}
