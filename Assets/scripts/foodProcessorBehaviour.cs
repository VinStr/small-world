using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodProcessorBehaviour : buildingBehaviour {

	public static float resourceTime = 5f;

	public GameObject pond;
	public void setPond(GameObject pond)
	{
		this.pond = pond;
	}

	void Awake()
	{
		building = Building.FOOD_PROCESSOR;
	}

	// Use this for initialization
	void Start () {

		base.Start();

		bottom = transform.GetChild(0).GetChild(5).position;
		top = bottom + (transform.GetChild(0).GetChild(5).forward * 0.05f);

	}

	private float animT = 0;
	private Vector3 top;
	private Vector3 bottom;
	private float animTime = 1f;
	private bool up = true;

	private float resourceTimer = 0;
	private bool processing = true;
	// Update is called once per frame
	void Update () {

		base.Update();

		//animation
		if (animT >= 1) {
			up = false;
			animT = 1;
		}
		else if (animT <= 0) {
			up = true;
			animT = 0;
		}

		if(up) animT += Time.deltaTime / animTime;
		else animT -= Time.deltaTime / animTime;
		transform.GetChild(0).GetChild(5).position = Vector3.Lerp(bottom, top, animT);


		if (pond.GetComponent<PlanetResourceBehaviour>().getHealth() <= 0) processing = false;

		//timer for resource output 5 seconds
		if (built == true && processing == true && outputFood < 4)
		{
			resourceTimer += Time.deltaTime / resourceTime;
			if (resourceTimer >= 1)
			{
				resourceTimer = 0;
				outputFood++;
				pond.GetComponent<PlanetResourceBehaviour>().decHealth();
			}
		}

		//show available materials for collection
		if (outputFood == 0)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputFood == 1)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputFood == 2)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputFood == 3)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
		}
		else if (outputFood == 4)
		{
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
		}

	}
}
