using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MeshData
{
    private StringBuilder _data;

    private int _vertexOffset;

    public MeshData(MeshFilter[] filters)
    {
        _vertexOffset = 0;
        _data = new StringBuilder();

        foreach (MeshFilter filter in filters)
        {
            SaveMeshData(_data, filter);
        }
    }

    public override string ToString()
    {
        return _data.ToString();
    }

    private void SaveMeshData(StringBuilder data,MeshFilter filter)
    {
        //保存网格名称
        SaveGroupData(data, filter);
        //保存顶点数据
        SaveVertices(data, filter);
        //保存法线数据
        SaveNormals(data,filter);
        //保存uv数据
        SaveUVs(data, filter);
        SaveMaterialsAndFace(data, filter);
    }

    private void SaveGroupData(StringBuilder data,MeshFilter filter)
    {
        data.Append("g ")
            .Append(filter.mesh.name)
            .Append("\n");
    }

    private void SaveVertices(StringBuilder data,MeshFilter filter)
    {
        foreach (Vector3 vector3 in filter.mesh.vertices)
        {
            Vector3 wPos = filter.transform.TransformPoint(vector3);
            data.Append("v ")
                .Append(-wPos.x)
                .Append(" ")
                .Append(wPos.y)
                .Append(" ")
                .Append(wPos.z)
                .Append("\n");
        }

        data.Append("\n");
    }

    private void SaveNormals(StringBuilder data,MeshFilter filter)
    {
        foreach (Vector3 vector3 in filter.mesh.normals)
        {
            Vector3 wDir = filter.transform.TransformDirection(vector3);
            data.Append("vn ")
                .Append(-wDir.x)
                .Append(" ")
                .Append(wDir.y)
                .Append(" ")
                .Append(wDir.z)
                .Append("\n");
        }
        data.Append("\n");
    }

    private void SaveUVs(StringBuilder data,MeshFilter filter)
    {
        foreach (Vector2 uv in filter.mesh.uv)
        {
            data.Append("vt ")
                .Append(uv.x)
                .Append(" ")
                .Append(uv.y)
                .Append("\n");
        }
    }

    private void SaveFace(StringBuilder data,int[] triangles,int offset)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            data.Append("f ")
                .Append(string.Format("{0}/{0}/{0}", triangles[i] + 1 + offset))
                .Append(" ")
                .Append(string.Format("{0}/{0}/{0}", triangles[i + 2] + 1 + offset))
                .Append(" ")
                .Append(string.Format("{0}/{0}/{0}", triangles[i + 1] + 1 + offset))
                .Append("\n");

        }
    }

    private void SaveMaterialsAndFace(StringBuilder data,MeshFilter filter)
    {
        Material[] materials = filter.GetComponent<Renderer>().materials;
        for (int i = 0; i < filter.mesh.subMeshCount; i++)
        {
            string materialName = materials[i].name;
            data.Append("usemtl ")
                .Append(materialName)
                .Append("\n");

            int[] triangles = filter.mesh.GetTriangles(i);
            SaveFace(data, triangles,_vertexOffset);
        }

        _vertexOffset += filter.mesh.vertices.Length;
    }
}

