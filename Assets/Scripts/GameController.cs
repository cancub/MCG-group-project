using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct edge
{
	public Vector2 p,q;
	public float cost;
}

public struct trafficLight
{
	public Vector2 location;
	public bool status;
}

public class GameController : MonoBehaviour {

	// the goal here is to have a map made up of vertices and edges between those vertices.
	// visually, roads that are capable of being traversed will be represented by empty space
	// while the portions of the map that are buildings will be represented by black cubes

	// the first objective is to build a grid of roads based on these edges
	// and remember a subset of vertices which represent a streetlight. DONE

	// the second objective is to control the movement of a character by setting traffic lights
	// on and off. traffic lights cannot be off in both directions. to achieve this, when a traffic
	// light's value is set to be <true>, the vertices immediately to the north and south of the intersection
	// are set to be off, placing black cubes there and removing the edges from the edge list that include this
	// vertex. Meanwhile in the east and west directions, the edges that were removed because of the previous setting
	// of the light are re-added (i.e., connections to the intersection and the vertices to the right and left, 
	// respectively). This process is visually represented as cubes being placed in the cells directly to the east and
	// west of the intersection. Vice versa for east-west, north-south for <false>.

	public GameObject ground;
	public GameObject mapParent;
	public float lightSwitchProbability;
	public float intersectionLightProbability;
	public Vector2 startingLocation;
	private List<edge> map;
	private List<edge> paths;
	private List<List<GameObject>> cubes;
	private List<trafficLight> trafficLights;
	private List<float> lightPeriods;
	private List<float> nextLightSwitch;
	private Vector2 size;
	private Vector2 topLeft;
	private List<Vector2> changesLocations;


	// Use this for initialization
	void Start () {
		map = new List<edge> ();
		cubes = new List<List<GameObject>> ();
		trafficLights = new List<trafficLight> ();
		lightPeriods = new List<float> ();
		nextLightSwitch = new List<float> ();
		size = 10f * new Vector2 (ground.transform.localScale.x, ground.transform.localScale.z) - new Vector2(1f,1f);
		topLeft = new Vector2 (size.x/ -2f, size.y / 2f);
//		print (size);
//		print (topLeft);
		BuildInitialMap ();
		paths = LPAStar.GetInitialPaths (startingLocation, new Vector2(2,2), map);
	}
	
	// Update is called once per frame
	void Update () {
//		RandomMapChange ();
		ScheduledMapChanges();

	}

	private void BuildInitialMap() {
		// build an initial grid that is essentially a network of straight roads

		edge e;

		for (int i = 0; i < size.x + 1; i++) {
			cubes.Add(new List<GameObject>());
			for (int j = 0; j < size.y + 1; j++) {
				// the cubes will be used as a visual reference for the pathfinding
				cubes[i].Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
				cubes [i] [j].transform.parent = mapParent.transform;

				if (i % 4 == 2 || j % 4 == 2) {
					if (i % 4 == 2) {
						// roads in the east-west direction 
						if (j > 0) {
							// add the edge from this node to the node to the west
							e.p = new Vector2 (i, j - 1);
							e.q = new Vector2 (i, j);
							e.cost = (e.p - e.q).magnitude;
							map.Add (e);
						}
						if (j < (int)size.y) {
							// add the edge from this node to the node to the east
							e.p = new Vector2 (i, j);
							e.q = new Vector2 (i, j + 1);
							e.cost = (e.p - e.q).magnitude;
							map.Add (e);
						}

					}

					if (j % 4 == 2) {
						// roads in the north-south direction
						if (i > 0) {
							// add the edge from this node to the node to the north
							e.p = new Vector2 (i - 1, j);
							e.q = new Vector2 (i, j);
							e.cost = (e.p - e.q).magnitude;
							map.Add (e);
						}
						if (i < (int)size.x) {
							// add the edge from this node to the node to the south
							e.p = new Vector2 (i, j);
							e.q = new Vector2 (i + 1, j);
							e.cost = (e.p - e.q).magnitude;
							map.Add (e);
						}
					}
					// no matter what, the cube should be set to be invisible at this location
					cubes [i] [j].transform.position = new Vector3 (topLeft.x + (float)i, topLeft.y - (float)j, 1f);
						
				} else {
					// othwerise this is a cube that is part of a building, so create it but make it visible
					// by placing it on the plane
					cubes [i] [j].transform.position = new Vector3 (topLeft.x + (float)i, topLeft.y - (float)j, 0);
				}
			}
		}

		trafficLight light;

		// now go through the intersections and create and set traffic lights
		for (int i = 0; i < (int)size.x; i++) {
			for (int j = 0; j < (int)size.y; j++) {
				if (i % 4 == 2 && j % 4 == 2 && Random.value < intersectionLightProbability) {
					// if both the x and y are streets then we have an intersection. with some probability,
					// add a traffic light at intersections and provide a period/timer for switching on and off
					light.location = new Vector2(i,j);
					light.status = (Random.value > 0.5f);
					trafficLights.Add (light);
					SetTrafficLight (light);
					lightPeriods.Add (Random.Range (3f, 5f));
					nextLightSwitch.Add (lightPeriods [lightPeriods.Count - 1]);

				}
			}
		}
	}

	private void ScheduledMapChanges() {
		// go through each of the traffic lights and, if the period of time is up, flip the light and update the
		// next time to flip the light
//		trafficLight schedLight;


		for (int i = 0; i < trafficLights.Count; i++) {
			if (Time.time > nextLightSwitch [i]) {
				// update the next light change with the period
				nextLightSwitch[i] += lightPeriods[i];
				ChangeLight (i);
			}
		}
	}
//
//	private void RandomMapChange() {
//		// with a certain probability, turn on a red light at an intersection
//		if (Random.value < lightSwitchProbability) {
//			Vector2 randLight = trafficLights [(int)Random.Range (0, trafficLights.Count)];
//			ChangeLight ((int)randLight.x, (int)randLight.y);
//		}
//	}

