using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class progress : MonoBehaviour
{

    public int neededMaterial = 3;
    public int neededMineral = 3;
    public int neededFood = 3;

    private int material = 0;
    public void setMeterial(int material)
    {
        if (material > neededMaterial) this.material = neededMaterial;
        else this.material = material;
    }

    private int mineral = 0;
    public void setmineral(int mineral)
    {
        if (mineral > neededMineral) this.mineral = neededMineral;
        else this.mineral = mineral;
    }

    private int food = 0;
    public void setfood(int food)
    {
        if (food > neededFood) this.food = neededFood;
        else this.food = food;
    }

    public void setActive(bool active)
    {
        gameObject.SetActive(active);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //set material bar
        if (neededMaterial == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else 
        {
            transform.GetChild(0).GetChild(0).localScale = new Vector3(transform.localScale.x, ((float)material / (float)neededMaterial), transform.localScale.z);
        }
        //set mineral bar
        if (neededMineral == 0)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).GetChild(0).localScale = new Vector3(transform.localScale.x, (float)mineral / (float)neededMineral, transform.localScale.z);
        }
        //set food bar
        if (neededFood == 0)
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(2).GetChild(0).localScale = new Vector3(transform.localScale.x, (float)food / (float)neededFood, transform.localScale.z);
        }

        //if (material == neededMaterial && mineral == neededMineral && food == neededFood) gameObject.SetActive(false);


    }
}