using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave {

    public int gameID {
        get;
        set;
    }
    public int alienWave
    {
        get;
        set;
    }
    public int resurchLevel
    {
        get;
        set;
    }



    [System.Serializable]
    public struct building
    {
        bool defult;
        public int recivedMaterial { get; }
        public int recivedMineral { get; }
        public int recivedFood { get; }
        public int material { get; }
        public int mineral { get; }
        public int food { get; }
        public Building buildingType { get; }
        public int position { get; }
        public int[] workers { get; }

        public building(int recivedMaterial, int recivedMineral, int recivedFood, int material, int mineral, int food, Building buildingType, int position, int[] workers)
        {
            defult = true;
            this.recivedMaterial = recivedMaterial;
            this.recivedMineral = recivedMineral;
            this.recivedFood = recivedFood;
            this.material = material;
            this.mineral = mineral;
            this.food = food;
            this.buildingType = buildingType;
            this.position = position;
            this.workers = workers;
        }

        public bool isNull()
        {
            return !defult;
        }
    }
    private building[]  buildings;
    public bool addBuilding(int recivedMaterial, int recivedMineral, int recivedFood, int material, int mineral, int food, Building buildingType, int position, int[] workers)
    {
        building temp = new building(recivedMaterial, recivedMineral, recivedFood, material, mineral, food, buildingType, position, workers);
        for(int i = 0; i < buildings.Length; i++)
        {
            if(buildings[i].isNull())
            {
                buildings[i] = temp;
                return true;
            }
        }
        return false;
    }
    public building getBuilding(int i)
    {
        return buildings[i];
    }



    [System.Serializable]
    public struct planetResource
    {
        bool defult;
        public Resources resourceType { get; }
        public int position { get; }
        public float health { get; }

        public planetResource(Resources resourceType, int position, float health)
        {
            defult = true;
            this.resourceType = resourceType;
            this.position = position;
            this.health = health;
        }

        public bool isNull()
        {
            return !defult;
        }
    }
    private planetResource[] planetResources;
    public bool addPlanetResources(Resources resourceType, int position, float health)
    {
        planetResource temp = new planetResource(resourceType, position, health);
        for (int i = 0; i < planetResources.Length; i++)
        {
            if (planetResources[i].isNull())
            {
                planetResources[i] = temp;
                return true;
            }
        }
        return false;
    }
    public planetResource getplanetResource(int i)
    {
        return planetResources[i];
    }


    [System.Serializable]
    public struct worker
    {
        bool defult;
        public int ID { get; }
        public int position { get; }
        public float health { get; }
        public Jobs job { get; }
        public Resources holdingResource { get; }

        public worker(int ID, int position, float health, Jobs job, Resources holdingResource)
        {
            defult = true;
            this.ID = ID;
            this.position = position;
            this.health = health;
            this.job = job;
            this.holdingResource = holdingResource;
        }

        public bool isNull()
        {
            return !defult;
        }
    }
    private worker[] workers;
    public bool addWorker(int ID, int position, float health, Jobs job, Resources holdingResource)
    {
        worker temp = new worker(ID, position, health, job, holdingResource);
        for (int i = 0; i < workers.Length; i++)
        {
            if (workers[i].isNull())
            {
                workers[i] = temp;
                return true;
            }
        }
        return false;
    }
    public worker getworker(int i)
    {
        return workers[i];
    }


    public GameSave(int buildingNum, int planetResourcesNum, int workersNum)
    {
        buildings = new building[buildingNum];
        planetResources = new planetResource[planetResourcesNum];
        workers = new worker[workersNum];
    }
    public int buildingNum()
    {
        return buildings.Length;
    }
    public int planetResourcesNum()
    {
        return planetResources.Length;
    }
    public int workersNum()
    {
        return workers.Length;
    }

}
