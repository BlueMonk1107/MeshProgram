using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test5_1 : MonoBehaviour
{

	private Mesh _mesh;
	private Vector3[] _orinalVertices, _displacedVertivices;
	private Vector3[] _vertexVelocities;
	private float _springForce = 20;
	private float _damping = 0.9f;

	// Use this for initialization
	void Start ()
	{
		_mesh = GetComponent<MeshFilter>().mesh;

		_orinalVertices = _mesh.vertices;
		
		_displacedVertivices = new Vector3[_orinalVertices.Length];
		for (int i = 0; i < _orinalVertices.Length; i++)
		{
			_displacedVertivices[i] = _orinalVertices[i];
		}
		
		_vertexVelocities = new Vector3[_orinalVertices.Length];
	}

	public void AddForce(Vector3 hitPos,float force)
	{
		Debug.Log("AddForce,point:"+hitPos+" force:"+force);

		hitPos = transform.InverseTransformPoint(hitPos);
		for (int i = 0; i < _displacedVertivices.Length; i++)
		{
			AddForceToVertex(i, hitPos, force);
		}
	}

	private void AddForceToVertex(int i,Vector3 hitPos,float force)
	{
		Vector3 point = _displacedVertivices[i] - hitPos;
		force = force / (1 + point.sqrMagnitude);
		float velocity = force * Time.deltaTime;
		_vertexVelocities[i] += point.normalized * velocity;
	}

	private void Update()
	{
		for (int i = 0; i < _displacedVertivices.Length; i++)
		{
			_vertexVelocities[i] += GetReactiveVelocity(i);
			_vertexVelocities[i] *= _damping;
			_displacedVertivices[i] += _vertexVelocities[i] * Time.deltaTime;
		}

		_mesh.vertices = _displacedVertivices;
		_mesh.RecalculateNormals();
	}

	private Vector3 GetReactiveVelocity(int i)
	{
		Vector3 reactiveForce = _orinalVertices[i] - _displacedVertivices[i];
		return reactiveForce * Time.deltaTime * _springForce;
	}
}