	private void SetTrafficLight (trafficLight light) {
		Vector3 cubePosition;

		if (light.status) {
			// true representes that traffic is blocked in the north-south direction, so remove the edges
			// that contain these vertices and show the cubes associated with these vertices

			// we know the location of the light, so we can simply break any edges that contain the vertices above
			// and below
			BreakEdges(new Vector2(light.location.x-1f,light.location.y));
			BreakEdges(new Vector2(light.location.x+1f,light.location.y));

			// now make visible the cubes to the north and south
			cubePosition = cubes [(int)light.location.x - 1] [(int)light.location.y].transform.position;
			cubePosition.z = 0;
			cubes [(int)light.location.x - 1] [(int)light.location.y].transform.position = cubePosition;

			cubePosition = cubes [(int)light.location.x + 1] [(int)light.location.y].transform.position;
			cubePosition.z = 0;
			cubes [(int)light.location.x + 1] [(int)light.location.y].transform.position = cubePosition;
		} else {
			// false representes that traffic is blocked in the east-west direction, so remove the edges
			// that contain these vertices and show the cubes associated with these vertices

			// we know the location of the light, so we can simply break any edges that contain the vertices to the
			// left and right
			BreakEdges(new Vector2(light.location.x,light.location.y-1f));
			BreakEdges(new Vector2(light.location.x,light.location.y+1f));

			cubePosition = cubes [(int)light.location.x] [(int)light.location.y - 1].transform.position;
			cubePosition.z = 0;
			cubes [(int)light.location.x] [(int)light.location.y - 1].transform.position = cubePosition;

			cubePosition = cubes [(int)light.location.x] [(int)light.location.y+1].transform.position;
			cubePosition.z = 0;
			cubes [(int)light.location.x] [(int)light.location.y +1].transform.position = cubePosition;
		}
	}

	private void ChangeLight(int index) {
		// this is essentially a modified version of SetTrafficLight. Here though, there is a switch rather than a set

		// flip the light
		trafficLight light = trafficLights [index];
		light.status = !light.status;
		trafficLights [index] = light;


		if (light.status) {
			// true represents that traffic is blocked in the north-south direction, so remove the edges
			// that contain these vertices and show the cubes associated with these vertices

			// we know the location of the light, so we can simply start traffic above and below and stop traffic
			// to the left and right
			StopTraffic((int)light.location.x - 1, (int)light.location.y);
			StopTraffic((int)light.location.x + 1, (int)light.location.y);
			// start traffic in the east-west direction
			StartTraffic ((int)light.location.x, (int)light.location.y - 1, !light.status);
			StartTraffic ((int)light.location.x, (int)light.location.y + 1, !light.status);
		} else {
			// false representes that traffic is blocked in the east-west direction, so remove the edges
			// that contain these vertices and show the cubes associated with these vertices
			StopTraffic((int)light.location.x, (int)light.location.y-1);
			StopTraffic((int)light.location.x, (int)light.location.y+1);
			// start the traffic in the north-south direction
			StartTraffic ((int)light.location.x-1, (int)light.location.y, !light.status);
			StartTraffic ((int)light.location.x+1, (int)light.location.y, !light.status);
		}
	}

	private void StopTraffic(int i, int j) {
		Vector3 cubePosition;

		// break all the edges that contain this vertex
		BreakEdges (new Vector2 ((float)i, (float)j));

		// now make visible the cubes at this node to visually stop traffic
		cubePosition = cubes [i] [j].transform.position;
		cubePosition.z = 0;
		cubes [i] [j].transform.position = cubePosition;
	}

	private void StartTraffic(int i, int j, bool ns) {
		Vector3 cubePosition;
		// if ns is true, it means that we are building edges in the north-south direction
		// if ns is false, build edges in the east-west.
		edge e;
		if (ns) {
			// north edge
			e.p = new Vector2 ((float)i - 1f, (float)j);
			e.q = new Vector2 ((float)i, (float)j);
			e.cost = (e.p - e.q).magnitude;
			map.Add (e);

			// south edge
			e.p = new Vector2 ((float)i, (float)j);
			e.q = new Vector2 ((float)i + 1f, (float)j);
			e.cost = (e.p - e.q).magnitude;
			map.Add (e);
		} else {
			// west edge
			e.p = new Vector2 ((float)i, (float)j-1f);
			e.q = new Vector2 ((float)i, (float)j);
			e.cost = (e.p - e.q).magnitude;
			map.Add (e);

			// south edge
			e.p = new Vector2 ((float)i, (float)j);
			e.q = new Vector2 ((float)i, (float)j+1f);
			e.cost = (e.p - e.q).magnitude;
			map.Add (e);
		}

		// in either case, make the cube at this position invisible so that traffic can roll through
		cubePosition = cubes [i] [j].transform.position;
		cubePosition.z = 1f;
		cubes [i] [j].transform.position = cubePosition;
	}

	private void BreakEdges(Vector2 vertex) {
		// walk backwards through the edges list and, if an edge contains this vertex, remove the edge
		for (int i = map.Count - 1; i >= 0; i--) {
			if ((map [i].p - vertex).magnitude < 0.1f || (map [i].q - vertex).magnitude < 0.1f) {
				map.RemoveAt (i);
			}
		}
	}
}
