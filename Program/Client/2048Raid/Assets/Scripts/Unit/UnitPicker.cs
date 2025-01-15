using UnityEngine;

public class UnitPicker : Singleton<UnitPicker>
{
    string baseUnitPath = "Prefabs/Unit";
    string baseUnitSprite = "Sprite/BaseUnit";
    string monsterSprite = "Sprite/RockMonster";

    string alliyIcon = "Sprite/AlliyIcon";
    string enemyIcon = "Sprite/EnemyIcon";
    string bossIcon = "Sprite/BossIcon";

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
        newUnit.unitTag.DeleteAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ALLIY);
        newUnit.unitTag.Acquire(EXTAG.COMBINABLE);

        //�� ����
        newUnit.SetObj(baseUnitSprite);
        newUnit.SetIcon(alliyIcon);
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

        newUnit.unitTag.DeleteAllTag();
        newUnit.unitTag.Acquire(MAINTAG.ENEMY);

        //�� ����
        newUnit.SetObj(monsterSprite);
        newUnit.SetIcon(enemyIcon);
        newUnit.SetValue(2);
        newUnit.SetHp(hp);

        return newUnit;
    }

    /// <summary>
    /// �±׿� �´� ������ ��������Ʈ�� ȹ���Ѵ�
    /// </summary>
    /// <param name="tag">���� �����±�</param>
    /// <returns></returns>
    public GameObject ObjPicker(UnitTag tag)
    {
        switch (tag.GetMainTag)
        {
            case MAINTAG.ALLIY: return Resources.Load<GameObject>(baseUnitSprite);
            case MAINTAG.ENEMY: return Resources.Load<GameObject>(monsterSprite);
            default: return null;
        }
    }
}
