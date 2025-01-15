using UnityEngine;

public enum DAMAGETYPE
{
	COMMON,
	CRITICAL
}

public class DamageTextMgr : MonoBehaviourSingleton<DamageTextMgr>
{
	[Header("[프리팹]")]
	[SerializeField] UINormalDamageText m_normalDamageTextPrefab = null;
	[SerializeField] UICriticalDamageText m_criticalDamageTextPrefab = null;
	
    UINormalDamageText m_normalDamageText = null;
    UICriticalDamageText m_criticalDamageText = null;
    
	RectTransform m_rectTransformCanvas = null;

	/// <summary>
	/// 게임 내에 데미지텍스트를 출력할지 여부
	/// </summary>
	public bool IsShowDamageText { get; set; } = true;

	#region singleton 세팅
	protected override void InitInAwake()
	{
        if (_initialized) return;

		m_rectTransformCanvas = transform.root.GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

		// 노멀
		if (m_normalDamageTextPrefab != null)
        {
			m_normalDamageText = Instantiate<UINormalDamageText>(m_normalDamageTextPrefab, Vector3.zero, Quaternion.identity, this.transform);
			m_normalDamageText.transform.localPosition = Vector3.zero;

		}
		// 크리티컬
		if (m_criticalDamageTextPrefab != null)
        {
            m_criticalDamageText = Instantiate<UICriticalDamageText>(m_criticalDamageTextPrefab, Vector3.zero, Quaternion.identity, this.transform);
            m_criticalDamageText.transform.localPosition = Vector3.zero;
        }

		_initialized = true;
	}
	protected override void DestroyInSingleton()
	{
		if (m_normalDamageText != null)
		{
			Destroy(m_normalDamageText);
			m_normalDamageText = null;
		}		
		if (m_criticalDamageText != null)
        {
            Destroy(m_criticalDamageText);
            m_criticalDamageText = null;
        }

		StopAllCoroutines();
	}
    #endregion

	/// <summary>
	/// 화면에 뿌려줄 데미지 텍스트를 요청한다
	/// </summary>
	/// <param name="damage">데미지 값</param>
	/// <param name="pos">출력 위치</param>
	/// <param name="damageType">데미지 타입(일반,크리티컬)</param>
	/// <param name="isEnemy">요청하는 객체 타입</param>
    public void ShowDamageText(float damage, Vector3 pos, DAMAGETYPE damageType, bool isEnemy = false)
	{
		if (damageType == DAMAGETYPE.COMMON)
			addNormalDamageText(damage, pos, isEnemy);
		else
			addCriticalDamageText(damage, pos, isEnemy);
	}

	/// <summary>
	/// 일반 데미지 텍스트를 생성한다
	/// </summary>
	/// <param name="damage">데미지 값</param>
	/// <param name="pos">표시 월드 위치</param>
	/// <param name="isEnemy">요청하는 객체 타입</param>
	private void addNormalDamageText(float damage, Vector3 pos, bool isEnemy = false)
	{
		// 게임옵션에 따라 출력을 결정
		if (IsShowDamageText == false)
			return;

		if (m_normalDamageText != null)
			m_normalDamageText.Make(damage, get2DPosFrom3DPos(pos), isEnemy);
	}
	/// <summary>
	/// 크리티컬 데미지 텍스트를 생성한다
	/// </summary>
	/// <param name="damage">데미지 값</param>
	/// <param name="pos">데미지 월드 좌표</param>
	/// <param name="isEnemy">요청하는 객체의 타입</param>
	public void addCriticalDamageText(float damage, Vector3 pos, bool isEnemy = false)
    {
		// 게임옵션에 따라 출력을 결정
		if (IsShowDamageText == false)
			return;

		if (m_criticalDamageText != null)
            m_criticalDamageText.Make(damage, get2DPosFrom3DPos(pos), isEnemy);
    }
	/// <summary>
	/// 월드 좌표를 캔버스상의 좌표로 전환한다
	/// </summary>
	/// <param name="pos3D">월드 3D상의 좌표</param>
	/// <returns></returns>
	Vector2 get2DPosFrom3DPos(Vector3 pos3D)
	{
        if (Camera.main == null) return Vector3.zero;

		Vector2 viewportPos = Camera.main.WorldToViewportPoint(pos3D);
		return new Vector2(
			((viewportPos.x * m_rectTransformCanvas.sizeDelta.x) - (m_rectTransformCanvas.sizeDelta.x * 0.5f)),
			((viewportPos.y * m_rectTransformCanvas.sizeDelta.y) - (m_rectTransformCanvas.sizeDelta.y * 0.5f))
			);
	}
}

