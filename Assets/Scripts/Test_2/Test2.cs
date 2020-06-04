using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Test2 : MonoBehaviour
{
	public int X = 3, Y = 2;
	private Vector3[] _vertices;
	private Vector2[] _uv;
	private Vector3[] _normals;

	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh();
		
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = mesh;
		mesh.name = "Mesh02";
		
		_vertices = new Vector3[(X+1)*(Y+1)];
		_uv = new Vector2[_vertices.Length];
		_normals = new Vector3[_vertices.Length];
		
		StartCoroutine(GeneratePlane(mesh));
	}

	private IEnumerator GeneratePlane(Mesh mesh)
	{
		yield return GetVertices();
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.normals = _normals;
		yield return GetTriangles(mesh);
	}

	private IEnumerator GetVertices()
	{
		for (int y = 0,i = 0; y < Y+1; y++)
		{
			for (int x = 0; x < X+1; x++,i++)
			{
				_vertices[i] = new Vector3(x,y);
				_uv[i] = new Vector2((float)x/X,(float)y/Y);
				_normals[i] = GetNormal(y);
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	private Vector3 GetNormal(int y)
	{
		if (y % 2 == 0)
		{
			return Vector3.up;
		}
		else
		{
			return Vector3.down;
		}
	}

	private IEnumerator GetTriangles(Mesh mesh)
	{
		int[] triangles = new int[X*Y*6];

		for (int startIndex = 0,y= 0,triangleIndex = 0; startIndex < _vertices.Length && y<Y && triangleIndex <triangles.Length; startIndex++)
		{
			for (int x = 0; x < X; x++, startIndex++,triangleIndex += 6)
			{
				//当前四边形的第一个三角形
				triangles[triangleIndex] = startIndex;
				triangles[triangleIndex + 1] = startIndex + X + 1;
				triangles[triangleIndex + 2] = startIndex + 1;
				mesh.triangles = triangles;
				yield return new WaitForSeconds(0.2f);
				
				//当前四边形的第二个三角形
				triangles[triangleIndex + 3] = startIndex + X + 1;
				triangles[triangleIndex + 4] = startIndex + X + 2;
				triangles[triangleIndex + 5] = startIndex + 1;
				
				mesh.triangles = triangles;
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if(_vertices == null)
			return;
		
		for (int i = 0; i < _vertices.Length; i++)
		{
			Gizmos.DrawSphere(_vertices[i],0.1f);
		}
	}
}
