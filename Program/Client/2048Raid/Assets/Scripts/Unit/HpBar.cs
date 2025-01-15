using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Slider followSlider; /*production slider*/
    [SerializeField] private Slider realSlider;   /*real value slider*/
    [Range(0.0f, 5.0f)]
    [SerializeField] private float prodSpeed = 3f;

    /// <summary>
    /// ü�¹� ���� ������ ���� 
    /// </summary>
    /// <param name="curHp">���� ü��</param>
    /// <param name="maxHp">�ִ� ü��</param>
    public void CallValueChange(double curHp, double maxHp)
    {
        //��¥ �� ����
        realSlider.value = (float)(curHp / maxHp);

        //����
        StopAllCoroutines();
        StartCoroutine(decreaseProd((float)(curHp / maxHp)));
    }

    /// <summary>
    /// follow slider value folloing to real value
    /// </summary>
    /// <param name="value">requie value</param>
    /// <returns></returns>
    private IEnumerator decreaseProd(float value)
    {
        while (true)
        {
            followSlider.value = Mathf.Lerp(followSlider.value, value, Time.deltaTime * prodSpeed);

            if (followSlider.value == value) yield break;

            yield return null;
        }
    }
}
