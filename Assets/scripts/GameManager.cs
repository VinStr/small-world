using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public enum Jobs
{
	IDLE,
	CONSTRUCTION,
	DEFENCE,
	FACTORY,
	QUARY,
	FOOD,
	CLONING,
	RESEARCH,
	
};

[System.Serializable]
public enum Building
{
	NULL,
	FACTORY,
	WORKER,
	DEFENCE,
	QUARY,
	FOOD_PROCESSOR,
	CLONER,
	RESEARCH,
	LANDER,
	DIRECT
};

[System.Serializable]
public enum Resources
{
	NONE,
	MATERIAL,
	MINERAL,
	FOOD
};

public struct triangle
{
	public Vector3 vertex1;
	public Vector3 vertex2;
	public Vector3 vertex3;

	public triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
	{
		this.vertex1 = vertex1;
		this.vertex2 = vertex2;
		this.vertex3 = vertex3;
	}

	public bool isIn(Vector3 vertex)//true if the given vector is one of the ones in the structure
	{
		if (vertex == vertex1 || vertex == vertex2 || vertex == vertex3) return true;
		else return false;
	}

	public int matches(triangle a)//returns number of vertices's the give triangle and this one share
	{
		int matches = 0;
		if (isIn(a.vertex1)) matches++;
		if (isIn(a.vertex2)) matches++;
		if (isIn(a.vertex3)) matches++;
		return matches;
	}
	public Vector3 findCenter()//returns center of the triangle
	{
		float sumX = vertex1.x + vertex2.x + vertex3.x;
		float sumY = vertex1.y + vertex2.y + vertex3.y;
		float sumZ = vertex1.z + vertex2.z + vertex3.z;
		return new Vector3(sumX / 3, sumY / 3, sumZ / 3);
	}

	//returns the singed area for the triangle from the given vectors
	public float sign(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}

	public bool PointInTriangle(Vector3 point)//finds if the point given exits in the given triangles plane
	{
		Vector3 v1 = vertex1, v2 = vertex2, v3 = vertex3;
		bool b1, b2, b3;

		b1 = sign(point, v1, v2) < 0.0f;
		b2 = sign(point, v2, v3) < 0.0f;
		b3 = sign(point, v3, v1) < 0.0f;

		return ((b1 == b2) && (b2 == b3));
	}

	public triangle scale(float factor)//returns a new triangle scaled by the given scale factor with the center being the reference of enlargement
	{
		Vector3 newVertex1 = factor * (vertex1 - this.findCenter()) + this.findCenter();
		Vector3 newVertex2 = factor * (vertex2 - this.findCenter()) + this.findCenter();
		Vector3 newVertex3 = factor * (vertex3 - this.findCenter()) + this.findCenter();

		return new triangle(newVertex1, newVertex2, newVertex3);
	}

	public float edgeRadius()
	{
		float c = Vector3.Distance(vertex1, findCenter());
		float b = Vector3.Distance(vertex1, vertex2) / 2f;
		float a = Mathf.Sqrt(Mathf.Pow(c, 2) - Mathf.Pow(b, 2));
		return a;
	}
}

public class GameManager : MonoBehaviour {

	public GameObject upperPane;
	public GameObject lowerPane;

	public int gameID = 0;

	private List<triangle> planetTriangles = new List<triangle>();
	public List<triangle> getTriangles()
	{
		return planetTriangles;
	}
	public triangle getTriangle (int index)
	{
		return planetTriangles[index];
	}

