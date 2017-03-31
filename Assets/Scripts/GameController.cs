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

	private List<List<bool>> map;
	private List<List<GameObject>> cubes;
	private Vector2 playerLocation;
	public GameObject ground;
	private Vector2 size;
	private Vector2 topLeft;


	// Use this for initialization
	void Start () {
		map = new List<List<bool>> ();
		cubes = new List<List<GameObject>> ();
		size = 10f * new Vector2 (ground.transform.localScale.x, ground.transform.localScale.z);
		topLeft = new Vector2 (size.x / -2f, size.y / 2f);
		print (size);
		print (topLeft);
		BuildInitialMap ();
		DrawMap ();
	}
	
	// Update is called once per frame
	void Update () {
		RandomMapChange ();
		DrawMap ();
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
				if (i%2==1 || j%2==1){
					map[i].Add(true);
					cubes[i][j].transform.position = new Vector3(topLeft.x+(float)i,topLeft.y - (float)j,1f);
				} else {
					map[i].Add(false);
					cubes[i][j].transform.position = new Vector3(topLeft.x+(float)i,topLeft.y - (float)j,0);
				}

			}
		}
	}

	private void RandomMapChange() {
		// with a certain probability, add a connection to the "cut"

	}

	private void DrawMap() {

	}

}
