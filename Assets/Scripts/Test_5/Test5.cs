using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Test5 : MonoBehaviour {

	public int _diameterSize;
    public float _unitDistace;
    private Vector3[] _vertices;
    private int[] _triangles;
    private int _t = 0, _v = 0;
    private float _r;
    private Vector3[] _normals;

    // Use this for initialization
    void Awake()
    {
        Mesh mesh = new Mesh();

        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        mesh.name = "Mesh05";

        _r = _diameterSize * _unitDistace / 2;

        GenerateMesh(mesh);

        gameObject.AddComponent<SphereCollider>();
    }

    private void GenerateMesh(Mesh mesh)
    {
        GenerateVextex();

        _normals = new Vector3[_vertices.Length];
        Vector3 inner;
        for (int i = 0; i < _vertices.Length; i++)
        {
            inner = SetNormal(i);
            ResetVextexPos(i,inner);
        }
        
        mesh.vertices = _vertices;
        
        GenerateTriange(mesh);
    }

    private int GetVextexCount()
    {
        int connerVertexCount = 8;
        int edgeVertexCount = (_diameterSize + _diameterSize + _diameterSize - 3) * 4;
        int faceVertexCount = (_diameterSize - 1) * (_diameterSize - 1) * 2
                              + (_diameterSize - 1) * (_diameterSize - 1) * 2
                              + (_diameterSize - 1) * (_diameterSize - 1) * 2;

        return connerVertexCount + edgeVertexCount + faceVertexCount;
    }

    private void GenerateVextex()
    {
        _vertices = new Vector3[GetVextexCount()];
        int index = 0;

        for (int y = 0; y < _diameterSize + 1; y++)
        {
            for (int x = 0; x < _diameterSize + 1; x++, index++)
            {
                _vertices[index] = GetVertexPos(x, y, 0);
            }

            for (int z = 1; z < _diameterSize + 1; z++, index++)
            {
                _vertices[index] = GetVertexPos(_diameterSize, y, z);
            }

            for (int x = _diameterSize - 1; x >= 0; x--, index++)
            {
                _vertices[index] = GetVertexPos(x, y, _diameterSize);
            }

            for (int z = _diameterSize - 1; z > 0; z--, index++)
            {
                _vertices[index] = GetVertexPos(0, y, z);
            }
        }

        for (int z = 1; z < _diameterSize; z++)
        {
            for (int x = 1; x < _diameterSize; x++, index++)
            {
                _vertices[index] = GetVertexPos(x, _diameterSize, z);
            }
        }

        for (int z = 1; z < _diameterSize; z++)
        {
            for (int x = 1; x < _diameterSize; x++, index++)
            {
                _vertices[index] = GetVertexPos(x, 0, z);
            }
        }
    }

    private Vector3 GetVertexPos(int xSize, int ySize,int zSize)
    {
        return new Vector3(xSize,ySize,zSize) * _unitDistace;
    }

    private int GetTriangeCount()
    {
        return _diameterSize * _diameterSize * 4 + _diameterSize * _diameterSize * 4 + _diameterSize * _diameterSize * 4;
    }

    private void GenerateTriange(Mesh mesh)
    {
        _triangles = new int[GetTriangeCount() * 3];
        int circleVextexCount = 2 * _diameterSize + 2 * _diameterSize;


        GenerateSide(mesh, circleVextexCount);

        GenerateTop(mesh, circleVextexCount);

        GenerateBottom(mesh, circleVextexCount);

        mesh.triangles = _triangles;
    }

    private void GenerateSide(Mesh mesh,int circleVextexCount)
    {
        for (int y = 0; y < _diameterSize; y++)
        {
            for (int i = 0; i < circleVextexCount; i++,_v++,_t += 6)
            {
                SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount,_v+1+circleVextexCount,circleVextexCount);
            }
        }
    }

    private void GenerateTop(Mesh mesh,int circleVextexCount)
    {
        //第一行前三个面
        for (int x = 0; x < _diameterSize - 1; x++,_v++,_t += 6)
        {
            SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount-1,_v+circleVextexCount);
        }
        
        //第一行第四个面
        SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount-1,_v+2);
        _t += 6;

        int vMin = circleVextexCount * (_diameterSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = _v + 2;
        //中间行所有面
        for (int z = 0; z < _diameterSize - 2; z++,vMin--,vMid++,vMax++)
        {
            //第一个面
            SetQuad(_triangles,_t,vMin,vMid,vMin - 1,vMid + _diameterSize);
            _t += 6;

            //中间面片
            for (int i = 0; i < _diameterSize - 2; i++,_t += 6,vMid++)
            {
                SetQuad(_triangles,_t,vMid,vMid+1,vMid +_diameterSize - 1,vMid +_diameterSize);
            }
        
            //最后一个面
            SetQuad(_triangles,_t,vMid,vMax,vMid + _diameterSize -1,vMax + 1);
            _t += 6;
        }

        int vTop = vMin - 2;
        //第一个面
        SetQuad(_triangles,_t,vMin,vMid,vMin - 1,vTop);
        _t += 6;

        
        for (int i = 0; i < _diameterSize - 2; i++,vMid++,vTop--)
        {
            //中间面
            SetQuad(_triangles,_t,vMid,vMid + 1,vTop,vTop -1 );
            _t += 6;
        }
        
        //最后一行最后一个面
        SetQuad(_triangles,_t,vMid,vTop - 2,vTop,vTop - 1);
        _t += 6;
    }

    private void GenerateBottom(Mesh mesh, int circleVextexCount)
    {
        int vMin = circleVextexCount - 1;
        int vMid = _vertices.Length - (_diameterSize - 1) * (_diameterSize - 1);
        
        //第一行，第一面
        SetQuad(_triangles,_t,vMin,vMid,0,1);
        _t += 6;

        int vMax = 1;
        //第一行，中间面
        for (int i = 0; i < _diameterSize - 2; i++, _t += 6,vMax++,vMid++)
        {
            SetQuad(_triangles,_t,vMid,vMid + 1,vMax,vMax + 1);
        }
        
        //第一行，最后面
        SetQuad(_triangles,_t,vMid,vMax + 2,vMax,vMax + 1);
        _t += 6;

        vMid++;
        vMax += 2;
        
        for (int z = 0; z < _diameterSize - 2; z++,vMin --,vMid++,vMax ++)
        {
            //第一面
            SetQuad(_triangles,_t,vMin - 1,vMid,vMin,vMid - _diameterSize + 1);
            _t += 6;
            
            //中间面
            for (int i = 0; i < _diameterSize - 2; i++, _t += 6,vMid++)
            {
                SetQuad(_triangles,_t,vMid,vMid + 1, vMid - _diameterSize + 1,vMid - _diameterSize + 2);
            }
            
            //最后面
            SetQuad(_triangles,_t,vMid,vMax + 1,vMid - _diameterSize + 1,vMax);
            _t += 6;
        }

        vMid = vMid - _diameterSize + 1;
        //最后行，第一面
        SetQuad(_triangles,_t,vMin - 1,vMin - 2,vMin,vMid);
        _t += 6;

        int vBottom = vMin - 2;
        //最后行，中间面
        for (int i = 0; i < _diameterSize - 2; i++, _t += 6,vBottom--,vMid++)
        {
            SetQuad(_triangles,_t,vBottom,vBottom - 1,vMid,vMid + 1);
        }

        //最后行，最后面
        SetQuad(_triangles,_t,vBottom,vBottom - 1,vMid,vBottom - 2);
        _t += 6;
    }

    private void SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11,int circleVextexCount)
    {
        v10 = (v00 / circleVextexCount) * circleVextexCount + v10 % circleVextexCount;
        v11 = (v01 / circleVextexCount) * circleVextexCount + v11 % circleVextexCount;

        SetQuad(triangles, i, v00, v10, v01, v11);
    }
    
    private void SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = v01;
        triangles[i + 2] = v10;

        triangles[i + 3] = v01;
        triangles[i + 4] = v11;
        triangles[i + 5] = v10;
    } 
    
    //圆角四边形部分逻辑

    private Vector3 SetNormal(int i)
    {
        var vextex = _vertices[i];
        var inner = new Vector3(_r,_r,_r);

        _normals[i] = (vextex - inner).normalized;

        return inner;
    }

    private void ResetVextexPos(int i,Vector3 inner)
    {
        _vertices[i] = _normals[i] * _r + inner;
    }
}
