using UnityEngine;

public class UnitPicker : Singleton<UnitPicker>
{
    string baseUnitPath = "Prefabs/Unit";
    string baseUnitSprite = "Sprite/BaseUnit";
    string monsterSprite = "Sprite/RockMonster";

    /// <summary>
    /// 기본 증원 유닛을 생성하여 반환한다
    /// </summary>
    /// <returns>기본 증원 유닛</returns>
    public Unit PickBaseUnit()
    {
        //오브젝트 생성
        GameObject newUnitObj = PAObjectPoolSingleton.Inst.Spawn(baseUnitPath);
        Unit newUnit = newUnitObj.GetComponent<Unit>();

        //태그 관리
        newUnit.unitTag.ClearAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ALLIY);
        newUnit.unitTag.Acquire(EXTAG.COMBINABLE);

        //값 세팅
        newUnit.SetSprite(baseUnitSprite);
        newUnit.SetValue(2);
        newUnit.SetHp(10);

        return newUnit;
    }

    /// <summary>
    /// 랜덤한 몬스터를 반환한다
    /// </summary>
    /// <returns>몬스터 유닛</returns>
    public Unit PickMonster()
    {
        // 랜덤한 유닛 정보를 datamgr을 통해서 불러온다
        float hp = 10;

        //오브젝트 생성
        GameObject newUnitObj = PAObjectPoolSingleton.Inst.Spawn(baseUnitPath);
        Unit newUnit = newUnitObj.GetComponent<Unit>();

        newUnit.unitTag.ClearAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ENEMY);

        //값 세팅
        newUnit.SetSprite(monsterSprite);
        newUnit.SetValue(2);
        newUnit.SetHp(hp);

        return newUnit;
    }

    /// <summary>
    /// 태그에 맞는 랜덤한 스프라이트를 획득한다
    /// </summary>
    /// <param name="tag">현재 유닛태그</param>
    /// <returns></returns>
    public Sprite SpritePicker(UnitTag tag)
    {
        switch (tag.GetMainTag)
        {
            case MAINTAG.ALLIY: return Resources.Load<Sprite>(baseUnitSprite);
            case MAINTAG.ENEMY: return Resources.Load<Sprite>(monsterSprite);
            default: return null;
        }
    }
}
