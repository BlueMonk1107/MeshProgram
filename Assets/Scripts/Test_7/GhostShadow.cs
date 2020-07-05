using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadow : MonoBehaviour {
	private Dictionary<int,GhostShadowItem> _items = new Dictionary<int, GhostShadowItem>();

	public void Init(int id,Material material,Shader shader,Action<GhostShadow> complete)
	{
		GameObject go = new GameObject();
		var item = go.AddComponent<GhostShadowItem>();
		go.transform.SetParent(transform);
		_items.Add(id,item);
		item.Init(material,shader,()=>complete(this));
	}

	public void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}

	public void UpdateMesh(int id,Mesh mesh,Vector3 pos,Quaternion rot)
	{
		if (!_items.ContainsKey(id))
		{
			Debug.LogError("当前ID不存在，ID："+id);
			return;
		}
		_items[id].UpdateMesh(mesh,pos,rot);
	}
}
