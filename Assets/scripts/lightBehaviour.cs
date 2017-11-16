using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightBehaviour : MonoBehaviour {

	public Transform target;
	public Transform sun;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(target);//look at the target object i.e. planet

		//sun.SetPositionAndRotation(this.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));//place sun model at position of light

	}
}