	public GameObject alienPrefab;
	private List<GameObject> aliens = new List<GameObject>();
	private int alienWave = 0;
	private bool invasion = false;
	private float alienProgress = 0;
	public void addAlien()
	{
		GameObject alien = Instantiate(alienPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		alien.transform.parent = transform;
		aliens.Add(alien);
	}

	private int researchLevel = 0;
	public int getResearchLevel()
	{
		return researchLevel;
	}
	public void incResearchLevel()
	{
		researchLevel++;
	}
	public float getResearchProgress()
	{
		float topProgress = 0;
		foreach(GameObject building in buildings)
		{
			if (building.GetComponent<researchBehaviour>() != null && building.GetComponent<researchBehaviour>().getResearchProgress() > topProgress)
			{
				topProgress = building.GetComponent<researchBehaviour>().getResearchProgress();
			}
		}
		return topProgress;
	}

	public GameObject workerPrefab;
	public GameObject landerPrefab;
	public GameObject factoryPrefab;
	public GameObject defencePrefab;
	public GameObject quaryPrefab;
	public GameObject foodProcessorPrefab;
	public GameObject clonerPrefab;
	public GameObject researchPrefab;
	public GameObject orePrefab;
	public GameObject pondPrefab;

	private GameObject lander;
	public GameObject getLander()
	{
		return this.lander;
	}

	private List<GameObject> workers = new List<GameObject>();

	private List<int> oresPos;
	private List<GameObject> ores;
	public bool oreAt(int position)
	{
		for(int i = 0; i < oresPos.Count; i++)
		{
			if (oresPos[i] == position) return true;
		}
		return false;
	}

	private List<int> pondsPos;
	private List<GameObject> ponds;
	public bool pondAt(int position)
	{
		for (int i = 0; i < pondsPos.Count; i++)
		{
			if (pondsPos[i] == position) return true;
		}
		return false;
	}
	public float oreDist = 0.1f;//distribution of ores on planet
	public float pondDist = 0.1f;//distribution of ponds on planet
	private List<GameObject> buildings = new List<GameObject>();

	public void destroy( GameObject entity)//remove all reference to the object then destroy it
	{
		if (entity.tag == "Worker")
		{
			for (int i = 0; i < buildings.Count; i++)
			{
				if (buildings[i].GetComponent<buildingBehaviour>().findWorker(entity))
				{
					buildings[i].GetComponent<buildingBehaviour>().unAssingWorker(entity);
					break;
				}
			}
			workers.Remove(entity);
		}
		else if (entity.tag == "Ore") { oresPos.RemoveAt(ores.IndexOf(entity)); ores.Remove(entity); }
		else if (entity.tag == "Building") buildings.Remove(entity);
		else if (entity.tag == "Alien") aliens.Remove(entity);
		

		Destroy(entity);

	}

	private Building currentBuild = Building.NULL;
	public void setCurrentBuild(Building building)
	{
		this.currentBuild = building;
	}
	public void setCurrentBuild(int building)
	{
		if (building == 0) this.currentBuild = Building.NULL;
		else if (building == 1) this.currentBuild = Building.FACTORY;
		else if (building == 2) this.currentBuild = Building.WORKER;
		else if (building == 3) this.currentBuild = Building.DEFENCE;
		else if (building == 4) this.currentBuild = Building.QUARY;
		else if (building == 5) this.currentBuild = Building.FOOD_PROCESSOR;
		else if (building == 6) this.currentBuild = Building.CLONER;
		else if (building == 7) this.currentBuild = Building.RESEARCH;
		else if (building == 8) this.currentBuild = Building.DIRECT;
	}

	public void addWorker(int start)
	{
		GameObject worker = Instantiate(workerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		worker.transform.parent = transform;
		worker.GetComponent<workerBehaviour>().trianglesA = planetTriangles;
		worker.GetComponent<workerBehaviour>().goToPosition(start,0);
		workers.Add(worker);
	}
	public void addWorker(Vector3 point)
	{
		GameObject worker = Instantiate(workerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		worker.transform.parent = transform;
		worker.GetComponent<workerBehaviour>().trianglesA = planetTriangles;
		worker.GetComponent<workerBehaviour>().goToPosition(point, 0);
		workers.Add(worker);
	}
	public void addWorker(GameObject worker)
	{
		worker.transform.parent = transform;
		workers.Add(worker);
	}
	public GameObject findWorker(int ID)
	{
		foreach(GameObject worker in workers)
		{
			if (worker.GetComponent<workerBehaviour>().getID() == ID) return worker;
		}
		return null;
	}

	public void addBuilding(int start, Building building)
	{
		GameObject buildingTemp;
		if (building == Building.FACTORY) buildingTemp = Instantiate(factoryPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		else if (building == Building.QUARY) {  buildingTemp = Instantiate(quaryPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)); buildingTemp.GetComponent<quaryBeheviour>().setOre(ores[oresPos.IndexOf(start)]);  }
		else if (building == Building.FOOD_PROCESSOR) { buildingTemp = Instantiate(foodProcessorPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)); buildingTemp.GetComponent<foodProcessorBehaviour>().setPond(ponds[pondsPos.IndexOf(start)]); }
		else if (building == Building.DEFENCE) buildingTemp = Instantiate(defencePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		else if (building == Building.CLONER) buildingTemp = Instantiate(clonerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		else if (building == Building.RESEARCH) buildingTemp = Instantiate(researchPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
		else {
			Debug.LogError("no building type set");
			return;
		}
		buildingTemp.transform.parent = transform;
		buildingBehaviour buildingTempBehaviour = buildingTemp.GetComponent<buildingBehaviour>();
		buildingTempBehaviour.setCurrentPos(start);
		buildingTemp.transform.position = planetTriangles[start].findCenter();
		Vector3 direction = Vector3.Cross(planetTriangles[start].vertex2 - planetTriangles[start].vertex1, planetTriangles[start].vertex3 - planetTriangles[start].vertex1);
		buildingTemp.transform.rotation = Quaternion.LookRotation(direction);
		buildingTemp.transform.Rotate(new Vector3(90, 0, 0));
		buildings.Add(buildingTemp);
	}

	public GameObject findAvalibleJob(Jobs job, GameObject worker)
	{

		float bestDistance = 10000;
		int bestIndex = -1;

		if (job == Jobs.CONSTRUCTION)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<buildingBehaviour>().getBuilt() == false &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.DEFENCE)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<TeslaTowerBehaviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.FACTORY)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<factoryBehaviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.QUARY)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<quaryBeheviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.FOOD)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<foodProcessorBehaviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.CLONING)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<cloningBehaviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					buildings[j].GetComponent<cloningBehaviour>().getFull() == false &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		else if (job == Jobs.RESEARCH)
		{
			for (int j = 0; j < buildings.Count; j++)
			{
				if (buildings[j].GetComponent<researchBehaviour>() != null &&
					buildings[j].GetComponent<buildingBehaviour>().getBuilt() == true &&
					worker.GetComponent<workerBehaviour>().canGetToPos(buildings[j].GetComponent<buildingBehaviour>().getCurrentPos()) &&
					buildings[j].GetComponent<buildingBehaviour>().jobAvalible(worker))
				{
					float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
					if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
				}
			}
			if (bestIndex >= 0 && buildings[bestIndex].GetComponent<buildingBehaviour>().assingWorker(worker)) return buildings[bestIndex];
		}
		return null;
	}

