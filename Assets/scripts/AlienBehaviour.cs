using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBehaviour : MonoBehaviour {

	private float health = 100f;

	public GameObject planet;
	private float radius = 1.5f;

	private GameObject target = null;
	private float range = 1.4f;

	public void findTarget()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, range);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[7] && colliders[i].gameObject == target)
			{
				return;
			}
		}

		for (int i = 0; i < colliders.Length; i++)
		{
			if(colliders[i].gameObject.tag == UnityEditorInternal.InternalEditorUtility.tags[7])
			{
				target = colliders[i].gameObject;
				return;
			}
		}
		target = null;
	}

	public void takeDamage()
	{
		health--;
	}

	// Use this for initialization
	void Start () {

		transform.position = Random.onUnitSphere * radius * 3;

	}

	private Vector3 velocity = new Vector3(0, 0, 0);
	private float t = 0;
	public static float damageRate = 0.2f;
	private bool arrived = false;
	// Update is called once per frame
	void Update () {

		transform.LookAt(planet.transform);
		transform.Rotate(new Vector3(-90, 0, 0));

		//animation to spin
		transform.GetChild(0).RotateAroundLocal(new Vector3(0,1,0), 1/Time.deltaTime * 0.001f);

		//set size to be related to health
		transform.GetChild(0).localScale = new Vector3(0.35f + (0.65f * health/100), 0.35f + (0.65f * health/100), 0.35f + (0.65f * health/100));
		if (health <= 0)
		{
			GetComponentInParent<GameManager>().destroy(this.gameObject);
			Destroy(this.gameObject);
		}

		//find target to fire at
		findTarget();
		if (target == null)
		{
			transform.GetChild(1).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0,0,0));

		}
		else
		{
			transform.GetChild(1).GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(target.transform.position));
			t += Time.deltaTime / damageRate;
			if (t >= 1)
			{
				if (target != null) target.transform.GetComponent<workerBehaviour>().takeDamage();
				t = 0;
			}
		}

		//random movement over the surface
		velocity += Random.insideUnitSphere;
		if (Vector3.Distance(planet.transform.position, transform.position) > radius && arrived == false) velocity.y = -5;
		else {
			arrived = true;
			velocity.y = 0;
		}
		float maxSpeed = 12f;
		velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
		velocity.z = Mathf.Clamp(velocity.z, -maxSpeed, maxSpeed);
		if (transform.position.x < 0.1 && transform.position.x > -0.1 && transform.position.z < 0.1 && transform.position.z > -0.1)
		{
			//velocity = -velocity;
			if (transform.position.y > 0)
			{
				if(arrived== true) transform.position = new Vector3(transform.position.x, radius, transform.position.z);
				transform.position = transform.position + transform.TransformVector(new Vector3(-0.1f, 0, -0.1f));
				velocity.z = -velocity.z;
			}
			else
			{
				if (arrived == true) transform.position = new Vector3(transform.position.x, -radius, transform.position.z);
				transform.position = transform.position + transform.TransformVector(new Vector3(0.1f, 0, 0.1f));
				velocity.z = -velocity.z;
			}
		}
		transform.position = transform.position + Time.deltaTime * transform.TransformVector(velocity);

	}
}
