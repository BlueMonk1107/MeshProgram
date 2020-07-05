using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadowMgr : MonoBehaviour
{
    private List<GhostShadow> _activeList;
    private List<GhostShadow> _inactiveList;
    private SkinnedMeshRenderer[] _renderers;
    private float _lastTime;
    private SpawnState _state;
    
    // Use this for initialization
    public void Init()
    {
        _activeList = new List<GhostShadow>();
        _inactiveList = new List<GhostShadow>();
        _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if(_renderers == null)
            _renderers = new SkinnedMeshRenderer[0];
        _state = SpawnState.DISENABLE;
    }

    // Update is called once per frame
    void Update()
    {
        if (JudgeState() && Time.time - _lastTime > ShadowData.SPAWN_INTERVAL_TIME)
        {
            _lastTime = Time.time;
            Spwan();
        }
    }

    public void SetSpawnState(SpawnState state)
    {
        _state = state;
    }

    private bool JudgeState()
    {
        return _state == SpawnState.ENABLE;
    }

    public void Spwan()
    {
        GhostShadow item = null;
        if (_inactiveList.Count > 0)
        {
            item = _inactiveList[0];
            _inactiveList.Remove(item);
        }
        else
        {
            item = SpawnNew();
            InitShadow(item);
        }

        item.SetActive(true);
        UpdateShadow(item);
        _activeList.Add(item);
    }

    private void InitShadow(GhostShadow item)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            item.Init(i,_renderers[i].material,GetShader(),Despwan);
        }
    }

    private Shader GetShader()
    {
        return Shader.Find(ShadowData.SHADER_NAME);
    }

    private GhostShadow SpawnNew()
    {
        GameObject go = new GameObject();
        return go.AddComponent<GhostShadow>();
    }

    private void UpdateShadow(GhostShadow item)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            Mesh mesh = new Mesh();
            _renderers[i].BakeMesh(mesh);
            item.UpdateMesh(i,mesh,_renderers[i].transform.position,_renderers[i].transform.rotation);
        }
    }

    private void Despwan(GhostShadow item)
    {
        _activeList.Remove(item);
        _inactiveList.Add(item);
        item.SetActive(false);
    }
}

public class ShadowData
{
    /// <summary>
    /// 当前虚影使用的shader名称
    /// </summary>
    public const string SHADER_NAME = "Particles/Additive";
    /// <summary>
    /// 虚影存在时间
    /// </summary>
    public const float EXIST_TIME = 1;
    /// <summary>
    /// 生成虚影的间隔时间
    /// </summary>
    public const float SPAWN_INTERVAL_TIME = 0.2f;
}

public enum SpawnState
{
    ENABLE,
    DISENABLE
}