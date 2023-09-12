using UnityEngine;

public class UnitPicker : Singleton<UnitPicker>
{
    string baseUnitPath = "Prefabs/Unit";
    string baseUnitSprite = "Sprite/BaseUnit";
    string monsterSprite = "Sprite/RockMonster";

    /// <summary>
    /// �⺻ ���� ������ �����Ͽ� ��ȯ�Ѵ�
    /// </summary>
    /// <returns>�⺻ ���� ����</returns>
    public Unit PickBaseUnit()
    {
        //������Ʈ ����
        GameObject newUnitObj = PAObjectPoolSingleton.Inst.Spawn(baseUnitPath);
        Unit newUnit = newUnitObj.GetComponent<Unit>();

        //�±� ����
        newUnit.unitTag.ClearAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ALLIY);
        newUnit.unitTag.Acquire(EXTAG.COMBINABLE);

        //�� ����
        newUnit.SetSprite(baseUnitSprite);
        newUnit.SetValue(2);
        newUnit.SetHp(10);

        return newUnit;
    }

    /// <summary>
    /// ������ ���͸� ��ȯ�Ѵ�
    /// </summary>
    /// <returns>���� ����</returns>
    public Unit PickMonster()
    {
        // ������ ���� ������ datamgr�� ���ؼ� �ҷ��´�
        float hp = 10;

        //������Ʈ ����
        GameObject newUnitObj = PAObjectPoolSingleton.Inst.Spawn(baseUnitPath);
        Unit newUnit = newUnitObj.GetComponent<Unit>();

        newUnit.unitTag.ClearAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ENEMY);

        //�� ����
        newUnit.SetSprite(monsterSprite);
        newUnit.SetValue(2);
        newUnit.SetHp(hp);

        return newUnit;
    }

    /// <summary>
    /// �±׿� �´� ������ ��������Ʈ�� ȹ���Ѵ�
    /// </summary>
    /// <param name="tag">���� �����±�</param>
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
