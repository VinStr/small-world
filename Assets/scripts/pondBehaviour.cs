using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pondBehaviour : PlanetResourceBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		base.Update();

		transform.GetChild(0).Rotate(new Vector3(0f, 0f, 0.3f));

	}
}
