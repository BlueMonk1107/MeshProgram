using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Test3 : MonoBehaviour {

	private Vector3[] _vertices;
	private int[] _triangles;
	
	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh();
		
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = mesh;
		mesh.name = "Mesh03";
		GenerateVextex();
		GenerateTriangle();

		mesh.vertices = _vertices;
		mesh.triangles = _triangles;
	}

	private void GenerateVextex()
	{
		_vertices = new Vector3[8];
		_vertices[0] = new Vector3(-0.5f,-0.5f,-0.5f);
		_vertices[1] = new Vector3(0.5f,-0.5f,-0.5f);
		_vertices[2] = new Vector3(-0.5f,-0.5f,0.5f);
		_vertices[3] = new Vector3(0.5f,-0.5f,0.5f);
		
		_vertices[4] = new Vector3(-0.5f,0.5f,-0.5f);
		_vertices[5] = new Vector3(0.5f,0.5f,-0.5f);
		_vertices[6] = new Vector3(-0.5f,0.5f,0.5f);
		_vertices[7] = new Vector3(0.5f,0.5f,0.5f);
	}

	private void GenerateTriangle()
	{
		_triangles = new int[36];
		//底面
		_triangles[0] = 0;
		_triangles[1] = 1;
		_triangles[2] = 2;
		
		_triangles[3] = 1;
		_triangles[4] = 3;
		_triangles[5] = 2;
		//前
		_triangles[6] = 0;
		_triangles[7] = 4;
		_triangles[8] = 5;
		
		_triangles[9] = 5;
		_triangles[10] = 1;
		_triangles[11] = 0;
		//后
		_triangles[12] = 3;
		_triangles[13] = 7;
		_triangles[14] = 6;
		
		_triangles[15] = 6;
		_triangles[16] = 3;
		_triangles[17] = 2;
		//左
		_triangles[18] = 6;
		_triangles[19] = 4;
		_triangles[20] = 0;
		
		_triangles[21] = 4;
		_triangles[22] = 0;
		_triangles[23] = 2;
		//右
		_triangles[24] = 1;
		_triangles[25] = 5;
		_triangles[26] = 7;
		
		_triangles[27] = 7;
		_triangles[28] = 3;
		_triangles[29] = 1;
		//上
		_triangles[30] = 6;
		_triangles[31] = 7;
		_triangles[32] = 4;
		
		_triangles[33] = 7;
		_triangles[34] = 5;
		_triangles[35] = 4;
	}
	
	private void OnDrawGizmos()
	{
		if(_vertices == null)
			return;
		
		for (int i = 0; i < _vertices.Length; i++)
		{
			Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]),0.1f);
		}
	}
}

