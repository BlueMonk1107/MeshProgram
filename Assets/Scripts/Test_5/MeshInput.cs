using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInput : MonoBehaviour
{

	public float _force = 10;
	private float _offset = 0.1f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				var deformer = hit.collider.GetComponent<Test5_1>();
				var point = hit.normal * _offset + hit.point;
				deformer.AddForce(point,_force);
			}
		}
	}
}
