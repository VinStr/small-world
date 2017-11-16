using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quaryBeheviour : buildingBehaviour {

	public static float resourceTime = 5f;

	private GameObject ore;
	public void setOre(GameObject ore)
	{
		this.ore = ore;
	}

	void Awake()
	{
		building = Building.QUARY;
	}

	// Use this for initialization
	void Start () {

		base.Start();

	}

	private float resourceTimer = 0;
	private bool mining = true;
	// Update is called once per frame
	void Update () {

		base.Update();

		if (ore.GetComponent<PlanetResourceBehaviour>().getHealth() <= 0) mining = false;

		//animation to spin
		if(mining) transform.GetChild(0).GetChild(0).RotateAroundLocal(new Vector3(0, 1, 0), 1 / Time.deltaTime * 0.001f);

		//timer for resource output 5 seconds
		if (built == true && mining == true && outputMineral < 4)
		{
			resourceTimer += Time.deltaTime / resourceTime;
			if (resourceTimer >= 1)
			{
				resourceTimer = 0;
				outputMineral++;
				ore.GetComponent<PlanetResourceBehaviour>().decHealth();
			}
		}

		//show available materials for collection
		if (outputMineral == 0)
		{
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
		}
		else if (outputMineral == 1)
		{
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
		}
		else if (outputMineral == 2)
		{
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
		}
		else if (outputMineral == 3)
		{
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
		}
		else if (outputMineral == 4)
		{
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
		}

	}
}
