using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보드 이동 명령 방향
/// </summary>
public enum DIR
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

/// <summary>
/// 보드 상태
/// </summary>
public enum BOARDSTATE
{
    READY,
    MOVING,
    COMBINE,
    BUFF,
    DEBUFF,
    INTERECT,
    RALLY,
    BOSSTURN,
    REINFORCE,
    BUSTCHECK,
    FOODBUST,
    GAMEOVER
}

/// <summary>
/// 합성유닛 정보 컨테이너
/// </summary>
public struct CombinePack
{
    /// <summary>
    /// 남아있는 유닛
    /// </summary>
    public Unit AliveUnit;
    /// <summary>
    /// 삭제되는 유닛
    /// </summary>
    public Unit DeletUnit;
    /// <summary>
    /// 생성될 위치
    /// </summary>
    public Node TargetNode;

    public CombinePack(Unit aliveUnit, Unit deletUnit, Node targetNode)
    {
        this.AliveUnit = aliveUnit;
        this.DeletUnit = deletUnit;
        this.TargetNode = targetNode;
    }
}

public class Board : MonoBehaviour
{
    public Node[,] AllNodes = null;
    private List<Node> emptyNodes = new();

    private Node[] RstartNodes = new Node[4];
    private Node[] LstartNodes = new Node[4];
    private Node[] UstartNodes = new Node[4];
    private Node[] DstartNodes = new Node[4];

    private Node[] TempNodes = new Node[4];

    private List<Unit> lineInfos = new();
    private List<CombinePack> combinePacks = new();

    [SerializeField]private int contorlledMovingUnitCount = 0;
    [SerializeField] private int contorlledInterUnitCount = 0;

    [Header("노드 부모 지정")]
    [SerializeField] Transform nodeGroup = null;

    [Header("[보드 옵션]")]
    public int Size;
    public float MovingTime;

    [Header("[보드 상태]")]
    [SerializeField] private BOARDSTATE curState;
    [SerializeField] private DIR dirOrdered;
    [Space]
    public int CurFood = 0;
    public int MaxFood = 10;
    [Space]
    [SerializeField] private int enemyApproachCount;
    [Space]
    [SerializeField] public bool IsSetted = false;
    [SerializeField] public bool IsFull;

    private WaitForSeconds waitTick = new WaitForSeconds(0.1f);

    //==================================================================================
    //
    // 보드 이벤트
    //
    public Action CamShakeEvent;

