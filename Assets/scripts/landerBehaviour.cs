using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landerBehaviour : buildingBehaviour {

	private GameObject pad;
	private GameObject emitter;
	private GameObject materialResource;
	private GameObject mineralResource;
	private GameObject foodResource;

	private Vector3 startPosition = new Vector3(0, 0, 0);
	private Vector3 endPosition = new Vector3(0, 0, 0);
	private float timeToReach = 5;

	private bool landing = true;
	public void land (triangle start, Vector3 planetPos)
	{
		t = 0;
		endPosition = start.findCenter();
		startPosition = start.findCenter() + 3f * Vector3.Normalize(start.findCenter() - planetPos);
		transform.position = startPosition;
		Vector3 direction = Vector3.Cross(start.vertex2 - start.vertex1, start.vertex3 - start.vertex1);
		transform.rotation = Quaternion.LookRotation(direction);
		transform.Rotate(new Vector3(90, 0, 0));
	}
	public bool landed()
	{
		return landing;
	}

	public void depositResource(Resources resource)
	{
		if (resource == Resources.MATERIAL) outputMaterial++;
		else if(resource == Resources.MINERAL) outputMineral++;
		else if (resource == Resources.FOOD) outputFood++;
	}


	void Awake()
	{
		//needed used as starting amounts
		outputMaterial = neededMaterial;
		outputMineral = neededMineral;
		outputFood = neededFood;

		building = Building.LANDER;

	}

	// Use this for initialization
	new void Start () {

		pad = transform.GetChild(0).GetChild(1).gameObject;
		emitter = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		materialResource = transform.GetChild(4).gameObject;
		mineralResource = transform.GetChild(5).gameObject;
		foodResource = transform.GetChild(6).gameObject;

		pad.SetActive(false);
		emitter.GetComponent<EllipsoidParticleEmitter>().emit = true;
		materialResource.SetActive(false);
		mineralResource.SetActive(false);
		foodResource.SetActive(false);

	}

	private bool once = true;
	// Update is called once per frame
	new void Update()
	{
		if (landing == true)
		{
			t += Time.deltaTime / timeToReach;
			transform.position = Vector3.Lerp(startPosition, endPosition, 1.06f - Mathf.Exp(0.06f - 3 * t));
			//emitter.GetComponent<EllipsoidParticleEmitter>().localVelocity = new Vector3(0, 0, 0.2f * (-0.06f + Mathf.Exp(0.06f - 3 * t)));
			if (transform.position == endPosition) landing = false;
		}
		else if (once)
		{
			pad.SetActive(true);
			emitter.GetComponent<EllipsoidParticleEmitter>().emit = false;
			materialResource.SetActive(true);
			mineralResource.SetActive(true);
			foodResource.SetActive(true);

			transform.GetComponentInParent<GameManager>().addWorker(transform.GetChild(1).position);
			transform.GetComponentInParent<GameManager>().addWorker(transform.GetChild(2).position);
			transform.GetComponentInParent<GameManager>().addWorker(transform.GetChild(3).position);
			once = false;
		}
		else
		{
			if(outputMaterial>0) materialResource.SetActive(true);
			else materialResource.SetActive(false);
			if (outputMineral > 0) mineralResource.SetActive(true);
			else mineralResource.SetActive(false);
			if (outputFood > 0) foodResource.SetActive(true);
			else foodResource.SetActive(false);
		}
	}
}
