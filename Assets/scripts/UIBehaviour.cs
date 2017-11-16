using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	public GameObject workerIconPrefab;

	public Color factory;
	public Color quary;
	public Color defence;
	public Color foodProcessor;
	public Color cloner;
	public Color research;

	private int alienWave = 0;
	public void setAlienWave(int alienWave)
	{
		this.alienWave = alienWave;
	}

	private float alienProgress = 0;//1-100
	public void setAlienProgress(float value)
	{
		alienProgress = value;
	}

	private int[] jobs;// idle 0, construction 1, defense 2, factory 3, quarry 4, food 5, cloning 6, research 7
	public void setJob(Jobs job, int value)
	{
		if (job == Jobs.IDLE) this.jobs[0] = value;
		else if (job == Jobs.CONSTRUCTION) this.jobs[1] = value;
		else if (job == Jobs.DEFENCE) this.jobs[2] = value;
		else if (job == Jobs.FACTORY) this.jobs[3] = value;
		else if (job == Jobs.QUARY) this.jobs[4] = value;
		else if (job == Jobs.FOOD) this.jobs[5] = value;
		else if (job == Jobs.CLONING) this.jobs[6] = value;
		else if (job == Jobs.RESEARCH) this.jobs[7] = value;
	}

	private int avalibleMaterial = 0;
	private int avalibleMineral = 0;
	private int avalibleFood = 0;
	public void setAvalibleResource(Resources resource, int value)
	{
		if (resource == Resources.MATERIAL) avalibleMaterial = value;
		else if (resource == Resources.MINERAL) avalibleMineral = value;
		else if (resource == Resources.FOOD) avalibleFood = value;
	}

	// Use this for initialization
	void Start () {

		jobs = new int[System.Enum.GetNames(typeof(Jobs)).Length];
		System.Array.Clear(jobs, 0, jobs.Length);

		//Debug.Log(FindObjectOfType<GameManager>().factoryPrefab.GetComponent<factoryBehaviour>().neededMaterial);

	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < System.Enum.GetNames(typeof(Jobs)).Length; i++) {
			transform.GetChild(2).GetChild(i + 1).GetChild(1).GetComponent<Text>().text = jobs[i].ToString();
		}

		//update alien wave progression bar
		Mathf.Clamp(alienProgress, 0, 100);
		transform.GetChild(2).GetChild(0).GetChild(0).localScale = new Vector3(alienProgress/100f, transform.localScale.y, transform.localScale.z);
		transform.GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = alienWave.ToString();

		//set the required resources for all buildings on buttons
		Color temp = new Color();
		float colorChange = (1f/255f)*20f;
		//factory
		temp = factory;
		if (transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.FACTORY);
		}
		else transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().factoryPrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().factoryPrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().factoryPrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food
		//quarry
		temp = quary;
		if (transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.QUARY);
		}
		else transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().quaryPrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().quaryPrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().quaryPrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food
		//defense
		temp = defence;
		if (transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.DEFENCE);
		}
		else transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().defencePrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().defencePrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().defencePrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food
		//food processor
		temp = foodProcessor;
		if (transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.FOOD_PROCESSOR);
		}
		else transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().color = foodProcessor;
		transform.GetChild(0).GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().foodProcessorPrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().foodProcessorPrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(3).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().foodProcessorPrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food
		//cloner
		temp = cloner;
		if (transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.CLONER);
		}
		else transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Image>().color = cloner;
		transform.GetChild(0).GetChild(0).GetChild(4).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().clonerPrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(4).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().clonerPrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(4).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().clonerPrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food
		//research
		temp = research;
		if (transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Toggle>().isOn)
		{
			transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Image>().color = new Color(temp.r + colorChange, temp.g + colorChange, temp.b + colorChange);
			GetComponentInParent<GameManager>().setCurrentBuild(Building.RESEARCH);
		}
		else transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Image>().color = temp;
		transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Image>().color = research;
		transform.GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetChild(0).GetComponent<Text>().text = FindObjectOfType<GameManager>().researchPrefab.GetComponent<buildingBehaviour>().neededMaterial.ToString();//material
		transform.GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetChild(1).GetComponent<Text>().text = FindObjectOfType<GameManager>().researchPrefab.GetComponent<buildingBehaviour>().neededMineral.ToString();//mineral
		transform.GetChild(0).GetChild(0).GetChild(5).GetChild(1).GetChild(2).GetComponent<Text>().text = FindObjectOfType<GameManager>().researchPrefab.GetComponent<buildingBehaviour>().neededFood.ToString();//food


		//set available resources numbers on screen
		transform.GetChild(2).GetChild(9).GetChild(0).GetComponent<Text>().text = avalibleMaterial.ToString();
		transform.GetChild(2).GetChild(9).GetChild(1).GetComponent<Text>().text = avalibleMineral.ToString();
		transform.GetChild(2).GetChild(9).GetChild(2).GetComponent<Text>().text = avalibleFood.ToString();

		//set research level
		transform.GetChild(2).GetChild(10).GetChild(1).GetComponent<Text>().text = GetComponentInParent<GameManager>().getResearchLevel().ToString();
		transform.GetChild(2).GetChild(10).GetChild(2).GetChild(0).localScale = new Vector3(GetComponentInParent<GameManager>().getResearchProgress() / 100f, transform.localScale.y, transform.localScale.z);
	}
}
