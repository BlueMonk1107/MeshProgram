using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadowItem : MonoBehaviour
{
	private MeshFilter _filter;
	private MeshRenderer _renderer;
	private FadeEffect _effect;
	private Action _onComplete;

	public void Init(Material material,Shader shader,Action complete)
	{
		if(_filter == null)
			_filter = gameObject.AddComponent<MeshFilter>();

		if (_renderer == null)
			_renderer = gameObject.AddComponent<MeshRenderer>();

		if (_effect == null)
			_effect = gameObject.AddComponent<FadeEffect>();

		_renderer.material = material;
		if(shader != null)
			_renderer.material.shader = shader;
		_onComplete = complete;
	}

	public void UpdateMesh(Mesh mesh, Vector3 pos, Quaternion rot)
	{
		if (_filter == null)
		{
			Debug.LogError("请先调用Init方法");
			return;
		}

		_filter.mesh = mesh;
		transform.position = pos;
		transform.rotation = rot;
		
		_effect.StartEffect(_onComplete);
	}
}
