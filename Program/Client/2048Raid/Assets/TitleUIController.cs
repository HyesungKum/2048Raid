using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleUIController : MonoBehaviour
{
    public TextMeshProUGUI VersionTmp = null;
    public TextMeshProUGUI CopyRightTmp = null;

    public Button StartButton = null;

    //=====================================================================
    //
    // transition property
    //
    public Image TransitionImage = null;
    public GameObject TrsObj = null;
    public float TransitionSpeed = 4f;
    public bool IsTransitionDone { get; private set; }
    public Action TransitionDone;

    private Material transitionMat = null;
    private WaitUntil WaitTrsMat = null;


    private void Start()
    {
        WaitTrsMat = new WaitUntil(() => transitionMat != null);
        IsTransitionDone = false;

        if(TransitionImage != null)
        {
            //�ʱ� Transition ���׸��� ����
            transitionMat = TransitionImage.material;
            transitionMat.SetFloat("_TrValue", -1);
        }


        StartButton.onClick.AddListener(() => SceneMove());
    }

    private void OnDestroy()
    {
        TransitionDone -= () => SceneManager.LoadScene("MainScene");
        StartButton.onClick.RemoveAllListeners();
    }

    public void SceneMove()
    {
        //ȭ����ȯ ���� �Ϸ� �̺�Ʈ�� Scene �ҷ����� �Լ� ����
        //���Ŀ� additive�� Scene�ҷ�����
        TransitionDone += () => SceneManager.LoadScene("MainScene");
        DoTransition();
    }

    public void DoTransition() =>StartCoroutine(Transition());
    private IEnumerator Transition()
    {
        yield return WaitTrsMat;

        float trsProgress;

        //������ ����
        if (IsTransitionDone)
        {
            TrsObj.SetActive(true);
            transitionMat.SetFloat("_TrValue", 10);
            trsProgress = transitionMat.GetFloat("_TrValue");

            do
            {
                trsProgress -= Time.deltaTime * TransitionSpeed;
                transitionMat.SetFloat("_TrValue", trsProgress);
                yield return null;
            } while (trsProgress > -1);

            IsTransitionDone = false;
        }
        //������ ����
        else
        {
            TrsObj.SetActive(true);
            transitionMat.SetFloat("_TrValue", -1);
            trsProgress = transitionMat.GetFloat("_TrValue");

            do
            {
                trsProgress += Time.deltaTime * TransitionSpeed;
                transitionMat.SetFloat("_TrValue", trsProgress);
                yield return null;
            } while (trsProgress < 10);

            IsTransitionDone = true;
            TrsObj.SetActive(false);
        }

        TransitionDone?.Invoke();
    }
}
