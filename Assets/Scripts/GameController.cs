using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct vertex
//{
//	public int id;
//	public Vector2 location;
//}
//
//public struct edge
//{
//	public vertex p,q;
//}

public class GameController : MonoBehaviour {

	public GameObject ground;
	public GameObject mapParent;
	public float lightSwitchProbability;
	public float intersectionLightProbability;
	private List<List<bool>> map;
	private List<List<GameObject>> cubes;
	private List<Vector2> trafficLights;
	private List<float> lightPeriods;
	private List<float> nextLightSwitch;
	private Vector2 playerLocation;
	private Vector2 size;
	private Vector2 topLeft;


	// Use this for initialization
	void Start () {
		map = new List<List<bool>> ();
		cubes = new List<List<GameObject>> ();
		trafficLights = new List<Vector2> ();
		lightPeriods = new List<float> ();
		nextLightSwitch = new List<float> ();
		size = 10f * new Vector2 (ground.transform.localScale.x, ground.transform.localScale.z) - new Vector2(1f,1f);
		topLeft = new Vector2 (size.x/ -2f, size.y / 2f);
		print (size);
		print (topLeft);
		BuildInitialMap ();
	}
	
	// Update is called once per frame
	void Update () {
//		RandomMapChange ();
		ScheduledMapChanges();
	}

	private void BuildInitialMap() {
		// build an initial grid that is essentially a network of straight roads 

		// the map can hold 12 x 12 cells to build the network, so the initial map
		// will simply have roads set as the locations at odd columns and rows (zero indexed)

		for (int i = 0; i < size.x + 1; i++) {
			map.Add(new List<bool>());
			cubes.Add(new List<GameObject>());
			for (int j = 0; j < size.y + 1; j++) {
				// the cubes will be used as a visual reference for the pathfinding
				cubes[i].Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
				cubes [i] [j].transform.parent = mapParent.transform;

				if (i%2==1 || j%2==1){
					map[i].Add(true);
					cubes[i][j].transform.position = new Vector3(topLeft.x+(float)i,topLeft.y - (float)j,1f);
				} else {
					map[i].Add(false);
					cubes[i][j].transform.position = new Vector3(topLeft.x+(float)i,topLeft.y - (float)j,0);
				}

				if (i % 2 == 1 && j % 2 == 1 && Random.value < intersectionLightProbability) {
					// add a traffic light at intersections and provide a period/timer for switching on and off
					trafficLights.Add (new Vector2((float)i,(float)j));
					lightPeriods.Add (Random.Range (3f, 5f));
					nextLightSwitch.Add (lightPeriods [lightPeriods.Count - 1]);
					// randomly set some of the lights as on to begin
					if (Random.value < lightSwitchProbability) {
						ChangeLight (i, j);
					}
				}

			}
		}
	}

	private void ScheduledMapChanges() {
		// go through each of the traffic lights and, if the period of time is up, flip the light and update the
		// next time to flip the light
		Vector2 schedLight;


		for (int i = 0; i < trafficLights.Count; i++) {
			if (Time.time > nextLightSwitch [i]) {
				// update the next light change with the period
				nextLightSwitch[i] += lightPeriods[i];
				schedLight = trafficLights [i];
				ChangeLight ((int)schedLight.x, (int)schedLight.y);
			}
		}
	}

	private void RandomMapChange() {
		// with a certain probability, turn on a red light at an intersection
		if (Random.value < lightSwitchProbability) {
			Vector2 randLight = trafficLights [(int)Random.Range (0, trafficLights.Count)];
			ChangeLight ((int)randLight.x, (int)randLight.y);
		}
	}

	private void ChangeLight(int i, int j) {
		// block traffic both visually and in the algorithm
		Vector3 newPosition = cubes[i][j].transform.position;


		if (map[i][j]) {
			map [i] [j] = false; // for the alg
			newPosition.z = 0; // for visual
		} else {
			map [i] [j] = true;
			newPosition.z = 1f;
		}

		cubes[i][j].transform.position = newPosition;
	}

}
