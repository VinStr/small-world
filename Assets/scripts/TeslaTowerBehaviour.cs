using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TeslaTowerBehaviour : buildingBehaviour {

	private GameObject[] target = new GameObject[3] { null, null, null };
	private float range = 1.4f;

	public void findTarget(int index)
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, range);
		GameObject backup = null;

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[8] && colliders[i].gameObject == target[index])
			{
				return;
			}
			else if (colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[8] && ArrayUtility.Contains(target, colliders[i].gameObject))
			{
				backup = colliders[i].gameObject;
			}

		}

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[8])
			{
				target[index] = colliders[i].gameObject;
				return;
			}
		}
		target[index] = backup;
	}

	void Awake()
	{
		building = Building.DEFENCE;
	}

	// Use this for initialization
	void Start () {

		base.Start();

	}

	// Update is called once per frame
	public static float damageRate = 0.2f;
	void Update () {

		base.Update();

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

		if (built == true)
		{
			if(assingedWorkers.Count >= 1) findTarget(0);
			if (assingedWorkers.Count >= 2) findTarget(1);
			if (assingedWorkers.Count >= 3) findTarget(2);
		}

		if (target[0] == null)//laser 1
		{
			transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

		}
		else
		{
			transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(target[0].transform.GetChild(0).position)+new Vector3(0,-3f,0));
			t += Time.deltaTime / damageRate;
			if (t >= 1)
			{
				target[0].transform.GetComponent<AlienBehaviour>().takeDamage();
				t = 0;
			}
		}
		if (target[1] == null)//laser 2
		{
			transform.GetChild(3).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

		}
		else
		{
			transform.GetChild(3).GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(target[1].transform.GetChild(0).position) + new Vector3(0, -3f, 0));
			t += Time.deltaTime / damageRate;
			if (t >= 1)
			{
				target[1].transform.GetComponent<AlienBehaviour>().takeDamage();
				t = 0;
			}
		}
		if (target[2] == null)//laser 3
		{
			transform.GetChild(4).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

		}
		else
		{
			transform.GetChild(4).GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(target[2].transform.GetChild(0).position) + new Vector3(0, -3f, 0));
			t += Time.deltaTime / damageRate;
			if (t >= 1)
			{
				target[2].transform.GetComponent<AlienBehaviour>().takeDamage();
				t = 0;
			}
		}

	}
}
