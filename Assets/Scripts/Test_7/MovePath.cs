using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MovePath : MonoBehaviour
{
	private const string MOVE_ID = "MOVE";
	public Transform[] Points;

	// Use this for initialization
	void Start ()
	{
		Vector3[] postions = Points.Select(i => i.position).ToArray();
		transform
			.DOPath(postions, 2)
			.SetOptions(true)
			.SetLookAt(0)
			.SetLoops(-1)
			.SetEase(Ease.Linear)
			.SetId(MOVE_ID);
	}

	public void Pause()
	{
		DOTween.Pause(MOVE_ID);
	}

	public void Continue()
	{
		DOTween.Play(MOVE_ID);
	}
}
