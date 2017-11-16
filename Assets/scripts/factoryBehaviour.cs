using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class factoryBehaviour : buildingBehaviour{



	public static float resourceTime = 5f;

	void Awake()
	{
		building = Building.FACTORY;
	}

	// Use this for initialization
	void Start () {

		base.Start();

	}

	private float resourceTimer = 0;
	// Update is called once per frame
	void Update () {

		base.Update();

		//timer for resource output 5 seconds
		if (built == true && outputMaterial < 4)
		{
			resourceTimer += Time.deltaTime / resourceTime;
			if (resourceTimer >= 1)
			{
				resourceTimer = 0;
				outputMaterial++;
			}
		} 

		//show available materials for collection
		if(outputMaterial == 0)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputMaterial == 1)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputMaterial == 2)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputMaterial == 3)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputMaterial == 4)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
		}


	}
}
