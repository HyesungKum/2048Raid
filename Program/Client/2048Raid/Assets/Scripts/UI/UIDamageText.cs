using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public abstract class UIDamageText : MonoBehaviour {

	protected class TextInfo
	{
		public float AddedTime = 0.0f;
		public float Yoffset = 0.0f;
		public float Xoffset = 0.0f;
		public Vector3 StartPos;
		public TextMeshProUGUI Text;
		public Shadow TextShadow;
        public float MoveFactor = 1.0f;
	}	

	public TMP_FontAsset TextFont;
	public int FontSize = 15;
	public FontStyles FontStyle = FontStyles.Normal;
	public Color FontColor = Color.white;
	public Color ShadowColor = Color.white;
    public Color EnemyFontColor = Color.white;
    public Color EnemyShadowColor = Color.white;
    public Vector2 ShadowDistance = Vector2.zero;
	public Color OutlineColor = Color.white;
	public Vector2 OutlineDistance = Vector2.zero;

	protected int MaxTextCount = 20;

	// for pool
	protected List<TextInfo> _usedTextInfoList = new();
	protected List<TextInfo> _unusedTextInfoList = new();

	int _namecount = 0;		

	void OnDisable()
	{
		foreach (TextInfo ti in _usedTextInfoList)
		{
			if (ti.Text != null)
				ti.Text.enabled = false;
		}
		_usedTextInfoList.Clear();
	}

	void Update()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		//데미지 텍스트들의 연출을 업데이트한다
		updateTextInfo();
	}

	/// <summary>
	/// 업데이트 될 연출 작업
	/// </summary>
	protected virtual void updateTextInfo()
	{
#if UNITY_EDITOR
		Debug.Log("##[Info]update TextInfo");
#endif
	}

	/// <summary>
	/// 데미지 텍스트 제작을 요청한다
	/// </summary>
	/// <param name="value">데미지 값</param>
	/// <param name="pos">시작 위치</param>
	/// <param name="isEnemy">요청 객체 타입</param>
	/// <param name="moveFactor">이동 계수</param>
	public void Make(float value, Vector3 pos, bool isEnemy = false, float moveFactor = 1.0f)
	{
		if (!enabled) return;

		makeDamageStr(value.ToString(), pos, isEnemy, moveFactor);
	}
	/// <summary>
	/// 스킬에 의한 데미지 텍스트 제작을 요청한다.
	/// </summary>
	/// <param name="skillName">스킬 이름</param>
	/// <param name="value">데미지 값</param>
	/// <param name="pos">시작 위치</param>
	/// <param name="isEnemy">요청 객체 타입</param>
	/// <param name="moveFactor">이동 계수</param>
	public void Make(string skillName, float value, Vector3 pos, bool isEnemy = false, float moveFactor = 1.0f)
	{
		if (!enabled) return;

		makeDamageStr($"<size=20>{skillName}</size>\n{value}", pos, isEnemy, moveFactor);
	}

	/// <summary>
	/// 문자열 변환 적용 데미지 텍스트 제작
	/// </summary>
	/// <param name="valueStr">변환된 문자열</param>
	/// <param name="pos">시작 위치</param>
	/// <param name="isEnemy">요청 객체 타입</param>
	/// <param name="moveFactor">이동 계수</param>
	private void makeDamageStr(string valueStr, Vector3 pos, bool isEnemy = false, float moveFactor = 1.0f)
    {
        if (!enabled) return;

        // 새로운 데미지 덱스트 생성
        TextInfo txtInfo = createTextInfo();

		// 데이터 세팅
        txtInfo.StartPos = pos;
        txtInfo.Text.text = valueStr;
        txtInfo.Text.transform.localPosition = txtInfo.StartPos;

        if (isEnemy == false)
        {
            txtInfo.Text.color = FontColor;
            txtInfo.TextShadow.effectColor = ShadowColor;

        }
        else
        {
            txtInfo.Text.color = EnemyFontColor;
            txtInfo.TextShadow.effectColor = EnemyShadowColor;
        }

        txtInfo.MoveFactor = moveFactor;
    }
	/// <summary>
	/// 데미지 텍스트 객체를 초기화 하여 생성한다
	/// </summary>
	/// <param name="forced">forced이면 재활용하지 않고 새로 생성한다</param>
	/// <returns>만들어진 데미지 텍스트</returns>
	protected TextInfo createTextInfo(bool forced = false)
	{
		if (forced == false)
		{
			if (_unusedTextInfoList.Count > 0)
			{
				TextInfo ti = _unusedTextInfoList[_unusedTextInfoList.Count - 1];
				_unusedTextInfoList.RemoveAt(_unusedTextInfoList.Count - 1);
				ti.AddedTime = Time.realtimeSinceStartup;
				ti.Yoffset = 0f;
				ti.Xoffset = 0f;
				ti.Text.transform.localScale = Vector3.zero;
				ti.Text.gameObject.SetActive(true);

				_usedTextInfoList.Add(ti);
				return ti;
			}
			else if (_usedTextInfoList.Count > MaxTextCount)
			{
				TextInfo ti = _usedTextInfoList[0];
				_usedTextInfoList.RemoveAt(0);
				ti.AddedTime = Time.realtimeSinceStartup;
				ti.Yoffset = 0f;
				ti.Xoffset = 0f;
				ti.Text.transform.localScale = Vector3.zero;

				_usedTextInfoList.Add(ti);
				return ti;
			}
		}

		// Create New TextInfo
		TextInfo txtInfo = new TextInfo();
		txtInfo.AddedTime = Time.realtimeSinceStartup;

		txtInfo.Text = new GameObject(_namecount.ToString(), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
		txtInfo.Text.transform.SetParent(this.transform);
		txtInfo.Text.rectTransform.anchoredPosition3D = Vector3.zero;
		txtInfo.Text.rectTransform.sizeDelta = new Vector2(1000, 1000);
		//txtInfo.MyText.transform.localScale = Vector3.zero;
		// font
		txtInfo.Text.font = TextFont;
		txtInfo.Text.fontSize = FontSize;
		txtInfo.Text.fontStyle = FontStyle;
		txtInfo.Text.color = FontColor;
		txtInfo.Text.alignment = TextAlignmentOptions.Midline;
		txtInfo.Text.lineSpacing = 0.8f;
		txtInfo.Text.raycastTarget = false;
		// outline
		Outline outline = txtInfo.Text.gameObject.AddComponent<Outline>();
		outline.effectColor = OutlineColor;
		outline.effectDistance = OutlineDistance;
		// shadow
		txtInfo.TextShadow = txtInfo.Text.gameObject.AddComponent<Shadow>();
		txtInfo.TextShadow.effectColor = ShadowColor;
		txtInfo.TextShadow.effectDistance = ShadowDistance;

		_usedTextInfoList.Add(txtInfo);
		++_namecount;
		return txtInfo;
	}
	/// <summary>
	/// 데미지 텍스트 제거후 풀링
	/// </summary>
	/// <param name="ent">제거할 데미지 텍스트</param>
	protected void deleteTextInfo(TextInfo ent)
	{
		_usedTextInfoList.Remove(ent);
		_unusedTextInfoList.Add(ent);

		ent.Text.gameObject.SetActive(false);

	}

}

