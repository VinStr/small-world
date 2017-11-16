using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloningBehaviour : buildingBehaviour {

	private bool full = false;
	public bool getFull()
	{
		return full;
	}
	private List<GameObject> workers = new List<GameObject>(4);

	public int workerFood = 3;
	private int food = 0;
	public bool addFood()
	{
		if (!full)
		{
			food++;
			return true;
		}
		else return false;
	}

	public void addWorker(Vector3 point)
	{
		GameObject worker = Instantiate(GetComponentInParent<GameManager>().workerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		worker.GetComponent<workerBehaviour>().trianglesA = GetComponentInParent<GameManager>().getTriangles();
		worker.GetComponent<workerBehaviour>().goToPosition(point, 0);
		worker.transform.rotation = transform.rotation;
		workers.Add(worker);
		GetComponentInParent<GameManager>().addWorker(worker);

	}

	void Awake()
	{
		building = Building.CLONER;
	}

	// Use this for initialization
	void Start () {

		base.Start();
		
	}

	//animation variables
	private float animT = 0;
	private Vector3 top;
	private Vector3 bottom;
	private float animTime = 1f;
	private bool up = true;

	// Update is called once per frame
	void Update () {

		base.Update();

		//animation
		if (animT >= 1)
		{
			up = false;
			animT = 1;
		}
		else if (animT <= 0)
		{
			up = true;
			animT = 0;
		}

		if (up) animT += Time.deltaTime / animTime;
		else animT -= Time.deltaTime / animTime;
		transform.GetChild(0).GetChild(5).position = Vector3.Lerp(bottom, top, animT);

		//add worker when enough food
		if (workers.Count >= 4) full = true;
		else full = false;

		if(!full && food>= workerFood)
		{
			food = 0;
			Vector3 point = new Vector3(0,0,0);
			if (workers.Count == 0) point = transform.GetChild(2).position;
			else if (workers.Count == 1) point = transform.GetChild(3).position;
			else if (workers.Count == 2) point = transform.GetChild(4).position;
			else if (workers.Count == 3) point = transform.GetChild(5).position;

			addWorker(point);
		}


		//if full remove all workers
		if (full)
		{
			foreach (GameObject worker in assingedWorkers)
			{
				unAssingWorker(worker);
			}
			assingedWorkers = new List<GameObject>();
		}

		if (workers.Count < 1) transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
		else transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
		if (workers.Count < 2) transform.GetChild(0).GetChild(7).gameObject.SetActive(true);
		else transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
		if (workers.Count < 3) transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
		else transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
		if (workers.Count < 4) transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
		else transform.GetChild(0).GetChild(9).gameObject.SetActive(false);

	}
}
