using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class GenerateMesh_2 : MonoBehaviour
{
	public Vector4 Tangent;

	private Mesh _mesh;
	// Use this for initialization
	void Start ()
	{
		_mesh= new Mesh();
		
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = _mesh;
		_mesh.name = "Mesh01";
		_mesh.vertices = GetVertexs();
		_mesh.triangles = GetTriangles();
		_mesh.uv = GetUVs();
		_mesh.uv2 = GetUV2s();
		_mesh.normals = GetNormals();
		//mesh.RecalculateNormals();
		
	}

	private void Update()
	{
		_mesh.tangents = GetTangent();
	}

	private Vector4[] GetTangent()
	{
		return new Vector4[]
		{
			Tangent,
			Tangent,
			Tangent,
			Tangent,
		};
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