	public GameObject findAvalibleResource(Resources resource , GameObject worker)
	{
		if (resource == Resources.NONE) return null;
		float bestDistance = 10000;
		int bestIndex = -1;

		if (lander.GetComponent<buildingBehaviour>().getResource(resource) > 0)
		{
			float distance = Vector3.Distance(worker.transform.position, lander.transform.position);
			bestDistance = distance;
			bestIndex = buildings.Count+1; 
		}

		for (int j = 0; j < buildings.Count; j++)
		{

			if (buildings[j].GetComponent<buildingBehaviour>().getResource(resource) > 0)
			{
				float distance = Vector3.Distance(worker.transform.position, buildings[j].transform.position);
				if (distance < bestDistance) { bestDistance = distance; bestIndex = j; }
			}  
		}
		if (bestIndex == buildings.Count + 1) return lander;
		else if(bestIndex>=0)return buildings[bestIndex];
		else return null;
	}

	/// <summary>
	/// change selection color
	/// </summary>
	/// <param name="color"></param>
	public void changeColor(int color)
	{
		if (color == 0) this.color = Color.green;
		else if (color == 1) this.color = Color.red;
		else if (color == 2) this.color = Color.blue;
	}

	//find current number of workers in triangle and reposition them
	public int otherWorkers(int index)
	{
		int count = 0;
		for (int i = 1; i< workers.Count; i++){
			if (workers[i].GetComponent<workerBehaviour>().getCurrentPos() == index) count++;
		}

		if (count > 1)
		{
			for (int i = 1; i < workers.Count; i++)
			{
				if (workers[i].GetComponent<workerBehaviour>().getCurrentPos() == index)
				{
					//P = (1 - sqrt(r1)) * A + (sqrt(r1) * (1 - r2)) * B + (sqrt(r1) * r2) * C
					float r1 = Random.Range(0f, 1f);
					float r2 = Random.Range(0f, 1f);
					Vector3 randomPoint = (1f - Mathf.Sqrt(r1)) * planetTriangles[index].scale(0.7f).vertex1 + (Mathf.Sqrt(r1) * (1f - r2)) * planetTriangles[index].scale(0.7f).vertex2 + (Mathf.Sqrt(r1) * r2) * planetTriangles[index].scale(0.7f).vertex3;
					workers[i].GetComponent<workerBehaviour>().goToPosition(randomPoint, 1);
				}
			}
		}
		return count;
	}

