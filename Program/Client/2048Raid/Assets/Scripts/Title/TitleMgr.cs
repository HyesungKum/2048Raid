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
        //Scene 시작 시 Logo fade 연출 작동
        StartCoroutine(FadeProduction(2, 0.02f));

        //인트로 로고 종료 이벤트에 타이틀 오브젝트 동작 연결
        IntroLogoDone += TitleActivate;
    }

    private void OnDestroy()
    {
        IntroLogoDone -= TitleActivate;
    }

    private void TitleActivate()
    {
        //타이틀 오브젝트 전체 활성화
        TitleObj.SetActive(true);

        //UI에 표시할 정보 입력
        UIController.VersionTmp.text = "Version "+Version;
        UIController.CopyRightTmp.text = CopyRight;

        //UI의 화면전환 동작
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
