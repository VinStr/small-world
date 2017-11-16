using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexMapping : MonoBehaviour {

	public List<triangle> trianglesA = new List<triangle>();

	//change color of triangle at given index
	public void changeColor(int index, Color color)
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] colors = mesh.colors;

		colors[index*3] = color;
		colors[index*3+1] = color;
		colors[index*3+2] = color;

		mesh.colors = colors;

		GetComponent<MeshFilter>().mesh = mesh;
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

	// Use this for initialization
	void Start () {

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		// find all triangles and record to array list
		trianglesA = new List<triangle>();
		for (int i = 0; i < mesh.triangles.Length; i++)
		{
			if ((i + 1) % 3 == 0)
			{
				trianglesA.Add(new triangle(vertices[mesh.triangles[i - 2]], vertices[mesh.triangles[i - 1]], vertices[mesh.triangles[i]]));
			}
		}

		//initialize base color to gray
		// create new colors array where the colors will be created.
		Color[] colors = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++)
		{
			colors[i] = Color.gray;
		}
		// assign the array of colors to the Mesh.
		mesh.colors = colors;
		GetComponent<MeshFilter>().mesh = mesh;		

	}

	// Update is called once per frame
	void Update()
	{

	}

}
