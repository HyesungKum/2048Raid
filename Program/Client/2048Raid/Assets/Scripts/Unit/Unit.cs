using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 유닛 메인 태그 <br/>
/// 아군,적군,보스 확인 가능
/// </summary>
public enum MAINTAG
{
    NONE,
    ALLIY,
    ENEMY,
    BOSS
}

/// <summary>
/// 해당 유닛이 가지는 특수한 상태 태그
/// </summary>
public enum EXTAG
{
    NONE,
    COMBINABLE
}

[Serializable]
/// <summary>
/// 유닛의 상호작용을 위한 태그 클래스
/// </summary>
public class UnitTag
{
    [SerializeField] MAINTAG mTag = new();
    [SerializeField] List<EXTAG> exTag = new();

    /// <summary>
    /// 메인 태그값을 확인
    /// </summary>
    public MAINTAG GetMainTag => mTag;
    /// <summary>
    /// 추가 태그값들을 확인
    /// </summary>
    public List<EXTAG> GetExtags => exTag;

    //====================================================================================
    //
    // 태그 컨트를 메서드
    //
    /// <summary>
    /// 메인 태그 획득
    /// </summary>
    /// <param name="AcquiredTag">목표 태그</param>
    public void Acquire(MAINTAG AcquiredTag)
    {
        if (mTag == AcquiredTag) return;

        mTag = AcquiredTag;
    }
    /// <summary>
    /// 추가 태그 획득을 위해 사용
    /// </summary>
    /// <param name="AcquiredTag">목표 태그</param>
    public void Acquire(EXTAG AcquiredTag)
    {
        //중복취득 방지
        foreach (EXTAG tag in exTag)
        {
            if (AcquiredTag == tag)
            {
#if UNITY_EDITOR
                Debug.Log($"Unit Extra Tag 중복 시도");
#endif
                return;
            } 
        }

        //태그 획득
        exTag.Add(AcquiredTag);
    }

    /// <summary>
    /// 요구하는 메인 태그가 있는지 확인
    /// </summary>
    /// <param name="targetTag"></param>
    /// <returns></returns>
    public bool Equal(MAINTAG targetTag)
    {
        if (mTag == targetTag) return true;
        return false;
    }
    /// <summary>
    /// 요구하는 추가 태그가 있는지 확인
    /// </summary>
    /// <param name="targetTag">목표 태그</param>
    /// <returns></returns>
    public bool Equal(EXTAG targetTag)
    {
        foreach (EXTAG tag in exTag)
        {
            if (targetTag == tag)
            {
                //원하는 태그 있음
                return true;
            }
        }
        //원하는 태그 없음
        return false;
    }

    /// <summary>
    /// 원하는 태그 삭제
    /// </summary>
    /// <param name="targetTag">목표 태그</param>
    public void ClearTargetTag(EXTAG targetTag)
    {
        exTag.Remove(targetTag);
    }
    /// <summary>
    /// 저장된 전체 태그 삭제
    /// </summary>
    public void ClearAllTag()
    {
        exTag.Clear();
    }
}

/// <summary>
/// 보드위에서 존재하는 유닛 클래스
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [SerializeField] private SpriteRenderer UnitSprite;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("부여된 태그")]
    public UnitTag unitTag;

    [Header("2048 값")]
    private int value;

    [Header("캐릭터 성능")]
    private float maxHp = 1;
    private float hp = 1;
    private float damage = 1;

    //====================================================================================
    //
    // Unity 라이프 사이클
    //
    private void OnEnable() => valueDisplayReset();

    //====================================================================================
    //
    // 이벤트
    //
    /// <summary>
    /// 유닛의 이동이 완료되면 발생하는 이벤트
    /// </summary>
    public Action MoveDone;

    //====================================================================================
    //
    // 값 접근 및 조건 확인
    //
    /// <summary>
    /// 현재 유닛의 값을 가져온다
    /// </summary>
    public int GetValue() => value;
    /// <summary>
    /// 원하는 유닛과 합성 가능한지 판단한다
    /// </summary>
    /// <param name="unit">비교 유닛</param>
    /// <returns></returns>
    public bool Combinable(Unit unit)
    {
        //합성 태그가 없으면 합성 불가
        if (!unitTag.Equal(EXTAG.COMBINABLE)) return false;

        //체력이 0 이하면 합성 불가능
        if (hp <= 0) return false;

        if (this.unitTag.Equal(unit.unitTag.GetMainTag))
        {
            //메인 태그가 동일하다면 합성
            if (this.value == unit.value)
            {
                return true;
            }
        }

        return false;
        //메인 태그가 다르다면 각자에 맞는 상황
    }

    //====================================================================================
    //
    // 유닛 컨트롤
    //
    /// <summary>
    /// 유닛의 값을 새로이 할당하고 값을 재 디스플레이 한다
    /// </summary>
    /// <param name="value">목표 값</param>
    public void SetValue(int value)
    {
        this.value = value;
        valueDisplayReset();
    }
    /// <summary>
    /// 현재 유닛의 체력을 세팅한다
    /// </summary>
    /// <param name="requireHp">목표 체력</param>
    public void SetHp(float requireHp)
    {
        maxHp = requireHp;
        hp = maxHp;

        hpDisplayReset();
    }
    /// <summary>
    /// 현재 체력에 영향을 주지 않는 최대 체력 세팅
    /// </summary>
    /// <param name="requireHP">목표 최대 체력</param>
    public void SetMaxHp(float requireHP)
    {
        maxHp = requireHP;
        hpDisplayReset();
    }
    /// <summary>
    /// 유닛의 생김새를 세팅한다
    /// </summary>
    /// <param name="spritePath">목표 스프라이트 경로</param>
    public void SetSprite(string spritePath)
    {
        Sprite sp = Resources.Load<Sprite>(spritePath);
        UnitSprite.sprite = sp;
    }

    /// <summary>
    /// 유닛을 강화하며 새로운 스프라이트 할당
    /// </summary>
    /// <param name="newValue">값</param>
    public void Upgrade()
    {
        SetValue(this.value * 2);
        SetMaxHp(100);
        UnitSprite.sprite = UnitPicker.Inst.SpritePicker(this.unitTag);
    }
    /// <summary>
    /// 원하는 노드로 원하는 시간에 걸쳐 이동한다
    /// </summary>
    /// <param name="node">목표 노드</param>
    /// <param name="time">목표 시간</param>
    public void MoveToNode(Node node, float time)
    {
        if (node == null) return;
        StartCoroutine(move(node.transform.position, time));
        node.AddUnit(this);
    }
    /// <summary>
    /// 이동 연출 코루틴 동작을 완료하면 이벤트를 발생 시킨다.
    /// </summary>
    /// <param name="targetPos">목표 vector</param>
    /// <param name="time">목표 시간</param>
    /// <returns></returns>
    private IEnumerator move(Vector3 targetPos, float time)
    {
        float speed = Vector3.Distance(this.transform.position, targetPos) / time;

        while (this.transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            yield return null;
        }

        MoveDone?.Invoke();
    }

    /// <summary>
    /// 유닛을 오브젝트 풀에 다시 집어 넣는다.
    /// </summary>
    public void Despawn()
    {
        PAObjectPoolSingleton.Inst.Despawn(this.gameObject);
        MoveDone = null;
    }

    //====================================================================================
    //
    // 디스플레이 컨트롤
    //
    /// <summary>
    /// 값 표시를 다시 시도한다
    /// </summary>
    private void valueDisplayReset()
    {
        valueText.text = $"{value}";
    }

    private void hpDisplayReset()
    {
        
    }
}
