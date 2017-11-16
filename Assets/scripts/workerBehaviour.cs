using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class workerBehaviour : MonoBehaviour {

	public static float movmentSpeed = 1f;

	private float health = 100f;
	public void takeDamage()
	{
		health--;
	}
	public float getHealth()
	{
		return health;
	}
	public void setHealth(float health)
	{
		this.health = health;
	}

	public List<triangle> trianglesA = new List<triangle>();
	private Jobs currentJob = Jobs.IDLE;
	public Jobs getCurrentJob()
	{
		return currentJob;
	}
	public void setCurrentJob(Jobs job)
	{
		this.job = null;
		currentJob = job;
	}

	private GameObject job = null;
	public void resetJob()
	{
		job = null;
		if (holdingResource != Resources.NONE)
		{
			GetComponentInParent<GameManager>().getLander().GetComponent<landerBehaviour>().depositResource(holdingResource);
			holdingResource = Resources.NONE;
		}
	}
	public void setJob(GameObject building)
	{
		this.job = building;
	}

	private Resources holdingResource = Resources.NONE;
	public void setHoldingResource(Resources resource)
	{
		this.holdingResource = resource;
		if (this.holdingResource == Resources.NONE)
		{
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(2).gameObject.SetActive(false);
		}
		else if (this.holdingResource == Resources.MATERIAL)
		{
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(2).gameObject.SetActive(false);
		}
		else if (this.holdingResource == Resources.MINERAL)
		{
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(2).gameObject.SetActive(false);
		}
		else if (this.holdingResource == Resources.FOOD)
		{
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			transform.GetChild(2).gameObject.SetActive(true);
		}
	}
	public Resources getHoldingResource()
	{
		return holdingResource;
	}

	private int ID = 0;
	private static int avalibleID = 0;
	public int getID()
	{
		return ID;
	}
	public void setID(int ID)
	{
		this.ID = ID;
	}
	public static void setAvalibleID(int avalibleID)
	{
		workerBehaviour.avalibleID = avalibleID;
	}


	private int currentPos = 0;

	public int getCurrentPos()
	{
		return this.currentPos;
	}

	private Vector3 startPosition;
	private Vector3 targetPosition;

	private Vector3 finalDirection;
	public float timeToReachTarget = 1f;

	public void goToPosition(int index, float time)
	{
		t = 0;
		timeToReachTarget = time;
		startPosition = transform.position;
		//targetPosition = trianglesA[index].findCenter();
		float r1 = Random.Range(0f, 1f);
		float r2 = Random.Range(0f, 1f);
		targetPosition = (1f - Mathf.Sqrt(r1)) * trianglesA[index].scale(0.7f).vertex1 + (Mathf.Sqrt(r1) * (1f - r2)) * trianglesA[index].scale(0.7f).vertex2 + (Mathf.Sqrt(r1) * r2) * trianglesA[index].scale(0.7f).vertex3;
		currentPos = index;

		Vector3 direction = Vector3.Cross(trianglesA[index].vertex2 - trianglesA[index].vertex1, trianglesA[index].vertex3 - trianglesA[index].vertex1);
		//direction += new Vector3(90, 0, 0);
		finalDirection = direction;

		//this.GetComponentInParent<GameManager>().GetComponent<GameManager>().otherWorkers(currentPos);

	}
	public void goToPosition(Vector3 point, float time)
	{
		t = 0;
		timeToReachTarget = time;
		startPosition = transform.position;
		targetPosition = point;
	}

	private bool moving = false;
	private int finalTarget;

	public bool canGetToPos(int index)
	{
		if (index >= trianglesA.Count) return false;
		int simPos = currentPos;
		previousMoves = new List<int>();
		previousMoves.Add(currentPos);
		while (simPos != index)
		{
			List<int> possibleMoves = new List<int>();
			//find all possible moves
			for (int i = 0; i < trianglesA.Count; i++)
			{
				if (i == simPos) continue;
				else if (trianglesA[simPos].matches(trianglesA[i]) == 2 && previousMoves.Contains(i) == false) possibleMoves.Add(i);
			}
			if (possibleMoves.Count == 0)
			{
				return false;
			}
			else {
				float bestDistance = Vector3.Distance(trianglesA[index].findCenter(), trianglesA[possibleMoves[0]].findCenter());
				int move = 0;
				for (int i = 0; i < possibleMoves.Count; i++)
				{
					if (Vector3.Distance(trianglesA[index].findCenter(), trianglesA[possibleMoves[i]].findCenter()) < bestDistance)
					{
						bestDistance = Vector3.Distance(trianglesA[index].findCenter(), trianglesA[possibleMoves[i]].findCenter());
						move = i;
					}
				}
				previousMoves.Add(possibleMoves[move]);
				simPos = possibleMoves[move];
			}
		}
		return true;
	}

	public bool moveToPosition(int index, float time)
	{
		//see if already at position
		if (currentPos == index) return true;

		//see if it is possible to reach position
		if (canGetToPos(index) == false) return false;

		//if can get to target then set variables to start moving
		finalTarget = index;
		timeToReachTarget = time;
		previousMoves = new List<int>();
		previousMoves.Add(currentPos);
		moving = true;
		return true;
		
	}
	private List<int> previousMoves = new List<int>();
	public int findNextTarget()
	{
		List<int> possibleMoves = new List<int>();
		//find all possible moves
		for (int i = 0; i < trianglesA.Count; i++)
		{
			if (i == currentPos) continue;
			else if (trianglesA[currentPos].matches(trianglesA[i]) == 2 && previousMoves.Contains(i) == false) possibleMoves.Add(i);
		}

		if (possibleMoves.Count == 0)
		{
			moving = false;
			return currentPos;
		}
		else {
			float bestDistance = Vector3.Distance(trianglesA[finalTarget].findCenter(), trianglesA[possibleMoves[0]].findCenter());
			int move = 0;
			for (int i = 0; i < possibleMoves.Count; i++)
			{
				if (Vector3.Distance(trianglesA[finalTarget].findCenter(), trianglesA[possibleMoves[i]].findCenter()) < bestDistance)
				{
					bestDistance = Vector3.Distance(trianglesA[finalTarget].findCenter(), trianglesA[possibleMoves[i]].findCenter());
					move = i;
				}
			}
			previousMoves.Add(possibleMoves[move]);
			return possibleMoves[move];
		}
	}

	public int findTriangle(Vector3 point)
	{
		float bestDistance = 10000000000;
		int current = trianglesA.Count + 1;
		for (int i = 0; i < trianglesA.Count; i++)
		{
			if (trianglesA[i].PointInTriangle(point) && Vector3.Distance(Camera.main.transform.position, trianglesA[i].findCenter()) < bestDistance)
			{
				bestDistance = Vector3.Distance(Camera.main.transform.position, trianglesA[i].findCenter());
				current = i;
			}

		}
		if (current < trianglesA.Count + 1)
		{
			return current;
		}
		else return trianglesA.Count + 1;
	}

	// Use this for initialization of variables
	void Awake () {
		startPosition = transform.position;
		targetPosition = transform.position;

		ID = avalibleID;
		avalibleID++;
	}

	void Start()
	{

	}

	private float t;
	// Update is called once per frame
	void Update () {

		//set size to be related to health
		transform.localScale = new Vector3(0.35f + (0.65f * health / 100), 0.35f + (0.65f * health / 100), 0.35f + (0.65f * health / 100)) * 0.1f;
		if (health <= 0)
		{
			GetComponentInParent<GameManager>().destroy(this.gameObject);
			Destroy(this.gameObject);
		}

		//find next target
		if ( moving == true && findTriangle(transform.position) == finalTarget) moving = false;
		if ( moving == true && transform.position == targetPosition)
		{
			goToPosition(findNextTarget(), timeToReachTarget);
		}

		//move to new target in given time
		if (transform.position != targetPosition)
		{
			if (timeToReachTarget == 0) t = 1;
			else t += Time.deltaTime / timeToReachTarget;
			transform.position = Vector3.Lerp(startPosition, targetPosition, t);

			transform.LookAt(finalDirection);
			transform.Rotate(new Vector3(-90, 0, 0));
		}

		//AI to do jobs
		if (job == null) job = transform.GetComponentInParent<GameManager>().findAvalibleJob(currentJob, this.gameObject);
		else if (currentJob == Jobs.CONSTRUCTION)
		{
			if (job.GetComponent<buildingBehaviour>().getBuilt() == true)
			{
				job = null;
				if (holdingResource != Resources.NONE)
				{
					GetComponentInParent<GameManager>().getLander().GetComponent<landerBehaviour>().depositResource(holdingResource);
					holdingResource = Resources.NONE;
				}
			}
			else if (holdingResource == Resources.NONE)
			{
				Resources resourceToFind = Resources.NONE;
				for (int i = 0; i < 3; i++)
				{
					resourceToFind = job.GetComponent<buildingBehaviour>().neededResource(i);
					if (resourceToFind != Resources.NONE && !(GetComponentInParent<GameManager>().findAvalibleResource(resourceToFind, this.gameObject) == null)) break;
				}
				if (job.GetComponent<buildingBehaviour>().needResource(resourceToFind) == false) resourceToFind = Resources.NONE;
				GameObject resourceObject = GetComponentInParent<GameManager>().findAvalibleResource(resourceToFind, this.gameObject);
				if (resourceObject != null)
				{
					if (currentPos == resourceObject.GetComponent<buildingBehaviour>().getCurrentPos() && moving == false)
					{
						if (resourceObject.GetComponent<buildingBehaviour>().takeResource(resourceToFind)) setHoldingResource(resourceToFind);
					}
					else {
						moveToPosition(resourceObject.GetComponent<buildingBehaviour>().getCurrentPos(), movmentSpeed);
					}
				}
			}
			else
			{
				if (!job.GetComponent<buildingBehaviour>().needResource(holdingResource))
				{
					GetComponentInParent<GameManager>().getLander().GetComponent<landerBehaviour>().depositResource(holdingResource);
					holdingResource = Resources.NONE;
				}

				if (currentPos == job.GetComponent<buildingBehaviour>().getCurrentPos() && moving == false)
				{
					if (job.GetComponent<buildingBehaviour>().addResource(holdingResource)) setHoldingResource(Resources.NONE);
				}
				else {
					moveToPosition(job.GetComponent<buildingBehaviour>().getCurrentPos(), movmentSpeed);
				}
			}


		}
		else if (currentJob == Jobs.CLONING)
		{
			if (job.GetComponent<cloningBehaviour>().getFull() == true)
			{
				job = null;
				if (holdingResource != Resources.NONE)
				{
					GetComponentInParent<GameManager>().getLander().GetComponent<landerBehaviour>().depositResource(holdingResource);
					holdingResource = Resources.NONE;
				}
			}
			else if (holdingResource == Resources.NONE)
			{
				Resources resourceToFind = Resources.FOOD;
				GameObject resourceObject = GetComponentInParent<GameManager>().findAvalibleResource(resourceToFind, this.gameObject);
				if (resourceObject != null)
				{
					if (currentPos == resourceObject.GetComponent<buildingBehaviour>().getCurrentPos() && moving == false)
					{
						if (resourceObject.GetComponent<buildingBehaviour>().takeResource(resourceToFind)) setHoldingResource(resourceToFind);
					}
					else {
						moveToPosition(resourceObject.GetComponent<buildingBehaviour>().getCurrentPos(), movmentSpeed);
					}
				}
			}
			else
			{
				if (currentPos == job.GetComponent<buildingBehaviour>().getCurrentPos() && moving == false)
				{
					if (job.GetComponent<cloningBehaviour>().addFood()) setHoldingResource(Resources.NONE);
				}
				else {
					moveToPosition(job.GetComponent<buildingBehaviour>().getCurrentPos(), movmentSpeed);
				}
			}
		}
		else
		{
			moveToPosition(job.GetComponent<buildingBehaviour>().getCurrentPos(), movmentSpeed);
		}


	}

}