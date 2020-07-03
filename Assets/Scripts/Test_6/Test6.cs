using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test6 : MonoBehaviour
{
	private Vector3 _startPos;
	private Vector3 _endPos;
	private Vector3 _hitPos;
	private Vector3 _dir, _upDir, _planeNormal;
	private Mesh _mesh;
	private Transform _hitTrans;
	private MeshFilter _leftMeshFilter;
	/// <summary>
	/// 三角形三个顶点的坐标信息缓存（世界坐标）
	/// </summary>
	private Vector3[] _triangleTemp = new Vector3[3];
	/// <summary>
	/// 三角形三个点乘结果的缓存
	/// </summary>
	private float[] _resultTemp = new float[3];
	
	//左侧（和平面法向量同侧）模型数据
	private List<Vector3> _leftVertices = new List<Vector3>();
	private List<int> _leftTriangles = new List<int>();
	private List<Vector3> _leftNormals = new List<Vector3>();
	/// <summary>
	/// key:原模型顶点下标  value:现模型的顶点下标
	/// </summary>
	private Dictionary<int, int> _leftIndexMapping = new Dictionary<int, int>();
	
	//右侧（和平面法向量反向）模型数据
	private List<Vector3> _rightVertices = new List<Vector3>();
	private List<int> _rightTriangles = new List<int>();
	private List<Vector3> _rightNormals = new List<Vector3>();
	/// <summary>
	/// key:原模型顶点下标  value:现模型的顶点下标
	/// </summary>
	private Dictionary<int, int> _rightIndexMapping = new Dictionary<int, int>();
	/// <summary>
	/// 切面上新生成的顶点
	/// </summary>
	private List<Vector3> _rectionVertexs = new List<Vector3>();


	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_startPos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			_endPos = Input.mousePosition;
			Ray();
			Cut();
		}

		DebugDir();
	}

	private void Ray()
	{
		var center = (_endPos + _startPos) * 0.5f;
		var ray = Camera.main.ScreenPointToRay(center);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			_hitPos = hit.point;
			_hitTrans = hit.transform;
			_leftMeshFilter = _hitTrans.GetComponent<MeshFilter>();
			_mesh = hit.transform.GetComponent<MeshFilter>().mesh;
			_dir = (hit.point - Camera.main.transform.position).normalized;
			_upDir = (-_dir * Vector3.Dot(Vector3.up, _dir) + Vector3.up).normalized;
			_planeNormal = Vector3.Cross(_dir, _upDir);

			Vector3 sildeDir = _endPos - _startPos;
			Vector3 baseDir = sildeDir.y < 0 ? -Vector3.up : Vector3.up;
			float angle = Vector3.Angle(sildeDir, baseDir);

			if (sildeDir.y < 0)
			{
				angle = sildeDir.x > 0 ? angle : -angle;
			}
			else
			{
				angle = sildeDir.x > 0 ? -angle : angle;
			}

			angle *= Mathf.Deg2Rad;
			_upDir = _upDir * Mathf.Cos(angle) + _planeNormal * Mathf.Sin(angle);
			_planeNormal = Vector3.Cross(_dir, _upDir);
		}
		else
		{
			_hitPos = Vector3.zero;
			_mesh = null;
		}
	}

	private void Cut()
	{
		if(_mesh == null)
			return;
		ClearData();
		CalculateVertexInfo();
		GenerateSectionInfo();
		GenerateMesh();
	}
	
	private void ClearData()
	{
		_leftVertices.Clear();
		_leftTriangles.Clear();
		_leftNormals.Clear();
		_leftIndexMapping.Clear();
		_rightNormals.Clear();
		_rightTriangles.Clear();
		_rightVertices.Clear();
		_rightIndexMapping.Clear();
		_rectionVertexs.Clear();
	}

	private void GenerateMesh()
	{
		GenerateLeftMesh();
		GenerateRightMesh();
	}
	
	private void GenerateLeftMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = _leftVertices.ToArray();
		mesh.normals = _leftNormals.ToArray();
		mesh.triangles = _leftTriangles.ToArray();
		_leftMeshFilter.mesh = mesh;
	}
	
	private void GenerateRightMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = _rightVertices.ToArray();
		mesh.normals = _rightNormals.ToArray();
		mesh.triangles = _rightTriangles.ToArray();
		
		GameObject newGo = new GameObject();
		newGo.transform.position = _hitTrans.position;
		newGo.transform.rotation = _hitTrans.rotation;

		newGo.AddComponent<MeshFilter>().mesh = mesh;
		newGo.AddComponent<MeshRenderer>().material = _hitTrans.GetComponent<MeshRenderer>().material;

		newGo.AddComponent<Rigidbody>();
		newGo.AddComponent<BoxCollider>();
	}

	/// <summary>
	/// 分别计算并存储切开的两个部分的顶点信息
	/// </summary>
	private void CalculateVertexInfo()
	{
		var triangles = _mesh.triangles;
		
		for (int i = 0; i < triangles.Length; i+= 3)
		{
			//三个顶点在原triangles中下标是 i i+1 i+2
			GetDotResult(i, triangles);

			if (_resultTemp[0] >= 0 && _resultTemp[1] >= 0 && _resultTemp[2] >= 0)
			{
				//左侧
				SaveOldVertex(i, true);
			}
			else if (_resultTemp[0] <= 0 && _resultTemp[1] <= 0 && _resultTemp[2] <= 0)
			{
				//右侧
				SaveOldVertex(i, false);
			}
			else
			{
				//被切割的三角形部分
				int differentIndex = GetDifferentSidePointIndex();
				//当前点在triangles的下标
				int p0_Index = i + differentIndex;
				int p1_index = (differentIndex + 1) % 3 + i;
				//先算出c1点进行存储
				SavePointOnSection(_mesh.triangles[p0_Index], _mesh.triangles[p1_index]);
				int p2_index = (differentIndex + 2) % 3 + i;
				//再算出c2点进行存储
				SavePointOnSection(_mesh.triangles[p0_Index], _mesh.triangles[p2_index]);

				SaveCutTriangleVertex(_resultTemp[differentIndex], p0_Index, p1_index, p2_index);
			}
		}
	}

	private void SaveCutTriangleVertex(float result,int p0,int p1,int p2)
	{
		if (result >= 0)
		{
			SaveOldVertex(p0,_leftVertices,_leftNormals,_leftIndexMapping);
			SaveSectionVertexWithOnePoint(p0,_leftTriangles,_leftVertices,_leftNormals,_leftIndexMapping);
			
			SaveOldVertex(p1,_rightVertices,_rightNormals,_rightIndexMapping);
			SaveOldVertex(p2,_rightVertices,_rightNormals,_rightIndexMapping);
			SaveSectionVertexWithTwoPoint(p1,p2,_rightTriangles,_rightVertices,_rightNormals,_rightIndexMapping);
		}
		else
		{
			SaveOldVertex(p0,_rightVertices,_rightNormals,_rightIndexMapping);
			SaveSectionVertexWithOnePoint(p0,_rightTriangles,_rightVertices,_rightNormals,_rightIndexMapping);
			
			SaveOldVertex(p1,_leftVertices,_leftNormals,_leftIndexMapping);
			SaveOldVertex(p2,_leftVertices,_leftNormals,_leftIndexMapping);
			SaveSectionVertexWithTwoPoint(p1,p2,_leftTriangles,_leftVertices,_leftNormals,_leftIndexMapping);
		}
	}

	private void SaveSectionVertexWithOnePoint(	
		int                   index, 
		List<int>             curTriangles,
		List<Vector3>         curVertices,
		List<Vector3>         curNormals,
		Dictionary<int, int>  indexMapping)
	{
		int vertexIndex = _mesh.triangles[index];
		
		//存储c1
		curVertices.Add(_rectionVertexs[_rectionVertexs.Count - 2]);
		//存储c2
		curVertices.Add(_rectionVertexs[_rectionVertexs.Count - 1]);
		
		curNormals.Add(_mesh.normals[vertexIndex]);
		curNormals.Add(_mesh.normals[vertexIndex]);
		
		curTriangles.Add(indexMapping[vertexIndex]);
		curTriangles.Add(curVertices.Count - 2);
		curTriangles.Add(curVertices.Count - 1);
	}

	private void SaveSectionVertexWithTwoPoint(
		int index1,
		int index2,
		List<int> curTriangles,
		List<Vector3> curVertices,
		List<Vector3> curNormals,
		Dictionary<int, int> indexMapping)
	{
		int vertexIndex1 = _mesh.triangles[index1];
		int vertexIndex2 = _mesh.triangles[index2];
		
		//存储c1
		curVertices.Add(_rectionVertexs[_rectionVertexs.Count - 2]);
		//存储c2
		curVertices.Add(_rectionVertexs[_rectionVertexs.Count - 1]);
		
		curNormals.Add(_mesh.normals[vertexIndex1]);
		curNormals.Add(_mesh.normals[vertexIndex2]);
		
		//c1-p1-p2
		curTriangles.Add(curVertices.Count - 2);
		curTriangles.Add(indexMapping[vertexIndex1]);
		curTriangles.Add(indexMapping[vertexIndex2]);

		
		//p2-c2-c1
		curTriangles.Add(indexMapping[vertexIndex2]);
		curTriangles.Add(curVertices.Count - 1);
		curTriangles.Add(curVertices.Count - 2);
		
	}

	/// <summary>
	/// 返回值是对应点在_resultTemp中的下标
	/// </summary>
	/// <returns></returns>
	private int GetDifferentSidePointIndex()
	{
		List<int> temp1 = new List<int>(2);
		List<int> temp2 = new List<int>(2);
		for (int i = 0; i < _resultTemp.Length; i++)
		{
			if (_resultTemp[i] > 0)
			{
				temp1.Add(i);
			}
			else
			{
				temp2.Add(i);
			}
		}

		if (temp1.Count == 1)
		{
			return temp1[0];
		}
		else
		{
			return temp2[0];
		}
	}
	//参数是 原模型vertices下标
	private void SavePointOnSection(int index1,int index2)
	{
		Vector3 side = _mesh.vertices[index2] - _mesh.vertices[index1];
		Vector3 dir = _hitTrans.TransformDirection(side.normalized);
		Vector3 startPos = _hitTrans.TransformPoint(_mesh.vertices[index1]);
		float lengthOnNormal = Vector3.Dot(_hitPos, _planeNormal) - Vector3.Dot(startPos, _planeNormal);
		float length = lengthOnNormal / Vector3.Dot(dir, _planeNormal);
		Vector3 target = startPos + dir * length;
		_rectionVertexs.Add(_hitTrans.InverseTransformPoint(target));
	}

	private void GetDotResult(int index, int[] triangles)
	{
		for (int i = 0; i < _triangleTemp.Length; i++)
		{
			_triangleTemp[i] = _hitTrans.TransformPoint(_mesh.vertices[triangles[index + i]]);

			_resultTemp[i] = Vector3.Dot(_planeNormal, _triangleTemp[i] - _hitPos);
		}
		
	}

	private void SaveOldVertex(int index,bool isLeft)
	{
		if (isLeft)
		{
			SaveTriangleVertex(index, _leftTriangles, _leftVertices, _leftNormals,_leftIndexMapping);
		}
		else
		{
			SaveTriangleVertex(index, _rightTriangles, _rightVertices, _rightNormals,_rightIndexMapping);
		}
	}

	private void SaveTriangleVertex(
		int                   index, 
		List<int>             curTriangles,
		List<Vector3>         curVertices,
		List<Vector3>         curNormals,
		Dictionary<int, int>  indexMapping)
	{
		for (int i = 0; i < 3; i++)
		{
			SaveOldVertex(index + i, curVertices, curNormals, indexMapping);
			curTriangles.Add(indexMapping[_mesh.triangles[index + i]]);
		}
	}
	
	private void SaveOldVertex(
		int                   index, 
		List<Vector3>         curVertices,
		List<Vector3>         curNormals,
		Dictionary<int, int>  indexMapping)
	{
		int vertexIndex = _mesh.triangles[index];

		if (!indexMapping.ContainsKey(vertexIndex))
		{
			curVertices.Add(_mesh.vertices[vertexIndex]);
			curNormals.Add(_mesh.normals[vertexIndex]);
			indexMapping.Add(vertexIndex,curVertices.Count - 1);
		}
	}

	private void GenerateSectionInfo()
	{
		Vector3 center = (_rectionVertexs[0] + _rectionVertexs[_rectionVertexs.Count / 2]) * 0.5f;
		Vector3 centerNormal = _hitTrans.InverseTransformDirection(_planeNormal);

		SaveSectionCenter(center, centerNormal);
		int leftCenterIndex = _leftVertices.Count - 1;
		int rightCenterIndex = _rightVertices.Count - 1;

		for (int i = 0; i < _rectionVertexs.Count; i+=2)
		{
			Vector3 v1 = _rectionVertexs[i];
			Vector3 v2 = _rectionVertexs[i + 1];
			Vector3 normal = Vector3.Cross(v1 - center, v2 - center);

			SaveSectionVertexInfo(i, -centerNormal, _leftVertices, _leftNormals);
			SaveLeftSectionTriangle(_planeNormal, normal, leftCenterIndex, _leftTriangles, _leftVertices);
			//SaveSectionTriangle(_planeNormal, normal, leftCenterIndex, _leftTriangles, _leftVertices);
			
			SaveSectionVertexInfo(i, centerNormal, _rightVertices, _rightNormals);
			SaveRightSectionTriangle(_planeNormal, normal, rightCenterIndex, _rightTriangles, _rightVertices);
			//SaveSectionTriangle(-_planeNormal, normal, rightCenterIndex, _rightTriangles, _rightVertices);
		}
	}
	
	private void SaveSectionTriangle(Vector3 planeNormal,Vector3 normal,int centerIndex,List<int> triangles,List<Vector3> vertices)
	{
		if (Vector3.Dot(planeNormal, normal) < 0)
		{
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);
		}
		else
		{
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 2);
		}
	}

	private void SaveLeftSectionTriangle(Vector3 planeNormal,Vector3 normal,int centerIndex,List<int> triangles,List<Vector3> vertices)
	{
		if (Vector3.Dot(planeNormal, normal) < 0)
		{
			//左侧切面 三角形法向量方向和planeNormal方向相反，才能正常显示
			// 0 1 2
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);
		}
		else
		{
			// 0 2 1
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 2);
		}
	}
	
	private void SaveRightSectionTriangle(Vector3 planeNormal,Vector3 normal,int centerIndex,List<int> triangles,List<Vector3> vertices)
	{
		if (Vector3.Dot(planeNormal, normal) > 0)
		{
			//右侧切面 三角形法向量方向和planeNormal方向相同，才能正常显示
			// 0 1 2
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);
		}
		else
		{
			// 0 2 1
			triangles.Add(centerIndex);
			triangles.Add(vertices.Count - 1);
			triangles.Add(vertices.Count - 2);
		}
	}


	private void SaveSectionVertexInfo(int index,Vector3 normal,List<Vector3> vertices,List<Vector3> normals)
	{
		vertices.Add(_rectionVertexs[index]);
		vertices.Add(_rectionVertexs[index + 1]);
		normals.Add(normal);
		normals.Add(normal);
	}

	private void SaveSectionCenter(Vector3 center,Vector3 normal)
	{
		_leftVertices.Add(center);
		_leftNormals.Add(-normal);
		
		_rightVertices.Add(center);
		_rightNormals.Add(normal);
	}

	private void DebugDir()
	{
		Debug.DrawRay(_hitPos,_dir,Color.blue);
		Debug.DrawRay(_hitPos,_upDir,Color.red);
		Debug.DrawRay(_hitPos,_planeNormal,Color.yellow);
	}
}