	public int findTriangle(Vector3 point)
	{
		float bestDistance = 10000000000;
		int current = planetTriangles.Count + 1;
		for (int i = 0; i < planetTriangles.Count; i++)
		{
			if (planetTriangles[i].PointInTriangle(point) && Vector3.Distance(Camera.main.transform.position, planetTriangles[i].findCenter()) < bestDistance)
			{
				bestDistance = Vector3.Distance(Camera.main.transform.position, planetTriangles[i].findCenter());
				current = i;
			}

		}
		if (current < planetTriangles.Count + 1)
		{
			return current;
		}
		else return planetTriangles.Count + 1;
	}

	public void save()
	{
		GameSave gameSave = new GameSave(buildings.Count,ores.Count+ponds.Count,workers.Count);
		gameSave.gameID = this.gameID;
		gameSave.alienWave = this.alienWave;
		gameSave.resurchLevel = this.researchLevel;
		foreach(GameObject building in buildings)
		{
			gameSave.addBuilding(
				building.GetComponent<buildingBehaviour>().getRecivedResource(Resources.MATERIAL),
				building.GetComponent<buildingBehaviour>().getRecivedResource(Resources.MINERAL),
				building.GetComponent<buildingBehaviour>().getRecivedResource(Resources.FOOD),
				building.GetComponent<buildingBehaviour>().getResource(Resources.MATERIAL),
				building.GetComponent<buildingBehaviour>().getResource(Resources.MINERAL),
				building.GetComponent<buildingBehaviour>().getResource(Resources.FOOD),
				building.GetComponent<buildingBehaviour>().getBuilding(),
				building.GetComponent<buildingBehaviour>().getCurrentPos(),
				building.GetComponent<buildingBehaviour>().getWorkers());
		}
		for(int i = 0; i<ores.Count;i++)
		{
			gameSave.addPlanetResources(Resources.MINERAL, oresPos[i], ores[i].GetComponent<PlanetResourceBehaviour>().getHealth());
		}
		for (int i = 0; i < ponds.Count; i++)
		{
			gameSave.addPlanetResources(Resources.FOOD, pondsPos[i], ponds[i].GetComponent<PlanetResourceBehaviour>().getHealth());
		}
		foreach (GameObject worker in workers)
		{
			gameSave.addWorker(worker.GetComponent<workerBehaviour>().getID(),
				worker.GetComponent<workerBehaviour>().getCurrentPos(),
				worker.GetComponent<workerBehaviour>().getHealth(),
				worker.GetComponent<workerBehaviour>().getCurrentJob(),
				worker.GetComponent<workerBehaviour>().getHoldingResource());
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/SmallWorldSave" + gameID + ".dat", FileMode.Open);
		bf.Serialize(file, gameSave);
		file.Close();
	}

	public void load()
	{
		GameSave gameSave;

		if (File.Exists(Application.persistentDataPath + "/SmallWorldSave" + gameID + ".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/SmallWorldSave" + gameID + ".dat", FileMode.Open);
			gameSave = (GameSave)bf.Deserialize(file);
			file.Close();
		}
		else
		{
			Debug.LogError("save " + gameID + " not found");
			return;
		}

		this.gameID = gameSave.gameID;
		this.alienWave = gameSave.alienWave;
		this.researchLevel = gameSave.resurchLevel;

		int highestID = 0;
		for(int i = 0; i < gameSave.workersNum(); i++)
		{
			GameObject worker = Instantiate(workerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			worker.transform.parent = transform;
			worker.GetComponent<workerBehaviour>().trianglesA = planetTriangles;
			worker.GetComponent<workerBehaviour>().goToPosition(gameSave.getworker(i).position, 0);
			worker.GetComponent<workerBehaviour>().setHealth(gameSave.getworker(i).health);
			worker.GetComponent<workerBehaviour>().setCurrentJob(gameSave.getworker(i).job);
			worker.GetComponent<workerBehaviour>().setHoldingResource(gameSave.getworker(i).holdingResource);
			worker.GetComponent<workerBehaviour>().setID(gameSave.getworker(i).ID);
			if (gameSave.getworker(i).ID > highestID) highestID = gameSave.getworker(i).ID;
			workers.Add(worker);
		}
		workerBehaviour.setAvalibleID(highestID + 1);
		for (int i = 0; i < gameSave.planetResourcesNum(); i++)
		{
			if(gameSave.getplanetResource(i).resourceType == Resources.MINERAL)
			{
				oresPos.Add(gameSave.getplanetResource(i).position);
				Vector3 direction = Vector3.Cross(planetTriangles[gameSave.getplanetResource(i).position].vertex2 - planetTriangles[gameSave.getplanetResource(i).position].vertex1, planetTriangles[gameSave.getplanetResource(i).position].vertex3 - planetTriangles[gameSave.getplanetResource(i).position].vertex1);
				GameObject ore = Instantiate(orePrefab, planetTriangles[gameSave.getplanetResource(i).position].findCenter(), new Quaternion(0, 0, 0, 0));
				ore.transform.parent = transform;
				ore.transform.LookAt(direction);
				ore.transform.Rotate(new Vector3(-90, 0, 0));
				ore.GetComponent<PlanetResourceBehaviour>().setHealth(gameSave.getplanetResource(i).health);
				ores.Add(ore);
			}
			else if(gameSave.getplanetResource(i).resourceType == Resources.FOOD)
			{
				pondsPos.Add(gameSave.getplanetResource(i).position);
				Vector3 direction = Vector3.Cross(planetTriangles[gameSave.getplanetResource(i).position].vertex2 - planetTriangles[gameSave.getplanetResource(i).position].vertex1, planetTriangles[gameSave.getplanetResource(i).position].vertex3 - planetTriangles[gameSave.getplanetResource(i).position].vertex1);
				GameObject pond = Instantiate(pondPrefab, planetTriangles[gameSave.getplanetResource(i).position].findCenter(), new Quaternion(0, 0, 0, 0));
				pond.transform.parent = transform;
				pond.transform.LookAt(direction);
				pond.transform.Rotate(new Vector3(-90, 0, 0));
				pond.GetComponent<PlanetResourceBehaviour>().setHealth(gameSave.getplanetResource(i).health);
				ponds.Add(pond);
			}
		}
		for (int i = 0; i < gameSave.buildingNum(); i++)
		{
			Building building = gameSave.getBuilding(i).buildingType;
			int start = gameSave.getBuilding(i).position;
			GameObject buildingTemp;
			if (building == Building.FACTORY) buildingTemp = Instantiate(factoryPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			else if (building == Building.QUARY) { buildingTemp = Instantiate(quaryPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)); buildingTemp.GetComponent<quaryBeheviour>().setOre(ores[oresPos.IndexOf(start)]); }
			else if (building == Building.FOOD_PROCESSOR) { buildingTemp = Instantiate(foodProcessorPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)); buildingTemp.GetComponent<foodProcessorBehaviour>().setPond(ponds[pondsPos.IndexOf(start)]); }
			else if (building == Building.DEFENCE) buildingTemp = Instantiate(defencePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			else if (building == Building.CLONER) buildingTemp = Instantiate(clonerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			else if (building == Building.RESEARCH) buildingTemp = Instantiate(researchPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
			else {
				Debug.LogError("no building type set");
				return;
			}
			buildingTemp.transform.parent = transform;
			buildingBehaviour buildingTempBehaviour = buildingTemp.GetComponent<buildingBehaviour>();
			buildingTempBehaviour.setCurrentPos(start);
			buildingTemp.transform.position = planetTriangles[start].findCenter();
			Vector3 direction = Vector3.Cross(planetTriangles[start].vertex2 - planetTriangles[start].vertex1, planetTriangles[start].vertex3 - planetTriangles[start].vertex1);
			buildingTemp.transform.rotation = Quaternion.LookRotation(direction);
			buildingTemp.transform.Rotate(new Vector3(90, 0, 0));
			buildingTemp.GetComponent<buildingBehaviour>().setRecivedResource(Resources.MATERIAL, gameSave.getBuilding(i).recivedMaterial);
			buildingTemp.GetComponent<buildingBehaviour>().setRecivedResource(Resources.MINERAL, gameSave.getBuilding(i).recivedMineral);
			buildingTemp.GetComponent<buildingBehaviour>().setRecivedResource(Resources.FOOD, gameSave.getBuilding(i).recivedFood);
			buildingTemp.GetComponent<buildingBehaviour>().setResource(Resources.MATERIAL, gameSave.getBuilding(i).material);
			buildingTemp.GetComponent<buildingBehaviour>().setResource(Resources.MINERAL, gameSave.getBuilding(i).mineral);
			buildingTemp.GetComponent<buildingBehaviour>().setResource(Resources.FOOD, gameSave.getBuilding(i).food);
			for(int j = 0; j< gameSave.getBuilding(i).workers.Length; j++)
			{
				GameObject worker = findWorker(gameSave.getBuilding(i).workers[j]);
				worker.GetComponent<workerBehaviour>().setJob(buildingTemp);
				buildingTemp.GetComponent<buildingBehaviour>().assingWorker(worker);
			}
			buildings.Add(buildingTemp);
		}


	}

	private float defultMovmentSpeed = workerBehaviour.movmentSpeed;
	private float defultResourceTimeFactory = factoryBehaviour.resourceTime;
	private float defultResourceTimeQuary = quaryBeheviour.resourceTime;
	private float defultResourceTimeFood = foodProcessorBehaviour.resourceTime;
	private float defultDamageRate = TeslaTowerBehaviour.damageRate;
	private float defultReasurchRate = researchBehaviour.reasurchRate;
	void Awake()
	{
		ores = new List<GameObject>((int)Mathf.Round(planetTriangles.Count * oreDist));
		oresPos = new List<int>((int)Mathf.Round(planetTriangles.Count * oreDist));
		ponds = new List<GameObject>((int)Mathf.Round(planetTriangles.Count * pondDist));
		pondsPos = new List<int>((int)Mathf.Round(planetTriangles.Count * pondDist));
	}

	// Use this for initialization
	void Start () {
		planetTriangles = transform.GetComponentInChildren<VertexMapping>().trianglesA;
		
		//create lander
		lander = Instantiate(landerPrefab, new Vector3(0,0,0), new Quaternion(0, 0, 0, 0));
		lander.transform.parent = transform;
		lander.GetComponent<landerBehaviour>().land(planetTriangles[0], transform.GetChild(0).position);
		
		//add all ores to world
		for (int i = 0; i< (int)Mathf.Round(planetTriangles.Count * oreDist); i++)
		{
			int index = (int)Mathf.Round(UnityEngine.Random.Range(0, planetTriangles.Count - 1));
			while (oresPos.Contains(index) || index == lander.GetComponent<buildingBehaviour>().getCurrentPos()){
				index = (int)Mathf.Round(UnityEngine.Random.Range(0, planetTriangles.Count - 1));
			}
			oresPos.Add(index);
			Vector3 direction = Vector3.Cross(planetTriangles[index].vertex2 - planetTriangles[index].vertex1, planetTriangles[index].vertex3 - planetTriangles[index].vertex1);
			GameObject ore = Instantiate(orePrefab, planetTriangles[index].findCenter(), new Quaternion(0, 0, 0, 0));
			ore.transform.parent = transform;
			ore.transform.LookAt(direction);
			ore.transform.Rotate(new Vector3(-90, 0, 0));
			ores.Add(ore);
		}
		for (int i = 0; i < (int)Mathf.Round(planetTriangles.Count * pondDist); i++)
		{
			int index = (int)Mathf.Round(UnityEngine.Random.Range(0, planetTriangles.Count - 1));
			while (pondsPos.Contains(index) || index == lander.GetComponent<buildingBehaviour>().getCurrentPos() || oresPos.Contains(index))
			{
				index = (int)Mathf.Round(UnityEngine.Random.Range(0, planetTriangles.Count - 1));
			}
			pondsPos.Add(index);
			Vector3 direction = Vector3.Cross(planetTriangles[index].vertex2 - planetTriangles[index].vertex1, planetTriangles[index].vertex3 - planetTriangles[index].vertex1);
			GameObject pond = Instantiate(pondPrefab, planetTriangles[index].findCenter(), new Quaternion(0, 0, 0, 0));
			pond.transform.parent = transform;
			pond.transform.LookAt(direction);
			pond.transform.Rotate(new Vector3(-90, 0, 0));
			ponds.Add(pond);
		}



	}

	private Color color = Color.green;

	private Vector3 touchStart = new Vector3(0, 0, 0);
	private Vector3 touchEnd = new Vector3(1, 1, 1);
   
	private Jobs from;
	private Jobs to;
	private bool nogo = false;
	public GameObject workerIconPrefab;
	private GameObject workerIcon;

	public static Rect RectToScreen(RectTransform transform)
	{
		Rect parent = new Rect(0, 0, Screen.width, Screen.height);
		if (transform.parent.GetComponent<RectTransform>() != null)
		{
			parent = RectToScreen(transform.parent.GetComponent<RectTransform>());
		}
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Vector2 anchor = (transform.anchorMin + transform.anchorMax) / 2;

		float x = parent.x + transform.rect.x + transform.position.x;
		float y = parent.y + transform.rect.y + transform.position.y;

		return new Rect(x, y, size.x, size.y);
	}

	// Update is called once per frame
	void Update()
	{
		//register triangle selection
		if (Input.touchCount == 1) // user is touching the screen with a single touch
		{
			Touch touch = Input.GetTouch(0);
			if (RectToScreen(upperPane.GetComponent<RectTransform>()).Contains(touch.position))//make sure its not in the GUI sections 
			{

			}
			else if (RectToScreen(lowerPane.GetComponent<RectTransform>()).Contains(touch.position))
			{
				if (touch.phase == TouchPhase.Began)
				{
					nogo = false;
					if (RectToScreen(lowerPane.transform.GetChild(1).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.IDLE;
					else if (RectToScreen(lowerPane.transform.GetChild(2).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.CONSTRUCTION;
					else if (RectToScreen(lowerPane.transform.GetChild(3).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.DEFENCE;
					else if (RectToScreen(lowerPane.transform.GetChild(4).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.FACTORY;
					else if (RectToScreen(lowerPane.transform.GetChild(5).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.QUARY;
					else if (RectToScreen(lowerPane.transform.GetChild(6).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.FOOD;
					else if (RectToScreen(lowerPane.transform.GetChild(7).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.CLONING;
					else if (RectToScreen(lowerPane.transform.GetChild(8).GetComponent<RectTransform>()).Contains(touch.position)) from = Jobs.RESEARCH;
					else nogo = true;

					workerIcon = Instantiate(workerIconPrefab, touch.position, new Quaternion(0, 0, 0, 0), lowerPane.transform);
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					workerIcon.transform.position = touch.position;
				}
				else if (touch.phase == TouchPhase.Ended)
				{
					if (RectToScreen(lowerPane.transform.GetChild(1).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.IDLE;
					else if (RectToScreen(lowerPane.transform.GetChild(2).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.CONSTRUCTION;
					else if (RectToScreen(lowerPane.transform.GetChild(3).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.DEFENCE;
					else if (RectToScreen(lowerPane.transform.GetChild(4).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.FACTORY;
					else if (RectToScreen(lowerPane.transform.GetChild(5).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.QUARY;
					else if (RectToScreen(lowerPane.transform.GetChild(6).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.FOOD;
					else if (RectToScreen(lowerPane.transform.GetChild(7).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.CLONING;
					else if (RectToScreen(lowerPane.transform.GetChild(8).GetComponent<RectTransform>()).Contains(touch.position)) to = Jobs.RESEARCH;
					else nogo=true;
					destroy(workerIcon);
					if (nogo!=true)
					{
						for (int i = 0; i < workers.Count; i++)
						{
							if (workers[i].GetComponent<workerBehaviour>().getCurrentJob() == from)
							{
								workers[i].GetComponent<workerBehaviour>().setCurrentJob(to);
								break;
							}
						}
					}
				}
				
			}
			else {
				if (touch.phase == TouchPhase.Began)
				{
					touchStart = touch.position;
				}
				else if (touch.phase == TouchPhase.Ended)
				{
					touchEnd = touch.position;
					if (Vector3.Distance(touchStart, touchEnd) < 50)
					{
						Ray ray = Camera.main.ScreenPointToRay(touch.position);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit))
						{
							int position = transform.GetComponentInChildren<VertexMapping>().findTriangle(hit.point);
							if (hit.transform.gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[7])
							{
								Vector2 screenPos = Camera.main.WorldToScreenPoint(hit.point);
							}

							if (currentBuild == Building.NULL)
							{

							}
							else if (currentBuild == Building.FACTORY)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.magenta);// new Color(116, 32, 132)

								addBuilding(position, Building.FACTORY);
							}
							else if (currentBuild == Building.QUARY)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.green);//new Color(32, 163, 47)
								if(oreAt(position)) addBuilding(position, Building.QUARY);
							}
							else if (currentBuild == Building.FOOD_PROCESSOR)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.yellow);//new Color
								if (pondAt(position)) addBuilding(position, Building.FOOD_PROCESSOR);
							}
							else if (currentBuild == Building.CLONER)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.blue);//new Color
								addBuilding(position, Building.CLONER);
							}
							else if (currentBuild == Building.DEFENCE)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.cyan);//new Color(51, 60, 122)
								addBuilding(position, Building.DEFENCE);
							}
							else if (currentBuild == Building.RESEARCH)
							{
								transform.GetComponentInChildren<VertexMapping>().changeColor(position, Color.white);//new Color(51, 60, 122)
								addBuilding(position, Building.RESEARCH);
							}
							else if (currentBuild == Building.WORKER)
							{
								addWorker(position);
							}
							else if (currentBuild == Building.DIRECT)
							{
								workers[0].GetComponent<workerBehaviour>().moveToPosition(position, workerBehaviour.movmentSpeed);
							}


						}
					}
				}
			}
		}


		//update job worker numbers on GUI
		int idle = 0, construction = 0, defense = 0, factory = 0, quarry = 0, food = 0, cloning = 0, research = 0;
		foreach (GameObject entity in workers)
		{
			if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.IDLE) idle++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.CONSTRUCTION) construction++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.DEFENCE) defense++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.FACTORY) factory++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.QUARY) quarry++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.FOOD) food++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.CLONING) cloning++;
			else if (entity.GetComponent<workerBehaviour>().getCurrentJob() == Jobs.RESEARCH) research++;
		}
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.IDLE, idle);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.CONSTRUCTION, construction);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.DEFENCE, defense);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.FACTORY, factory);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.QUARY, quarry);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.FOOD, food);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.CLONING, cloning);
		transform.GetComponentInChildren<UIBehaviour>().setJob(Jobs.RESEARCH, research);

		//update available resources on GUI
		int avalibleMaterial = 0, avalibleMineral = 0, avalibleFood = 0;
		avalibleMaterial = avalibleMaterial + lander.GetComponent<buildingBehaviour>().getResource(Resources.MATERIAL);
		avalibleMineral = avalibleMineral + lander.GetComponent<buildingBehaviour>().getResource(Resources.MINERAL);
		avalibleFood = avalibleFood + lander.GetComponent<buildingBehaviour>().getResource(Resources.FOOD);
		foreach(GameObject entity in buildings)
		{
			avalibleMaterial = avalibleMaterial + entity.GetComponent<buildingBehaviour>().getResource(Resources.MATERIAL);
			avalibleMineral = avalibleMineral + entity.GetComponent<buildingBehaviour>().getResource(Resources.MINERAL);
			avalibleFood = avalibleFood + entity.GetComponent<buildingBehaviour>().getResource(Resources.FOOD);
		}
		transform.GetComponentInChildren<UIBehaviour>().setAvalibleResource(Resources.MATERIAL, avalibleMaterial);
		transform.GetComponentInChildren<UIBehaviour>().setAvalibleResource(Resources.MINERAL, avalibleMineral);
		transform.GetComponentInChildren<UIBehaviour>().setAvalibleResource(Resources.FOOD, avalibleFood);

		//update alien wave progress
		if(invasion == false) alienProgress += (Time.deltaTime / (60f - (1f / 1.98f) * alienWave)) * 100f;
		if (alienProgress >= 100)
		{
			invasion = true;
			alienProgress = 0;
			alienWave++;
			for(int i = 0; i < Mathf.Round(alienWave * 1.2f); i++)
			{
				addAlien();
			}
		}
		if(aliens.Count == 0 && alienProgress == 0)
		{
			invasion = false;
		}
		transform.GetComponentInChildren<UIBehaviour>().setAlienProgress(alienProgress);
		transform.GetComponentInChildren<UIBehaviour>().setAlienWave(alienWave);


		//change entity's behavior based on research level
		researchLevel = Mathf.Clamp(researchLevel, 0, 5);
		if(researchLevel >= 0)
		{
			workerBehaviour.movmentSpeed = defultMovmentSpeed;
			factoryBehaviour.resourceTime = defultResourceTimeFactory;
			quaryBeheviour.resourceTime = defultResourceTimeQuary;
			foodProcessorBehaviour.resourceTime = defultResourceTimeFood;
			TeslaTowerBehaviour.damageRate = defultDamageRate;
			researchBehaviour.reasurchRate = defultReasurchRate;
		}
		if (researchLevel >= 1)
		{
			workerBehaviour.movmentSpeed = defultMovmentSpeed / 1.5f;
		}
		if (researchLevel >= 2)
		{
			factoryBehaviour.resourceTime = defultResourceTimeFactory / 2;
		}
		if (researchLevel >= 3)
		{
			quaryBeheviour.resourceTime = defultResourceTimeQuary / 2;
			foodProcessorBehaviour.resourceTime = defultResourceTimeFood / 2;
		}
		if (researchLevel >= 4)
		{
			TeslaTowerBehaviour.damageRate = defultDamageRate * 2;
		}
		if (researchLevel >= 5)
		{
			workerBehaviour.movmentSpeed = defultMovmentSpeed / 2;
			factoryBehaviour.resourceTime = defultResourceTimeFactory / 3;
			quaryBeheviour.resourceTime = defultResourceTimeQuary / 3;
			foodProcessorBehaviour.resourceTime = defultResourceTimeFood / 3;
			TeslaTowerBehaviour.damageRate = defultDamageRate * 2.5f;
		}




	}
}
