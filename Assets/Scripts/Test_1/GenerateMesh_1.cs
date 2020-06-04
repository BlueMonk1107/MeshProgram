using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class GenerateMesh_1 : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Mesh mesh = new Mesh();
		
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = mesh;
		mesh.name = "Mesh01";
		mesh.vertices = GetVertexs();
		mesh.triangles = GetTriangles();
		mesh.uv = GetUVs();
		mesh.uv2 = GetUV2s();
		mesh.normals = GetNormals();
	}

	private Vector3[] GetVertexs()
	{
		return new Vector3[]
		{
			new Vector3(0, 0, 0),
			new Vector3(1, 0, 0),
			new Vector3(1, 1, 0),
			new Vector3(0, 1, 0),
		};
	}

	private Vector2[] GetUVs()
	{
		return new Vector2[]
		{
			new Vector2(1,0),
			new Vector2(0,0),
			new Vector2(0,1),
			new Vector2(1,1),
		};
	}
	
	private Vector2[] GetUV2s()
	{
		return new Vector2[]
		{
			new Vector2(0,0),
			new Vector2(0,0),
			new Vector2(0,0),
			new Vector2(0,0),
		};
	}
	
	private Vector3[] GetNormals()
	{
		return new Vector3[]
		{
			Vector3.right, 
			Vector3.right, 
			Vector3.right, 
			Vector3.right, 
		};
	}

	private int[] GetTriangles()
	{
		return new int[]
		{
			0, 1, 2,
			0, 2, 3
		};
	}
}
