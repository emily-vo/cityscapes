using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

public class CityGenerator : MonoBehaviour
{
	// Stores x and z pos of city
	public GameObject prefab;
	public Material buildingMaterial;
	public static System.Random random;
	public static double currRand;
	public const int BLOCK_SIZE = 3;
	public const int GRID_SIZE = 20;
	List<List<bool>> cityGrid;
	Dictionary<Vector2, List<GameObject>> buildings;
	HashSet<Vector2> gridPos;
	public Camera cloneCamera;
	public float buildingShift;
	// Use this for initialization
	void Start()
	{
		random = new System.Random();
		currRand = random.NextDouble ();
		//cityGrid = new List<List<bool>>();
		buildings = new Dictionary<Vector2, List<GameObject>>();
		gridPos = new HashSet<Vector2> ();
		/*
		for (float x = -GRID_SIZE / 2; x < GRID_SIZE / 2; x++)
		{
			for (float z = -GRID_SIZE / 2; z < GRID_SIZE / 2; z++)
			{
				Vector3 screenPoint = cloneCamera.WorldToViewportPoint(new Vector3(x, 0, z));
				// bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

				// Puts buildings in groups for "city blocking" and tests if in frustum
				if (x % 5 != BLOCK_SIZE && z % BLOCK_SIZE * 2 != 0 ||
					Vector3.Distance(new Vector3(x, 0, z), new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z)) < 5)
				{
					buildings.Add(new Vector2(x, z + buildingShift), AddRectangularSkyscraper (x, z + buildingShift));
				}
			}
		}
		*/


		// buildings.Add(new RectangularSkyscraper(0, 0));

	}

