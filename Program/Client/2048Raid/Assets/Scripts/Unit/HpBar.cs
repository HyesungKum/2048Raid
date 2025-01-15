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
    /// 체력바 값의 변동에 대응 
    /// </summary>
    /// <param name="curHp">현재 체력</param>
    /// <param name="maxHp">최대 체력</param>
    public void CallValueChange(double curHp, double maxHp)
    {
        //진짜 값 변동
        realSlider.value = (float)(curHp / maxHp);

        //연출
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
