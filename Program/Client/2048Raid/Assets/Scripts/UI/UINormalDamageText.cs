using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINormalDamageText : UIDamageText {

	public AnimationCurve YoffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 40.0f) });
	public AnimationCurve XoffsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 40.0f) });
	public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f) });
	public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f) });

	public float Speed = 1f;//배속

	Keyframe[] _yoffsets;
	Keyframe[] _xoffsets;
	Keyframe[] _scales;
	Keyframe[] _alphas;

	float _yoffsetEnd = 0;
	float _xoffsetEnd = 0;
	float _scalesEnd = 0;
	float _alphaEnd = 0;
	float _totalEnd = 0;
	float _curTime = 0;
	float _scaleValue = 0;
	TextInfo _txtInfo = null;

	Color _color = Color.white;

	void Start()
	{
        MaxTextCount = 20;
        TextInfo textInfo;

		// 최대 화면에 보여질 갯수 만큼 미리 생성해서 재활용
		for (int i = 0; i < MaxTextCount; ++i)    
        {
            textInfo = createTextInfo(true);
            deleteTextInfo(textInfo);
        }
    }

	/// <summary>
	/// 텍스트 연출 업데이트
	/// </summary>
	protected override void updateTextInfo()
	{
		_yoffsets = YoffsetCurve.keys;
		_xoffsets = XoffsetCurve.keys;
		_scales = ScaleCurve.keys;
		_alphas = AlphaCurve.keys;

        _yoffsetEnd = _yoffsets[_yoffsets.Length - 1].time;
		_xoffsetEnd = _xoffsets[_xoffsets.Length - 1].time;
        _scalesEnd = _scales[_scales.Length - 1].time;
        _alphaEnd = _alphas[_alphas.Length - 1].time;
		_totalEnd = Mathf.Max(_yoffsetEnd, _scalesEnd, _alphaEnd);

		for (int i = _usedTextInfoList.Count - 1; i >= 0; --i)
		{
			_txtInfo = _usedTextInfoList[i];
            _curTime = (Time.realtimeSinceStartup - _txtInfo.AddedTime) * Speed;

			// offset
			_txtInfo.Yoffset = YoffsetCurve.Evaluate(_curTime) * _txtInfo.MoveFactor;			
			_txtInfo.Xoffset = XoffsetCurve.Evaluate(_curTime) * _txtInfo.MoveFactor;
			// scale
			_scaleValue = ScaleCurve.Evaluate(_curTime);
			if (_scaleValue < 0.001f) _scaleValue = 0.001f;
			_txtInfo.Text.transform.localScale = Vector3.one * _scaleValue;
			// alpha
			_color = _txtInfo.Text.color;
			_color.a = AlphaCurve.Evaluate(_curTime);
			_txtInfo.Text.color = _color;

			// end
			if (_curTime > _totalEnd)
				deleteTextInfo(_txtInfo);
			else
			{
				_txtInfo.Text.enabled = true;
			}
		}

		// offset
		for (int i = _usedTextInfoList.Count - 1; i >= 0; --i)
		{
			_txtInfo = _usedTextInfoList[i];

			_txtInfo.Text.transform.localPosition = _txtInfo.StartPos + Vector3.up * _txtInfo.Yoffset + Vector3.right * _txtInfo.Xoffset;
		}
	}
}