	// Update is called once per frame
	void Update()
	{
		// Removes buildings not in frustum

		List<Vector2> toRemove = new List<Vector2> ();
		foreach (Vector2 pos in buildings.Keys) {
			List<GameObject> o;
			buildings.TryGetValue (pos, out o);
			Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(pos.x, 0, pos.y));
			// bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
			if (Vector3.Distance(new Vector3(pos.x, 0, pos.y), new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z)) > 3.5)
			{
				toRemove.Add (pos);
  			}
		}

		foreach (Vector2 pos in toRemove) {
			List<GameObject> o;
			buildings.TryGetValue (pos, out o);
			buildings.Remove (pos);
			foreach (GameObject cube in o) {
				Destroy (cube);
			}
		}



		// Adds buildings in frustum

		float centerX = Camera.main.transform.position.x;
		float centerZ = Camera.main.transform.position.z;
		for (float x = (float) Mathf.Round(centerX - GRID_SIZE / 2.0f); x < centerX + GRID_SIZE / 2.0f; x++) {
			for (float z = (float) Mathf.Round(centerZ - GRID_SIZE / 2.0f); z < centerZ + GRID_SIZE / 2.0; z++) {
				float xShift = buildingShift*Mathf.Round(Camera.main.transform.forward.x);
				float zShift = buildingShift*Mathf.Round(Camera.main.transform.forward.z);
				Vector3 screenPoint = cloneCamera.WorldToViewportPoint(new Vector3(x, 0, z));
				// bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
				if (!buildings.ContainsKey (new Vector2(x + xShift, z + zShift)) &&
					Vector3.Distance(new Vector3(x, 0, z), new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z)) < 3.5) {
					buildings.Add(new Vector2(x + xShift, z + zShift),
						AddRectangularSkyscraper(x + xShift, z + zShift));
					//print (Mathf.Round (Camera.main.transform.forward.x));
					//print (Mathf.Round (Camera.main.transform.forward.z));
				}
			}
		}


	}

	public List<GameObject> AddRectangularSkyscraper(float x, float z) {
		List<GameObject> cubes = new List<GameObject>();
		System.Random rnd = new System.Random ((int) ((x.ToString() + "," + z.ToString()).GetHashCode() * currRand));
		float maxHeight = (float)((rnd.NextDouble() * 3) + 5) / 4.0f;
		float gridZOffset = (float)(rnd.NextDouble() / 2);
		float gridXOffset = (float)(rnd.NextDouble() / 2);
		// Stores all geometry associated with this building
		float xWidth = (float)(rnd.NextDouble() * 0.3 + 0.1);
		float zWidth = (float)(rnd.NextDouble() * 0.3 + 0.1);
		float xOffset = (float)(rnd.NextDouble() * Math.Abs(xWidth - 0.5) / 2);
		float zOffset = (float)(rnd.NextDouble() * Math.Abs(zWidth - 0.5) / 2);
		// Create main building structure
		GameObject main = GameObject.CreatePrimitive(PrimitiveType.Cube);
		main.transform.localScale = new Vector3(xWidth, maxHeight, zWidth);
		main.transform.localPosition = new Vector3(x + xOffset + gridXOffset, maxHeight / 2, z + zOffset + gridZOffset);
		main.GetComponent<Renderer>().material = buildingMaterial;

		Mesh mesh = main.GetComponent<MeshFilter>().mesh;
		float height = main.transform.localScale.y;
		float width = main.transform.localScale.x;
		float length = main.transform.localScale.z;
		float h = (float) rnd.NextDouble() * (1 - height / 2);
		float w = (float) rnd.NextDouble() * (1 - width / 1.2f);
		Vector2[] UVs = new Vector2[mesh.vertices.Length];
		// Front
		UVs[0] = new Vector2(0 + w, 0 + h);
		UVs[1] = new Vector2(width / 1.2f + w, 0 + h);
		UVs[2] = new Vector2(0 + w, height / 2 + h);
		UVs[3] = new Vector2(width / 1.2f + w, height / 2 + h);
		// Back
		h = (float)rnd.NextDouble() * (1 - height / 2);
		w = (float)rnd.NextDouble() * (1 - width / 1.2f);
		UVs[6] = new Vector2(width / 1.2f + w, 0 + h);
		UVs[7] = new Vector2(0 + w, 0 + h);
		UVs[10] = new Vector2(width / 1.2f + w, height / 2 + h);
		UVs[11] = new Vector2(0 + w, height / 2 + h);
		// Left
		h = (float)rnd.NextDouble() * (1 - height / 2);
		float l = (float) rnd.NextDouble() * (1 - length / 1.2f);
		UVs[16] = new Vector2(0 + l, 0 + h);
		UVs[17] = new Vector2(0 + l, height / 2 + h);
		UVs[18] = new Vector2(length / 1.2f + l, height / 2 + h);
		UVs[19] = new Vector2(length / 1.2f + l, 0 + h);
		// Right
		h = (float)rnd.NextDouble() * (1 - height / 2);
		l = (float)rnd.NextDouble() * (1 - length / 1.2f);
		UVs[20] = new Vector2(0 + l, 0 + h);
		UVs[21] = new Vector2(0 + l, height / 2 + h);
		UVs[22] = new Vector2(length / 1.2f + l, height / 2 + h);
		UVs[23] = new Vector2(length / 1.2f + l, 0 + h);
		mesh.uv = UVs;

		cubes.Add (main);


		int numBuildings = 1;
		if (rnd.NextDouble() < 0.2f)
		{
			numBuildings = rnd.Next(2, 4);
		}
		for (int i = 0; i < numBuildings; i++) {
			// Generate width of floor plan
			GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
			mesh = o.GetComponent<MeshFilter>().mesh;
			height = o.transform.localScale.y;
			width = o.transform.localScale.x;
			length = o.transform.localScale.z;
			h = (float) rnd.NextDouble() * (1 - height / 2);
			w = (float) rnd.NextDouble() * (1 - width / 1.2f);
			UVs = new Vector2[mesh.vertices.Length];
			// Front
			UVs[0] = new Vector2(0 + w, 0 + h);
			UVs[1] = new Vector2(width / 1.2f + w, 0 + h);
			UVs[2] = new Vector2(0 + w, height / 2 + h);
			UVs[3] = new Vector2(width / 1.2f + w, height / 2 + h);
			// Back
			h = (float)rnd.NextDouble() * (1 - height / 2);
			w = (float)rnd.NextDouble() * (1 - width / 1.2f);
			UVs[6] = new Vector2(width / 1.2f + w, 0 + h);
			UVs[7] = new Vector2(0 + w, 0 + h);
			UVs[10] = new Vector2(width / 1.2f + w, height / 2 + h);
			UVs[11] = new Vector2(0 + w, height / 2 + h);
			// Left
			h = (float)rnd.NextDouble() * (1 - height / 2);
			l = (float) rnd.NextDouble() * (1 - length / 1.2f);
			UVs[16] = new Vector2(0 + l, 0 + h);
			UVs[17] = new Vector2(0 + l, height / 2 + h);
			UVs[18] = new Vector2(length / 1.2f + l, height / 2 + h);
			UVs[19] = new Vector2(length / 1.2f + l, 0 + h);
			// Right
			h = (float)rnd.NextDouble() * (1 - height / 2);
			l = (float)rnd.NextDouble() * (1 - length / 1.2f);
			UVs[20] = new Vector2(0 + l, 0 + h);
			UVs[21] = new Vector2(0 + l, height / 2 + h);
			UVs[22] = new Vector2(length / 1.2f + l, height / 2 + h);
			UVs[23] = new Vector2(length / 1.2f + l, 0 + h);
			mesh.uv = UVs;
			float randHeight = (float)rnd.NextDouble () * maxHeight + 0.5f;
			xWidth = (float)(rnd.NextDouble () * 0.4 + 0.2);
			zWidth = (float)(rnd.NextDouble () * 0.4 + 0.2);
			xOffset = (float)(rnd.NextDouble () * Math.Abs (xWidth - 0.5) / 2);
			zOffset = (float)(rnd.NextDouble () * Math.Abs (zWidth - 0.5) / 2);
			Vector3 scale = new Vector3 (xWidth, randHeight, zWidth);
			Vector3 position = new Vector3 (x + xOffset + gridXOffset, randHeight / 2, z + zOffset + gridZOffset);
			o.transform.localScale = scale;
			o.transform.localPosition = position;
			//prefab.Instantiate (prefab.transform, position, Quaternion.identity);
			//GameObject o = (GameObject)Instantiate(prefab, position, Quaternion.identity);
			//o.GetComponent<Renderer>().material.mainTexture = texture;
			//o.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
			o.GetComponent<Renderer> ().material = buildingMaterial;
			cubes.Add (o);
		}
		return cubes;
	}

}
