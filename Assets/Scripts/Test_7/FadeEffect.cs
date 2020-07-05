using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
	private Action _onComplete;
	private MeshRenderer _renderer;
	private Material _material;
	private Color _color;
	private float _timer;
	private bool _isPlaying;

	public void StartEffect(Action complete)
	{
		_onComplete = complete;
		_timer = 0;

		if (_renderer == null)
			_renderer = GetComponent<MeshRenderer>();

		if (_renderer == null || _renderer.material == null)
		{
			_isPlaying = false;
			gameObject.name = "Error Gameobject";
			Debug.LogError("当前物体没有MeshRenderer或没有材质，物体名称："+gameObject.name);
			return;
		}
		else
		{
			_isPlaying = true;
			_material = _renderer.material;
			_color = new Color32(25,25,25,255);
			SetColor(_color);
		}
	}

	private void Update()
	{
		if(!_isPlaying)
			return;
		
		_timer += Time.deltaTime;

		if (_timer < ShadowData.EXIST_TIME)
		{
			float alpha = (ShadowData.EXIST_TIME - _timer) / ShadowData.EXIST_TIME;
			_color.a = alpha;
			SetColor(_color);
		}
		else
		{
			Hide();
		}
	}

	private void SetColor(Color32 color)
	{
		_material.SetColor("_TintColor", color);
	}

	private void Hide()
	{
		_isPlaying = false;
		if (_onComplete != null)
			_onComplete();

		_onComplete = null;
	}
}