    //==================================================================================
    //
    // 초기화 구문
    //
    /// <summary>
    /// 보드 초기화
    /// </summary>
    public void BoardReset()
    {
        //등록된 노드들이 초기에 없다면 생성
        if (AllNodes == null) CreateNodes();
        else//있다면 노드 초기화
        {
            foreach (Node node in AllNodes)
            {
                node.DelClear();
            }
        }

        //상태 변경
        IsFull = false;
        CurFood = 2;
        MaxFood = 10;
        contorlledMovingUnitCount = 0;
        contorlledInterUnitCount = 0;
        enemyApproachCount = 3;

        //보드 시작
        StartCoroutine(startBoard());
    }
    /// <summary>
    /// 초기 노드 생성
    /// </summary>
    public void CreateNodes()
    {
        //과거에 있었던 자식 노드들을 삭제
        foreach (Node obj in nodeGroup.GetComponentsInChildren<Node>())
        {
            DestroyImmediate(obj.gameObject);
        }

        //관리 메모리 할당
        AllNodes = new Node[Size, Size];
        lineInfos = new();

        //새로운 노드 생성
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                GameObject obj = new GameObject("Node");
                obj.transform.SetParent(nodeGroup.transform);
                obj.AddComponent(typeof(Node));
                obj.AddComponent(typeof(RectTransform));
                obj.AddComponent(typeof(SpriteRenderer));

                //관리 등록
                AllNodes[y, x] = obj.GetComponent<Node>();
            }
        }

        //노드 이웃 연결
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                //좌표 저장
                AllNodes[y, x].XOrder = x;
                AllNodes[y, x].YOrder = y;

                //이웃 노드 연결
                if (x != 0) AllNodes[y, x].LNode = AllNodes[y, x - 1];
                if (x != Size - 1) AllNodes[y, x].RNode = AllNodes[y, x + 1];
                if (y != 0) AllNodes[y, x].UNode = AllNodes[y - 1, x];
                if (y != Size - 1) AllNodes[y, x].DNode = AllNodes[y + 1, x];

            }
        }

        for (int i = 0; i < Size; i++)
        {
            LstartNodes[i] = AllNodes[i, Size - 1];
            RstartNodes[i] = AllNodes[i, 0];
            UstartNodes[i] = AllNodes[0, i];
            DstartNodes[i] = AllNodes[Size - 1, i];
        }

        //셋팅 변수 적용
        IsSetted = true;
    }

    //==================================================================================
    //
    // 보드 조작
    //
    /// <summary>
    /// 보드들의 유닛에게 이동 명령
    /// </summary>
    /// <param name="direction">목표 방향</param>
    public void MoveOrder(DIR direction)
    {
        //방향 명령 저장
        dirOrdered = direction;

        //한줄씩 검사 시작
        for (int i = 0; i < Size; i++)
        {
            //라인 정보 초기화 후
            lineInfos.Clear();

            //검사 시작 노드 선택
            Node targetNode = null;
            switch (direction)
            {
                case DIR.LEFT: targetNode = AllNodes[i, 0]; break;
                case DIR.RIGHT: targetNode = AllNodes[i, Size - 1]; break;
                case DIR.DOWN: targetNode = AllNodes[Size - 1, i]; break;
                case DIR.UP: targetNode = AllNodes[0, i]; break;
            }

            //노드안에 들어있는 유닛 검사 후 라인 정보에 저장
            do
            {
                if (!targetNode.IsEmpty)
                {
                    lineInfos.Add(targetNode.RefUnit);
                    targetNode.UnitClear();
                }

                switch (direction)
                {
                    case DIR.LEFT: targetNode = targetNode.RNode; break;
                    case DIR.RIGHT: targetNode = targetNode.LNode; break;
                    case DIR.DOWN: targetNode = targetNode.UNode; break;
                    case DIR.UP: targetNode = targetNode.DNode; break;
                }

            } while (targetNode != null);

            //라인 정보에 저장된 유닛 상호작용 및 이동 검사
            if (lineInfos.Count == 0) continue;//라인 정보가 없다면 넘기기

            Node destinationNode = null;
            Unit aheadUnit = null;

            //라인 정보에 저장된 순서대로 상호작용 검사
            for (int j = 0; j < lineInfos.Count; j++)
            {
                Unit targetUnit = lineInfos[j];
                if (aheadUnit == null)//첫 검사되는 유닛이라면
                {
                    switch (direction)
                    {
                        case DIR.LEFT: destinationNode = AllNodes[i, j]; break;
                        case DIR.RIGHT: destinationNode = AllNodes[i, Size - 1 - j]; break;
                        case DIR.DOWN: destinationNode = AllNodes[Size - 1 - j, i]; break;
                        case DIR.UP: destinationNode = AllNodes[j, i]; break;
                    }
                }
                else
                {
                    if (targetUnit.Combinable(aheadUnit))//앞서 합성 가능한 유닛이 있다면
                    {
                        if (destinationNode.IsBlocked)//합성가능하지만 이미 노드에 2명이상 진입한 상태라면
                        {
                            switch (direction)
                            {
                                case DIR.LEFT: destinationNode = destinationNode.RNode; break;
                                case DIR.RIGHT: destinationNode = destinationNode.LNode; break;
                                case DIR.DOWN: destinationNode = destinationNode.UNode; break;
                                case DIR.UP: destinationNode = destinationNode.DNode; break;
                            }
                        }
                        else
                        {
                            CombinePack pack = new(targetUnit, aheadUnit, destinationNode);
                            combinePacks.Add(pack);
                        }
                    }
                    else//합성 불가능한 유닛이 앞에 있다면
                    {
                        switch (direction)
                        {
                            case DIR.LEFT: destinationNode = destinationNode.RNode; break;
                            case DIR.RIGHT: destinationNode = destinationNode.LNode; break;
                            case DIR.DOWN: destinationNode = destinationNode.UNode; break;
                            case DIR.UP: destinationNode = destinationNode.DNode; break;
                        }
                    }
                }

                //이동 명령
                targetUnit.MoveToNode(destinationNode, MovingTime);

                //이전 유닛 정보 저장
                aheadUnit = targetUnit;
            }
        }
    }
    /// <summary>
    /// 증원, 새로운 유닛을 노드에 추가한다
    /// </summary>
    public void RequestReinforce()
    {
        emptyNodes.Clear();

        //빈 노드 확인
        foreach (Node node in AllNodes)
        {
            if (node.IsEmpty) emptyNodes.Add(node);
        }

        //빈 노드가 없을 시 증원 없음
        if (emptyNodes.Count == 0)
        {
            IsFull = true;
            return;
        }
        else IsFull = false;

        //랜덤으로 증원 위치 결정
        int targetIdx = UnityEngine.Random.Range(0, emptyNodes.Count);
        Node targetNode = emptyNodes[targetIdx];

        //유닛 선택
        Unit NewUnit = null;
        if (enemyApproachCount == 0)
        {
            //적군 침입
            NewUnit = UnitPicker.Inst.PickMonster();
            enemyApproachCount = 3;
        }
        else
        {
            //아군 지원
            NewUnit = UnitPicker.Inst.PickBaseUnit();
            enemyApproachCount--;

            //식량 증원
            foodCome();
        }

        //노드에 유닛 등록
        targetNode.AddUnit(NewUnit);

        //위치 조정
        NewUnit.transform.position = targetNode.transform.position;

        //유닛 이벤트 연결
        NewUnit.MoveDone = reactMove;
        NewUnit.InterDone = reactInter; 
    }

    //====================================================================================
    //
    // 보드 상태 머신
    //
    /// <summary>
    /// 보드의 상태머신을 실행함
    /// </summary>
    /// <returns></returns>
    private IEnumerator startBoard()
    {
        while (!IsSetted) yield return null;

        yield return waitTick;

        changeState(BOARDSTATE.REINFORCE);
    }

    /// <summary>
    /// 현재 State Machine의 상태를 변경한다
    /// </summary>
    /// <param name="state">목표 상태</param>
    private void changeState(BOARDSTATE state)
    {
        if (curState == state) return;

        StopCoroutine(curState.ToString() + "STATE");
        curState = state;
        StartCoroutine(state.ToString() + "STATE");
    }
    /// <summary>
    /// 입력 대기
    /// </summary>
    /// <returns></returns>
    private IEnumerator READYSTATE()
    {
        while (true)
        {
            if (TouchMgr.Inst.GetDragUp(out DIR dir))
            {
                MoveOrder(dir);
                changeState(BOARDSTATE.MOVING);
            }
            yield return null;
        }
    }
    /// <summary>
    /// 모든 이동 명령
    /// </summary>
    /// <returns></returns>
    private IEnumerator MOVINGSTATE()
    {
        CurFood--;

        //관리되는 유닛 전부가 움직일 때 까지 대기
        do
        {
            yield return null;
        } while (contorlledMovingUnitCount != 0);

        changeState(BOARDSTATE.COMBINE);
    }
    /// <summary>
    /// 모든 노드들의 합성 작업
    /// </summary>
    /// <returns></returns>
    private IEnumerator COMBINESTATE()
    {
        yield return null;

        //저장된 합성 팩을 확인하여 합성 작업을 진행
        foreach (CombinePack packData in combinePacks)
        {
            //피합성체 삭제
            packData.DeletUnit.Despawn();
            
            //남은 대상 강화
            packData.AliveUnit.Upgrade();

            //노드에 유닛 등록
            packData.TargetNode.ClearInCount();
            packData.TargetNode.AddUnit(packData.AliveUnit);
        }

        //사용한 합성 팩 초기화
        combinePacks.Clear();

        changeState(BOARDSTATE.INTERECT);
    }

    private IEnumerator INTERECTSTATE()
    {
        yield return null;

        contorlledInterUnitCount = 0;

        //검사 노드를 레퍼런스에서 복사 후 사용
        switch (dirOrdered)
        {
            case DIR.RIGHT:
                {
                    for (int i = 0; i < Size; i++)
                    {
                        TempNodes[i] = LstartNodes[i];
                    }
                } break;
            case DIR.LEFT:
                {
                    for (int i = 0; i < Size; i++)
                    {
                        TempNodes[i] = RstartNodes[i];
                    }
                } break;
            case DIR.UP:
                {
                    for (int i = 0; i < Size; i++)
                    {
                        TempNodes[i] = UstartNodes[i];
                    }
                } break;
            case DIR.DOWN:
                {
                    for (int i = 0; i < Size; i++)
                    {
                        TempNodes[i] = DstartNodes[i];
                    }
                } break;
        }

        do
        {
            for (int i = 0; i < Size; i++)
            {
                Node node = TempNodes[i];

                //현제 노드에 동작 가능한 유닛없다면 넘기기
                if (node.RefUnit == null) continue;

                //다음 노드가 없다면 넘기기
                Node nextNode = null;
                switch (dirOrdered)
                {
                    case DIR.RIGHT: nextNode = node.RNode; break;
                    case DIR.LEFT: nextNode = node.LNode; break;
                    case DIR.UP: nextNode = node.UNode; break;
                    case DIR.DOWN: nextNode = node.DNode; break;
                }
                if (nextNode == null) continue;

                //유닛이 없다면 넘기기
                if (nextNode.RefUnit == null) continue;

                //상호작용
                contorlledInterUnitCount++;
                node.RefUnit.Interect(nextNode.RefUnit);
            }

            //관리되는 유닛 전부가 움직일 때 까지 대기
            do
            {
                yield return null;
            } while (contorlledInterUnitCount != 0);

            //확인 노드 이동
            for (int i = 0; i < Size; i++)
            {
                switch (dirOrdered)
                {
                    case DIR.RIGHT: TempNodes[i] = TempNodes[i].LNode; break;
                    case DIR.LEFT: TempNodes[i] = TempNodes[i].RNode; break;
                    case DIR.UP: TempNodes[i] = TempNodes[i].DNode; break;
                    case DIR.DOWN: TempNodes[i] = TempNodes[i].UNode; break;
                }
            }

        } while (TempNodes[0] != null);

        changeState(BOARDSTATE.RALLY);
    }

    /// <summary>
    /// 집결 시 동작
    /// </summary>
    /// <returns></returns>
    private IEnumerator RALLYSTATE()
    {
        yield return null;

        if (dirOrdered == DIR.DOWN)
        {
            foreach (Node node in AllNodes)
            {
                if (node.IsEmpty) continue;

                if (node.RefUnit.unitTag.Equal(MAINTAG.ALLIY))
                {
                    node.RefUnit.Rally();
                }
            }
        }

        changeState(BOARDSTATE.REINFORCE);
    }

    /// <summary>
    /// 증원 작업
    /// </summary>
    /// <returns></returns>
    private IEnumerator REINFORCESTATE()
    {
        yield return null;

        //증원 요청
        RequestReinforce();

        //관리 유닛 개수를 0으로 한 뒤
        contorlledMovingUnitCount = 0;

        //새로이 관리되는 유닛 개수를 확인한다
        foreach (Node node in AllNodes)
        {
            if (!node.IsEmpty) contorlledMovingUnitCount++;
        }

        changeState(BOARDSTATE.BUSTCHECK);
    }
    /// <summary>
    /// 게임오버 조건 체크
    /// </summary>
    /// <returns></returns>
    private IEnumerator BUSTCHECKSTATE()
    {
        yield return null;

        //게임 오버 조건 확인
        if (CurFood <= 0) changeState(BOARDSTATE.GAMEOVER);
        else changeState(BOARDSTATE.READY);
    }
    /// <summary>
    /// 게임 오버되어 리셋 및 결과창 출력
    /// </summary>
    /// <returns></returns>
    private IEnumerator GAMEOVERSTATE()
    {
        Debug.Log("식량이 전부 고갈되었습니다!");

        while (true)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.R))
            {
                BoardReset();
                yield break;
            }
        }
    }

    //====================================================================================
    //
    // 이벤트
    //
    /// <summary>
    /// 유닛들의 움직임에 대응
    /// </summary>
    private void reactMove()
    {
        contorlledMovingUnitCount--;
    }

    /// <summary>
    /// 유닛들의 각 상호작용에 대응
    /// </summary>
    private void reactInter()
    {
        contorlledInterUnitCount--;
    }

    private void foodCome()
    {
        if (CurFood > MaxFood - 3)
        {
            //최대 식량 넘지 못함
            CurFood = MaxFood;
        }
        else CurFood += 3;
    }
}
