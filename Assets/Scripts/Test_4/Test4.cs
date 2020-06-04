using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Test4 : MonoBehaviour
{
    public int xGridCount, yGridCount, zGridCount;
    private Vector3[] _vertices;
    private int[] _triangles;
    private int _t = 0, _v = 0;

    // Use this for initialization
    void Start()
    {
        Mesh mesh = new Mesh();

        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        mesh.name = "Mesh04";

        StartCoroutine(GenerateMesh(mesh));
        
    }

    private IEnumerator GenerateMesh(Mesh mesh)
    {
        yield return StartCoroutine(GenerateVextex());
        mesh.vertices = _vertices;
        yield return StartCoroutine(GenerateTriange(mesh));
    }

    private int GetVextexCount()
    {
        int connerVertexCount = 8;
        int edgeVertexCount = (xGridCount + yGridCount + zGridCount - 3) * 4;
        int faceVertexCount = (xGridCount - 1) * (yGridCount - 1) * 2
                              + (zGridCount - 1) * (yGridCount - 1) * 2
                              + (xGridCount - 1) * (zGridCount - 1) * 2;

        return connerVertexCount + edgeVertexCount + faceVertexCount;
    }

    private IEnumerator GenerateVextex()
    {
        _vertices = new Vector3[GetVextexCount()];
        int index = 0;

        for (int y = 0; y < yGridCount + 1; y++)
        {
            for (int x = 0; x < xGridCount + 1; x++, index++)
            {
                _vertices[index] = new Vector3(x, y, 0);
                yield return null;
            }

            for (int z = 1; z < zGridCount + 1; z++, index++)
            {
                _vertices[index] = new Vector3(xGridCount, y, z);
                yield return null;
            }

            for (int x = xGridCount - 1; x >= 0; x--, index++)
            {
                _vertices[index] = new Vector3(x, y, zGridCount);
                yield return null;
            }

            for (int z = zGridCount - 1; z > 0; z--, index++)
            {
                _vertices[index] = new Vector3(0, y, z);
                yield return null;
            }
        }

        for (int z = 1; z < zGridCount; z++)
        {
            for (int x = 1; x < xGridCount; x++, index++)
            {
                _vertices[index] = new Vector3(x, yGridCount, z);
                yield return null;
            }
        }

        for (int z = 1; z < zGridCount; z++)
        {
            for (int x = 1; x < xGridCount; x++, index++)
            {
                _vertices[index] = new Vector3(x, 0, z);
                yield return null;
            }
        }
    }

    private int GetTriangeCount()
    {
        return xGridCount * yGridCount * 4 + xGridCount * zGridCount * 4 + yGridCount * zGridCount * 4;
    }

    private IEnumerator GenerateTriange(Mesh mesh)
    {
        _triangles = new int[GetTriangeCount() * 3];
        int circleVextexCount = 2 * xGridCount + 2 * zGridCount;
       

        yield return StartCoroutine(GenerateSide(mesh, circleVextexCount));

        yield return StartCoroutine(GenerateTop(mesh, circleVextexCount));

        yield return StartCoroutine(GenerateBottom(mesh, circleVextexCount));
    }

    private IEnumerator GenerateSide(Mesh mesh,int circleVextexCount)
    {
        for (int y = 0; y < yGridCount; y++)
        {
            for (int i = 0; i < circleVextexCount; i++,_v++,_t += 6)
            {
                SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount,_v+1+circleVextexCount,circleVextexCount);
                mesh.triangles = _triangles;
                yield return null;
            }
        }
    }

    private IEnumerator GenerateTop(Mesh mesh,int circleVextexCount)
    {
        //第一行前三个面
        for (int x = 0; x < xGridCount - 1; x++,_v++,_t += 6)
        {
            SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount-1,_v+circleVextexCount);
            mesh.triangles = _triangles;
            yield return null;
        }
        
        //第一行第四个面
        SetQuad(_triangles,_t,_v,_v+1,_v+circleVextexCount-1,_v+2);
        mesh.triangles = _triangles;
        _t += 6;
        yield return null;

        int vMin = circleVextexCount * (yGridCount + 1) - 1;
        int vMid = vMin + 1;
        int vMax = _v + 2;
        //中间行所有面
        for (int z = 0; z < zGridCount - 2; z++,vMin--,vMid++,vMax++)
        {
            //第一个面
            SetQuad(_triangles,_t,vMin,vMid,vMin - 1,vMid + xGridCount);
            mesh.triangles = _triangles;
            _t += 6;
            yield return null;

            //中间面片
            for (int i = 0; i < xGridCount - 2; i++,_t += 6,vMid++)
            {
                SetQuad(_triangles,_t,vMid,vMid+1,vMid +xGridCount - 1,vMid +xGridCount);
                mesh.triangles = _triangles;
                yield return null;
            }
        
            //最后一个面
            SetQuad(_triangles,_t,vMid,vMax,vMid + xGridCount -1,vMax + 1);
            mesh.triangles = _triangles;
            _t += 6;
            yield return null;
        }

        int vTop = vMin - 2;
        //第一个面
        SetQuad(_triangles,_t,vMin,vMid,vMin - 1,vTop);
        mesh.triangles = _triangles;
        _t += 6;
        yield return null;

        
        for (int i = 0; i < xGridCount - 2; i++,vMid++,vTop--)
        {
            //中间面
            SetQuad(_triangles,_t,vMid,vMid + 1,vTop,vTop -1 );
            mesh.triangles = _triangles;
            _t += 6;
            yield return null;
        }
        
        //最后一行最后一个面
        SetQuad(_triangles,_t,vMid,vTop - 2,vTop,vTop - 1);
        mesh.triangles = _triangles;
        _t += 6;
        yield return null;
    }

    private IEnumerator GenerateBottom(Mesh mesh, int circleVextexCount)
    {
        int vMin = circleVextexCount - 1;
        int vMid = _vertices.Length - (xGridCount - 1) * (zGridCount - 1);
        
        //第一行，第一面
        SetQuad(_triangles,_t,vMin,vMid,0,1);
        _t += 6;
        mesh.triangles = _triangles;
        yield return null;

        int vMax = 1;
        //第一行，中间面
        for (int i = 0; i < xGridCount - 2; i++, _t += 6,vMax++,vMid++)
        {
            SetQuad(_triangles,_t,vMid,vMid + 1,vMax,vMax + 1);
            mesh.triangles = _triangles;
            yield return null;
        }
        
        //第一行，最后面
        SetQuad(_triangles,_t,vMid,vMax + 2,vMax,vMax + 1);
        _t += 6;
        mesh.triangles = _triangles;
        yield return null;

        vMid++;
        vMax += 2;
        
        for (int z = 0; z < zGridCount - 2; z++,vMin --,vMid++,vMax ++)
        {
            //第一面
            SetQuad(_triangles,_t,vMin - 1,vMid,vMin,vMid - xGridCount + 1);
            _t += 6;
            mesh.triangles = _triangles;
            yield return null;
            
            //中间面
            for (int i = 0; i < xGridCount - 2; i++, _t += 6,vMid++)
            {
                SetQuad(_triangles,_t,vMid,vMid + 1, vMid - xGridCount + 1,vMid - xGridCount + 2);
                mesh.triangles = _triangles;
                yield return null;
            }
            
            //最后面
            SetQuad(_triangles,_t,vMid,vMax + 1,vMid - xGridCount + 1,vMax);
            _t += 6;
            mesh.triangles = _triangles;
            yield return null;
        }

        vMid = vMid - xGridCount + 1;
        //最后行，第一面
        SetQuad(_triangles,_t,vMin - 1,vMin - 2,vMin,vMid);
        _t += 6;
        mesh.triangles = _triangles;
        yield return null;

        int vBottom = vMin - 2;
        //最后行，中间面
        for (int i = 0; i < xGridCount - 2; i++, _t += 6,vBottom--,vMid++)
        {
            SetQuad(_triangles,_t,vBottom,vBottom - 1,vMid,vMid + 1);
            mesh.triangles = _triangles;
            yield return null;
        }

        //最后行，最后面
        SetQuad(_triangles,_t,vBottom,vBottom - 1,vMid,vBottom - 2);
        _t += 6;
        mesh.triangles = _triangles;
        yield return null;
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

    private void OnDrawGizmos()
    {
        if (_vertices == null)
            return;

        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]), 0.1f);
        }
    }
}