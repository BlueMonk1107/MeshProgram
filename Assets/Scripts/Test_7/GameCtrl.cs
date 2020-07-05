using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
	private GhostShadowMgr _shadowMgr;
	private MovePath _move;

	// Use this for initialization
	void Start ()
	{
		_move = GetComponent<MovePath>();
		_shadowMgr = GetComponent<GhostShadowMgr>();
		_shadowMgr.Init();
		StartCoroutine(ProcessCtrl());
	}


	private IEnumerator ProcessCtrl()
	{
		while (true)
		{
			Execute(HumanState.MOVE);
			yield return new WaitForSeconds(3);
			Execute(HumanState.IDLE);
			yield return new WaitForSeconds(2);
		}
	}

	private void Execute(HumanState state)
	{
		switch (state)
		{
			case HumanState.IDLE:
				_shadowMgr.SetSpawnState(SpawnState.DISENABLE);
				_move.Pause();
				break;
			case HumanState.MOVE:
				_shadowMgr.SetSpawnState(SpawnState.ENABLE);
				_move.Continue();
				break;
			default:
				throw new ArgumentOutOfRangeException("state", state, null);
		}
	}
}

public enum HumanState
{
	IDLE,
	MOVE
}
