using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class researchBehaviour : buildingBehaviour {


	private float reasurchProgress = 0;
	public float getResearchProgress()
	{
		return reasurchProgress;
	}
	public static float reasurchRate = 1f;

	void Awake()
	{
		building = Building.RESEARCH;
	}

	// Use this for initialization
	void Start () {

		base.Start();

	}
	
	// Update is called once per frame
	void Update () {

		base.Update();

		//animation
		transform.GetChild(0).GetChild(5).Rotate(Vector3.up, 30.0f * Time.deltaTime, Space.World);

		if (built == true)
		{
			if (assingedWorkers.Count <= 0) reasurchProgress += reasurchRate * 0f;
			else if (assingedWorkers.Count == 1) reasurchProgress += reasurchRate;
			else if (assingedWorkers.Count == 2) reasurchProgress += reasurchRate * 1.5f;
			else if (assingedWorkers.Count >= 3) reasurchProgress += reasurchRate * 2f;
		}

		Mathf.Clamp(reasurchProgress, 0, 100);
		if (reasurchProgress >= 100)
		{
			reasurchProgress = 0;
			transform.GetComponentInParent<GameManager>().incResearchLevel();
		}


		//put workers in the positions
		if (built)
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponentInParent<GameManager>().getTriangle(0).edgeRadius());
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[7])
				{
					if (assingedWorkers.Contains(colliders[i].gameObject))
					{
						int index = assingedWorkers.IndexOf(colliders[i].gameObject);
						if (index == 0) colliders[i].GetComponent<workerBehaviour>().goToPosition(transform.GetChild(0).GetChild(0).position, 1);
						else if (index == 1) colliders[i].GetComponent<workerBehaviour>().goToPosition(transform.GetChild(0).GetChild(1).position, 1);
						else if (index == 2) colliders[i].GetComponent<workerBehaviour>().goToPosition(transform.GetChild(0).GetChild(2).position, 1);
					}
				}
			}
		}

	}
}
