using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingBehaviour : MonoBehaviour {

	protected Building building = Building.NULL;
	public Building getBuilding()
	{
		return building;
	}

	protected int currentPos = 0;
	public void setCurrentPos(int pos)
	{
		this.currentPos = pos;
	}
	public int getCurrentPos()
	{
		return this.currentPos;
	}

	public int neededMaterial = 3;
	protected int recivedMaterial = 0;
	public int neededMineral = 3;
	protected int recivedMineral = 0;
	public int neededFood = 3;
	protected int recivedFood = 0;
	public bool addResource(Resources resource)
	{
		if (resource == Resources.MATERIAL)
		{
			if (recivedMaterial == neededMaterial) return false;
			else { recivedMaterial++; return true; }
		}
		else if (resource == Resources.MINERAL)
		{
			if (recivedMineral == neededMineral) return false;
			else { recivedMineral++; return true; }
		}
		else if (resource == Resources.FOOD)
		{
			if (recivedFood == neededFood) return false;
			else { recivedFood++; return true; }
		}
		else if (resource == Resources.NONE) return true;
		else return false;
	}
	public bool setRecivedResource(Resources resource, int value)
	{
		if (resource == Resources.MATERIAL)
		{
			if (recivedMaterial == neededMaterial) return false;
			else { recivedMaterial = value; return true; }
		}
		else if (resource == Resources.MINERAL)
		{
			if (recivedMineral == neededMineral) return false;
			else { recivedMineral = value; return true; }
		}
		else if (resource == Resources.FOOD)
		{
			if (recivedFood == neededFood) return false;
			else { recivedFood = value; return true; }
		}
		else if (resource == Resources.NONE) return true;
		else return false;
	}
	public int getRecivedResource(Resources resource)
	{
		if (resource == Resources.MATERIAL) return recivedMaterial;
		else if (resource == Resources.MINERAL) return recivedMineral;
		else if (resource == Resources.FOOD) return recivedFood;
		else return 0;
	}

	protected bool built = false;
	public bool getBuilt()
	{
		return this.built;
	}

	protected List<GameObject> assingedWorkers = new List<GameObject>(3);
	public bool jobAvalible(GameObject worker)
	{
		if (assingedWorkers.Count >= 3 || assingedWorkers.Contains(worker)) return false;
		else
		{
			return true;
		}
	}
	public bool assingWorker(GameObject worker)
	{
		if (assingedWorkers.Count >= 3 || assingedWorkers.Contains(worker)) return false;
		else
		{
			assingedWorkers.Add(worker);
			return true;
		}
	}
	public bool unAssingWorker(GameObject worker)
	{
		if (!assingedWorkers.Contains(worker)) return false;
		else
		{
			assingedWorkers[assingedWorkers.IndexOf(worker)].GetComponent<workerBehaviour>().resetJob();
			assingedWorkers.Remove(worker);
			return true;
		}
	}
	public bool findWorker(GameObject worker)
	{
		return assingedWorkers.Contains(worker);
	}
	public int[] getWorkers()
	{
		List<int> workerIDs = new List<int>();
		foreach(GameObject worker in assingedWorkers)
		{
			workerIDs.Add(worker.GetComponent<workerBehaviour>().getID());
		}
		return workerIDs.ToArray();
	}

	protected int outputMaterial = 0;
	protected int outputMineral = 0;
	protected int outputFood = 0;
	public bool needResource (Resources resource)
	{
		if (resource == Resources.MATERIAL && recivedMaterial >= neededMaterial) return false;
		else if (resource == Resources.MINERAL && recivedMineral >= neededMineral) return false;
		else if (resource == Resources.FOOD && recivedFood >= neededFood) return false;
		else return true;
	}
	public Resources neededResource(int priority)
	{
		int[] needed = new int[] { neededFood - recivedFood, neededMaterial - recivedMaterial, neededMineral - recivedMineral};
		Resources[] neededRes = new Resources[] { Resources.FOOD, Resources.MATERIAL, Resources.MINERAL };

		int temp = 0;
		Resources temp2 = Resources.NONE;

		for (int write = 0; write < needed.Length; write++)
		{
			for (int sort = 0; sort < needed.Length - 1; sort++)
			{
				if (needed[sort] > needed[sort + 1])
				{
					temp = needed[sort + 1];
					needed[sort + 1] = needed[sort];
					needed[sort] = temp;

					temp2 = neededRes[sort + 1];
					neededRes[sort + 1] = neededRes[sort];
					neededRes[sort] = temp2;
				}
			}
		}

		if (priority > needed.Length)
		{
			Debug.LogError("priority larger than number of resources available, returning none");
			return Resources.NONE;
		}
		return neededRes[needed.Length - 1 - priority];

		
	}
	public int getResource(Resources resource)
	{
		if (resource == Resources.MATERIAL) return outputMaterial;
		if (resource == Resources.MINERAL) return outputMineral;
		if (resource == Resources.FOOD) return outputFood;
		else return 0;
	}
	public bool takeResource(Resources resource)
	{
		if (resource == Resources.MATERIAL && outputMaterial>0)
		{
			outputMaterial--;
			return true;
		}
		if (resource == Resources.MINERAL && outputMineral > 0)
		{
			outputMineral--;
			return true;
		}
		if (resource == Resources.FOOD && outputFood > 0)
		{
			outputFood--;
			return true;
		}
		else return false;
	}
	public void setResource(Resources resource, int value)
	{
		if (resource == Resources.MATERIAL) outputMaterial = value;
		else if (resource == Resources.MINERAL) outputMineral = value;
		else if (resource == Resources.FOOD) outputFood = value;
	}

	// Use this for initialization
	protected void Start()
	{

		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetComponentInChildren<progress>().neededMaterial = this.neededMaterial;
		transform.GetComponentInChildren<progress>().neededMineral = this.neededMineral;
		transform.GetComponentInChildren<progress>().neededFood = this.neededFood;

	}

	protected float t = 0;
	private bool progressOff = false;
	// Update is called once per frame
	protected void Update()
	{
		if (recivedMaterial == neededMaterial && recivedMineral == neededMineral && recivedFood == neededFood)
		{
			if (progressOff == false)
			{
				GetComponentInChildren<progress>().setActive(false);
				progressOff = true;
			}
		}
		else {
			transform.gameObject.GetComponentInChildren<progress>().setMeterial(recivedMaterial);
			transform.gameObject.GetComponentInChildren<progress>().setmineral(recivedMineral);
			transform.gameObject.GetComponentInChildren<progress>().setfood(recivedFood);
		}
	

		//when all resources to build received grow building
		if (recivedMaterial >= neededMaterial && recivedMineral >= neededMineral && recivedFood >= neededFood && t < 1)

		{
			built = true;
			foreach(GameObject worker in assingedWorkers)
			{
				unAssingWorker(worker);
			}
			assingedWorkers = new List<GameObject>();//remove all construction workers from assigned workers list so other workers can join
			transform.GetChild(0).gameObject.SetActive(true);
			t += Time.deltaTime / 1;
			transform.GetChild(0).localPosition = Vector3.Lerp(new Vector3(0, -0.07f, 0), new Vector3(0, 0, 0), t);
			transform.GetChild(0).localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1, 1, 1), t);

		}
	}

}
