using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleContorller : MonoBehaviour
{
    public GameObject TitleObj;

    public TitleUIController UIController;

    public SpriteRenderer LogoRenderer = null;

    public Action IntroLogoDone;

    public string Version;
    public string CopyRight;

    private void Start()
    {
        //Scene ���� �� Logo fade ���� �۵�
        StartCoroutine(FadeProduction(2, 0.02f));

        //��Ʈ�� �ΰ� ���� �̺�Ʈ�� Ÿ��Ʋ ������Ʈ ���� ����
        IntroLogoDone += TitleActivate;
    }

    private void OnDestroy()
    {
        IntroLogoDone -= TitleActivate;
    }

    private void TitleActivate()
    {
        //Ÿ��Ʋ ������Ʈ ��ü Ȱ��ȭ
        TitleObj.SetActive(true);

        //UI�� ǥ���� ���� �Է�
        UIController.VersionTmp.text = "Version "+Version;
        UIController.CopyRightTmp.text = CopyRight;

        //UI�� ȭ����ȯ ����
        UIController.DoTransition();
    }

    /// <summary>
    /// controlled fade in and out production
    /// </summary>
    /// <param name="duration">time for each production</param>
    /// <param name="smoothness">fading smoothness</param>
    /// <returns></returns>
    IEnumerator FadeProduction(float duration, float smoothness) 
    {
        float progress = 0;
        float increment = smoothness/duration;

        do
        {
            LogoRenderer.color = Color.Lerp(LogoRenderer.color, Color.white, progress);
            progress += increment;

            yield return new WaitForSeconds(smoothness);
        } while (progress < 1);

        progress = 0;
        increment = smoothness / duration;
        do
        {
            LogoRenderer.color = Color.Lerp(LogoRenderer.color, Color.clear, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        } while (progress < 1);

        IntroLogoDone?.Invoke();

        LogoRenderer.gameObject.SetActive(false);
    }
}
