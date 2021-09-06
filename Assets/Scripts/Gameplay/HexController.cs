using MarchingBytes;
using System.Collections.Generic;
using UnityEngine;


public class HexController : MonoBehaviour
{
    
    [SerializeField] private SpriteRenderer spriteBG;
    [SerializeField] private AxieController axieCharacter;
    
    public Hex HexData => hexData;
    public int FlatIndex => flatIndex;  // this Index will use in 1D list.
    public int HexDistanceToCenter => hexDistance;
    public bool MarkChange => markChangeData;

    List<Vector3Int> neighbors = new List<Vector3Int>();
    
    private Hex hexData;
    private int flatIndex;
    private int hexDistance;
    private bool markChangeData;

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

    /// <summary>
    /// This will make sure this Hex already change something.
    /// </summary>
    /// <param name="value"></param>
    public void MarkChangeData(bool value)
    {
        markChangeData = value;
    }

    public void MoveCharacterTo(HexController hexTarget)
    {
        axieCharacter.MoveTo(hexTarget.hexData.WorldPosition(), GameManager.Instance.SecondPerTick*0.25f);
        axieCharacter.FaceToEnemy(hexTarget.transform.position.x - transform.position.x);
        hexTarget.SetCharacter(axieCharacter);

        RemoveCharacter();
    }

    public void AttackTo(HexController hexTarget, int damage)
    {
        if (IsEmpty())
        {
            // why is empty here => it get hit from another character then die
            return;
        }

        hexTarget.TakeDamage(damage); // atk to def
        axieCharacter.AttackTo(hexTarget.transform.position,GameManager.Instance.SecondPerTick * 0.25f);
        axieCharacter.FaceToEnemy(hexTarget.transform.position.x - transform.position.x);
    }

    public int DistanceTo(HexController hexCtr)
    {
        return Mathf.Max(Mathf.Abs(hexCtr.hexData.Q - hexData.Q), Mathf.Abs(hexCtr.hexData.R - hexData.R), Mathf.Abs(hexCtr.hexData.S - hexData.S));
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

    public int CalculateDamageDeal(int targetNumber)
    {
        if (IsEmpty())
        {
            return 0;
        }

        return axieCharacter.CalculateDamageDeal(targetNumber);
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

    public void TakeDamage(int damage)
    {
        if (IsEmpty())
        {
            return;
        }

        bool isDead = axieCharacter.TakeDamage(damage);
        if (isDead)
        {
            GameManager.Instance.HexMap.RemoveAxieCache(axieCharacter.Team, axieCharacter);
            RemoveCharacter(true);
        }
    }

    public int GetRandomNumber()
    {
        //Debug.Log(Q + "," + R);
        if (IsEmpty())
        {
            return 0;
        }

        return axieCharacter.RandomNumber;
    }

    public void SetCharacter(AxieController axie)
    {
        if (IsEmpty())
        {
            axieCharacter = axie;
            //axie.SetStandingHex(this);
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
