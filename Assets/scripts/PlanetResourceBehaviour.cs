using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResourceBehaviour : MonoBehaviour {

	protected float health = 100;
	public int decAmount = 2;
	public float getHealth()
	{
		return health;
	}
	public void decHealth()
	{
		health = health - decAmount;
	}
	public void setHealth(float health)
	{
		this.health = health;
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	protected void Update () {

		//set size to be related to health
		transform.localScale = new Vector3(0.35f + (0.65f * health / 100), 0.35f + (0.65f * health / 100), 0.35f + (0.65f * health / 100)) * 0.1f;
		if (health <= 0)
		{
			GetComponentInParent<GameManager>().destroy(this.gameObject);
			Destroy(this.gameObject);
		}

	}
}
