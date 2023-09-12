using UnityEngine;

/// <summary>
/// 노드 상태
/// </summary>
public enum NODESTATE
{
    NONE,
    WARNING,
    DAMAGEAREA,
    BUFFAREA,
    DEBUFFAREA,
}

/// <summary>
/// 유닛들이 위치하게 되는 노드
/// </summary>
public class Node : MonoBehaviour
{
    [Header("노드에 올라와 있는 유닛")]
    public Unit RefUnit = null;

    [Header("이웃 노드")]
    public Node RNode = null;
    public Node LNode = null;
    public Node UNode = null;
    public Node DNode = null;

    [Header("노드 상태")]
    public NODESTATE State = NODESTATE.NONE;
    [Space]
    public int XOrder;
    public int YOrder;
    [SerializeField] private int inCount = 0;

    /// <summary>
    /// 해당 노드에 유닛이 있는지
    /// </summary>
    public bool IsEmpty
    {
        get 
        {
            return RefUnit == null;
        }
    }
    /// <summary>
    /// 해당노드에 두명 이상 진입한 경우 
    /// </summary>
    public bool IsBlocked
    {
        get { return inCount == 2; }
    }

    //====================================================================================
    //
    // 노드 컨트롤 메서드
    //
    /// <summary>
    /// 진입한 수를 초기화 한다
    /// </summary>
    public void ClearInCount()
    {
        this.inCount = 0;
    }

    /// <summary>
    /// 노드에 유닛 들어옴
    /// </summary>
    /// <param name="targetUnit">목표 유닛</param>
    public void AddUnit(Unit targetUnit)
    {
        //노드 접근 카운트 증가
        inCount++;
        //참조
        RefUnit = targetUnit;
    }

    /// <summary>
    /// 노드 참조 유닛 초기화
    /// </summary>
    public void UnitClear()
    {
        //상태 초기화
        inCount = 0;
        RefUnit = null;
    }

    /// <summary>
    /// 노드 유닛 삭제 후 초기화
    /// </summary>
    public void DelClear()
    {
        if (RefUnit != null)
        {
            //유닛 삭제
            DestroyImmediate(RefUnit.gameObject);
            RefUnit = null;
        }

        //상태 초기화
        this.State = NODESTATE.NONE;
    }
}
